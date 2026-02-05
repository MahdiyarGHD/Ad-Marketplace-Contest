using System.Linq.Expressions;
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
    ITelegramBotClient botClient)
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

        var pricings = req.Pricings
            .Select(p => (p.AdFormat, p.PriceType, p.PriceTon))
            .ToList();

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
