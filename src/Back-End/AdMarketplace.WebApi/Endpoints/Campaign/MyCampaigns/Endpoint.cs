using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Campaign.MyCampaigns;

public class Endpoint(ICampaignService campaignService, IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/campaigns/my");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await campaignService.GetByAdvertiserIdAsync(userResult.Value.Id, req.Skip, req.Take);

        if (result.IsError)
            return result.Errors;

        var campaigns = result.Value.Select(c => new CampaignItem
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            BudgetTon = c.BudgetTon,
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
