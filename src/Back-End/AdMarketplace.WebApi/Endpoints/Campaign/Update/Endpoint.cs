using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Campaign.Update;

public class Endpoint(ICampaignService campaignService, IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Put("/api/campaigns/{Id}");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await campaignService.UpdateAsync(
            id: req.Id,
            advertiserId: userResult.Value.Id,
            title: req.Title,
            description: req.Description,
            budgetTon: req.BudgetTon,
            categoryId: req.CategoryId,
            brief: req.Brief,
            maxPricePerPlacement: req.MaxPricePerPlacement,
            targeting: req.Targeting,
            creative: req.Creative,
            startsAt: req.StartsAt,
            endsAt: req.EndsAt,
            applicationDeadline: req.ApplicationDeadline);

        if (result.IsError)
            return result.Errors;

        var campaign = result.Value;

        return new Response
        {
            Id = campaign.Id,
            Title = campaign.Title,
            Description = campaign.Description,
            BudgetTon = campaign.BudgetTon,
            Status = campaign.Status,
            CategoryId = campaign.CategoryId,
            CategoryName = campaign.Category?.Name,
            Targeting = campaign.TargetingJson,
            Creative = campaign.CreativeJson,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}
