using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace AdMarketplace.Infra.Interfaces;

public interface IHandler
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}