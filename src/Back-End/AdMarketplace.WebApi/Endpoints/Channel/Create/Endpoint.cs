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
    IUserChannelConnectionService userChannelConnectionService,
    IChannelPricingService channelPricingService,
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
            return Error.Validation("ChannelPricing.DuplicatePricing", "Duplicate pricing entries found");

        try
        {
            var chat = await botClient.GetChat(req.ChatId, ct);
            var me = await botClient.GetMe(ct);
            var botMember = await botClient.GetChatMember(req.ChatId, me.Id, ct);
            
            if (botMember is not ChatMemberAdministrator { CanPromoteMembers: true, CanInviteUsers: true })
            {
                return Error.Forbidden("Channel.BotNotAdmin", "The bot must be an administrator in the channel");
            }

            var result = await channelService.CreateAsync(
                chatId: req.ChatId,
                title: chat.Title ?? connectionResult.Value.Title,
                username: chat.Username,
                description: chat.Description,
                ownerId: userResult.Value.Id,
                categoryId: req.CategoryId);

            if (result.IsError)
                return result.Errors;

            var pricingResult = await channelPricingService.CreateBulkAsync(result.Value.Id, pricings);

            if (pricingResult.IsError)
                return pricingResult.Errors;

            return new Response
            {
                Id = result.Value.Id,
                ChatId = result.Value.ChatId,
                Title = result.Value.Title,
                CreatedAt = result.Value.CreatedAt,
                Pricings = pricingResult.Value.Select(p => new PricingResponse
                {
                    Id = p.Id,
                    AdFormat = p.AdFormat,
                    PriceType = p.PriceType,
                    PriceTon = p.PriceTon
                }).ToList()
            };
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException ex)
        {
            return Error.Failure("Telegram.ApiError", $"Failed to verify channel: {ex.Message}");
        }
    }
}
