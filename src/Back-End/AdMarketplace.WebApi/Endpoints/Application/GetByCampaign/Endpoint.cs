using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Application.GetByCampaign;

public class Endpoint(
    ICampaignApplicationService applicationService,
    ICampaignService campaignService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/campaigns/{CampaignId}/applications");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var campaignResult = await campaignService.GetByIdAsync(req.CampaignId);
        if (campaignResult.IsError)
            return campaignResult.Errors;

        if (campaignResult.Value.AdvertiserId != userResult.Value.Id)
            return Error.Forbidden("Campaign.NotOwner", "You are not the owner of this campaign");

        var result = await applicationService.GetByCampaignIdAsync(req.CampaignId, req.Skip, req.Take);

        if (result.IsError)
            return result.Errors;

        var applications = result.Value.Select(a => new ApplicationItem
        {
            Id = a.Id,
            Channel = new ChannelInfo
            {
                Id = a.Channel.Id,
                ChatId = a.Channel.ChatId,
                Title = a.Channel.Title,
                Username = a.Channel.Username,
                Description = a.Channel.Description,
                SubscriberCount = a.Channel.SubscriberCount,
                PremiumCount = a.Channel.PremiumCount,
                AverageViews = a.Channel.AverageViews,
                LanguageDistribution = a.Channel.LanguageDistributionJson,
                CategoryId = a.Channel.CategoryId,
                CategoryName = a.Channel.Category?.Name,
                Pricings = a.Channel.Pricings.Select(p => new ChannelPricingInfo
                {
                    Id = p.Id,
                    AdFormat = p.AdFormat,
                    PriceType = p.PriceType,
                    PriceTon = p.PriceTon
                }).ToList()
            },
            ProposedAdFormat = a.ProposedAdFormat,
            ProposedPriceType = a.ProposedPriceType,
            ProposedPriceTon = a.ProposedPriceTon,
            ProposedPostingTime = a.ProposedPostingTime,
            Message = a.Message,
            Status = a.Status,
            RejectionReason = a.RejectionReason,
            CreatedAt = a.CreatedAt
        }).ToList();

        return new Response { Applications = applications };
    }
}
