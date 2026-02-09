using System.Linq.Expressions;
using AdMarketplace.Database;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Endpoint(
    IChannelService channelService,
    IUserService userService,
    IAgentService agentService,
    IUserChannelConnectionService userChannelConnectionService,
    IChannelPricingService channelPricingService,
    IAnalyticsUpdateQueue analyticsQueue,
    ITelegramBotClient botClient,
    AdMarketDbContext dbContext)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/channels");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var connectionResult = await userChannelConnectionService.GetByUserAndChatIdAsync(
            userResult.Value.Id,
            req.ChatId);

        if (connectionResult.IsError)
            return connectionResult.Errors;

        var pricings = req.Pricings
            .Select(p => (p.AdFormat, p.PriceType, p.PriceTon))
            .ToList();

        if (pricings.Count == 0)
            return Error.Validation("ChannelPricing.EmptyList", "At least one pricing must be provided");

        if (pricings.Any(p => p.PriceTon <= 0))
            return Error.Validation("ChannelPricing.InvalidPrice", "All prices must be greater than zero");

        var hasDuplicates = pricings
            .GroupBy(p => (p.AdFormat, p.PriceType))
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
            return Error.Validation("ChannelPricing.DuplicatePricing", "Duplicate pricing entries found in request");

        var botValidationResult = await ValidateBotPermissionsAsync(req.ChatId, ct);
        if (botValidationResult.IsError)
            return botValidationResult.Errors;

        var chat = botValidationResult.Value;

        var channelResult = await channelService.CreateAsync(
            chatId: req.ChatId,
            title: chat.Title ?? connectionResult.Value.Title,
            username: chat.Username,
            description: null,
            ownerId: userResult.Value.Id,
            categoryId: req.CategoryId);

        if (channelResult.IsError)
            return channelResult.Errors;

        var pricingResult = await channelPricingService.CreateBulkAsync(channelResult.Value.Id, pricings);
        if (pricingResult.IsError)
            return pricingResult.Errors;

        var attachResult = await agentService.AttachAgentToChannel(channelResult.Value.Id);

        if(!attachResult.IsError)
            await analyticsQueue.EnqueueAsync(new AnalyticsUpdateMessage(channelResult.Value.Id, DateTime.UtcNow), ct);

        return MapToResponse(channelResult.Value, pricingResult.Value);
    }

    private async Task<ErrorOr<Chat>> ValidateBotPermissionsAsync(long chatId, CancellationToken ct)
    {
        try
        {
            var chat = await botClient.GetChat(chatId, ct);
            var me = await botClient.GetMe(ct);
            var botMember = await botClient.GetChatMember(chatId, me.Id, ct);

            if (botMember is not ChatMemberAdministrator 
            { 
                CanPromoteMembers: true, 
                CanInviteUsers: true, 
                CanDeleteMessages: true, 
                CanPostMessages: true,
                CanEditMessages: true,
            })
                return Error.Forbidden("Channel.BotNotAdmin", "The bot must be an administrator in the channel");

            return chat;
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException ex)
        {
            return Error.Failure("Telegram.ApiError", $"Failed to verify channel: {ex.Message}");
        }
    }

    private static Response MapToResponse(Database.Models.Channel channel, List<Database.Models.ChannelPricing> pricings)
    {
        return new Response
        {
            Id = channel.Id,
            ChatId = channel.ChatId,
            Title = channel.Title,
            Status = channel.Status,
            CreatedAt = channel.CreatedAt,
            Pricings = [..pricings.Select(p => new PricingResponse
            {
                Id = p.Id,
                AdFormat = p.AdFormat,
                PriceType = p.PriceType,
                PriceTon = p.PriceTon
            })]
        };
    }
}
