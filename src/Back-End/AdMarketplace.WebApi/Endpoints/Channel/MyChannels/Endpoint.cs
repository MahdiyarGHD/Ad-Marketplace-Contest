using AdMarketplace.Extensions;
using AdMarketplace.Infra;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.MyChannels;

public class Endpoint(IChannelService channelService, IUserService userService)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/my");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelService.GetByOwnerIdAsync(userResult.Value.Id);

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Channels = result.Value.Select(c => new ChannelItem
            {
                Id = c.Id,
                TelegramChannelId = c.ChatId,
                Title = c.Title,
                Username = c.Username,
                Description = c.Description,
                SubscriberCount = c.SubscriberCount,
                AverageViews = c.AverageViews,
                LanguageDistributionJson = c.LanguageDistributionJson,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }
}
