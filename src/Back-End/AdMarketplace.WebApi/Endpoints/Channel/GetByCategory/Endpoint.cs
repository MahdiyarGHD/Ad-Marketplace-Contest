using AdMarketplace.Domain.Contracts.Responses;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.GetByCategory;

public class Endpoint(IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/by-category/{CategoryId}");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await channelService.GetByCategoryIdAsync(req.CategoryId, req.Skip, req.Take);

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Channels = result.Value.Select(c => new ChannelResponseContract
            {
                Id = c.Id,
                ChatId = c.ChatId,
                Title = c.Title,
                Username = c.Username,
                Description = c.Description,
                SubscriberCount = c.SubscriberCount,
                AverageViews = c.AverageViews,
                LanguageDistributionJson = c.LanguageDistributionJson,
            }).ToList()
        };
    }
}
