using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Campaign.GetActive;

public class Endpoint(ICampaignService campaignService) : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/campaigns/active");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await campaignService.SearchAsync(
            categoryId: req.CategoryId,
            minBudget: req.MinBudget,
            maxBudget: req.MaxBudget,
            status: CampaignStatusType.Active,
            adFormat: req.AdFormat,
            priceType: req.PriceType,
            skip: req.Skip,
            take: req.Take);

        if (result.IsError)
            return result.Errors;

        var campaigns = result.Value.Select(c => new CampaignItem
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            BudgetTon = c.BudgetTon,
            MaxPricePerPlacement = c.MaxPricePerPlacement,
            Status = c.Status,
            CategoryId = c.CategoryId,
            CategoryName = c.Category?.Name,
            Targeting = c.TargetingJson,
            ApplicationDeadline = c.ApplicationDeadline,
            CreatedAt = c.CreatedAt
        }).ToList();

        return new Response { Campaigns = campaigns };
    }
}
