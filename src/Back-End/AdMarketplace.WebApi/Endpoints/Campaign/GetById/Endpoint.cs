using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Campaign.GetById;

public class Endpoint(ICampaignService campaignService) : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/campaigns/{Id}");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await campaignService.GetByIdAsync(req.Id);

        if (result.IsError)
            return result.Errors;

        var campaign = result.Value;

        return new Response
        {
            Id = campaign.Id,
            AdvertiserId = campaign.AdvertiserId,
            Title = campaign.Title,
            Description = campaign.Description,
            BudgetTon = campaign.BudgetTon,
            Status = campaign.Status,
            CategoryId = campaign.CategoryId,
            CategoryName = campaign.Category?.Name,
            Brief = campaign.Brief,
            MaxPricePerPlacement = campaign.MaxPricePerPlacement,
            Targeting = campaign.TargetingJson,
            Creative = campaign.CreativeJson,
            StartsAt = campaign.StartsAt,
            EndsAt = campaign.EndsAt,
            ApplicationDeadline = campaign.ApplicationDeadline,
            CreatedAt = campaign.CreatedAt
        };
    }
}
