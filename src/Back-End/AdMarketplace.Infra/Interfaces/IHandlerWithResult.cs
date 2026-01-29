using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace AdMarketplace.Infra.Interfaces;

public interface IHandlerWithResult<TResult>
{
    Task<TResult> HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}