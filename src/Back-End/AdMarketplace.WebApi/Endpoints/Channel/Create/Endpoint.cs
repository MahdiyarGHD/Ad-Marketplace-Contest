using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Endpoint(
    IChannelService channelService,
    IUserService userService,
    IUserChannelConnectionService userChannelConnectionService,
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

            return new Response
            {
                Id = result.Value.Id,
                ChatId = result.Value.ChatId,
                Title = result.Value.Title,
                CreatedAt = result.Value.CreatedAt
            };
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException ex)
        {
            return Error.Failure("Telegram.ApiError", $"Failed to verify channel: {ex.Message}");
        }
    }
}
