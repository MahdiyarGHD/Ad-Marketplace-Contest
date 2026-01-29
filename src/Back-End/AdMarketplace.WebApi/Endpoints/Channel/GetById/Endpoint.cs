using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.GetById;

public class Endpoint(IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/{Id}");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await channelService.GetByIdAsync(req.Id);

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Id = result.Value.Id,
            TelegramChannelId = result.Value.ChatId,
            Title = result.Value.Title,
            Username = result.Value.Username,
            Description = result.Value.Description,
            SubscriberCount = result.Value.SubscriberCount,
            AverageViews = result.Value.AverageViews,
            LanguageDistributionJson = result.Value.LanguageDistributionJson,
            OwnerId = result.Value.OwnerId,
            CategoryId = result.Value.CategoryId,
            CategoryName = result.Value.Category?.Name,
            CreatedAt = result.Value.CreatedAt
        };
    }
}
