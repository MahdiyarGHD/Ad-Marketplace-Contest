using AdMarketplace.Bot.Extensions;
using AdMarketplace.Bot.Handlers;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Bot;

public sealed class UpdateHandler(
    IUserService userService,
    UserChannelConnectionHandler userChannelConnectionHandler,
    DraftSubmissionHandler draftSubmissionHandler)
{
    public async Task HandleUpdate(Update update, CancellationToken ct)
    {
        if (!update.IsRelevantUpdate())
            return;

        if (update.GetUserFromUpdate() is { } telegramUser)
            await userService.EnsureExistsAsync(new InitDataUserContract
            {
                Id = telegramUser.Id,
                FirstName = telegramUser.FirstName,
                LastName = telegramUser.LastName,
                UserName = telegramUser.Username
            });
            
        
        // Update Handlers
        await userChannelConnectionHandler.HandleUpdateAsync(update, ct);
        await draftSubmissionHandler.HandleUpdateAsync(update, ct);
        await draftSubmissionHandler.HandleCallbackAsync(update, ct);
    }

}