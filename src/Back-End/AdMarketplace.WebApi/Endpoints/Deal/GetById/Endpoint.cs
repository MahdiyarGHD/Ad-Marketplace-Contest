using AdMarketplace.Domain.Types;
using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.GetById;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/deals/{Id}");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await dealService.GetByIdAsync(req.Id);
        if (result.IsError)
            return result.Errors;

        var deal = result.Value;

        if (deal.AdvertiserId != userResult.Value.Id && deal.Channel.OwnerId != userResult.Value.Id)
            return Error.Forbidden("Deal.Forbidden", "You don't have access to this deal");

        return new Response
        {
            Id = deal.Id,
            CampaignId = deal.CampaignId,
            CampaignTitle = deal.Campaign?.Title,
            ChannelId = deal.ChannelId,
            ChannelTitle = deal.Channel.Title,
            AdvertiserId = deal.AdvertiserId,
            AdvertiserFirstName = deal.Advertiser.FirstName,
            AdvertiserLastName = deal.Advertiser.LastName,
            AmountTon = deal.AmountTon,
            AdFormat = deal.AdFormat,
            PriceType = deal.PriceType,
            Status = deal.Status,
            DraftStatus = deal.DraftStatus,
            AdvertiserFeedback = deal.AdvertiserFeedback,
            ScheduledPostTime = deal.ScheduledPostTime,
            ActualPostTime = deal.ActualPostTime,
            PostedMessageId = deal.PostedMessageId,
            DraftMessageId = deal.DraftMessageId,
            EscrowWalletAddress = deal.EscrowWalletAddress,
            TransactionHash = deal.TransactionHash,
            AutoCancelAt = deal.AutoCancelAt,
            FundsReleasedAt = deal.FundsReleasedAt,
            CreatedAt = deal.CreatedAt,
            UpdatedAt = deal.UpdatedAt
        };
    }
}
