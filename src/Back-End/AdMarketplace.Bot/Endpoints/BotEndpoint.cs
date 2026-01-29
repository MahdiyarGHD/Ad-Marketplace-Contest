
using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
namespace AdMarketplace.Bot.Endpoints;



[HideFromDocs]
public class BotEndpoint(UpdateHandler updateHandler, ILogger<BotEndpoint> logger)
    : Endpoint<BotRequest>
{
    public override void Configure()
    {
        Post("/bot");
        Description(b => b.ExcludeFromDescription());
        AllowAnonymous(); 
    }

    public override async Task HandleAsync(BotRequest req, CancellationToken ct)
    {
        try
        {
            await updateHandler.HandleUpdate(req.Update, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling update");
        }

        await Send.OkAsync(cancellation: ct);
    }
}

public class BotRequest
{
    [FastEndpoints.FromBody] 
    public Update Update { get; set; } = null!;
}

