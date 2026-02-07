using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class Campaign : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public Guid AdvertiserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? Brief { get; private set; }
    
    public decimal BudgetTon { get; private set; }
    public decimal? MaxPricePerPlacement { get; private set; }
    
    public CampaignStatusType Status { get; private set; }
    public Guid? CategoryId { get; private set; }
    
    public DateTimeOffset? StartsAt { get; private set; }
    public DateTimeOffset? EndsAt { get; private set; }
    public DateTimeOffset? ApplicationDeadline { get; private set; }
    
    public CampaignTargetingContract? TargetingJson { get; private set; }
    public CampaignCreativeContract? CreativeJson { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    public User Advertiser { get; private set; } = null!;
    public Category? Category { get; private set; }
    public ICollection<CampaignApplication> Applications { get; private set; } = [];
    public ICollection<Deal> Deals { get; private set; } = [];
    
    public static Campaign Create(
        Guid advertiserId,
        string title,
        string description,
        decimal budgetTon,
        Guid? categoryId = null,
        string? brief = null,
        decimal? maxPricePerPlacement = null,
        CampaignTargetingContract? targeting = null,
        CampaignCreativeContract? creative = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        DateTimeOffset? applicationDeadline = null)
    {
        return new Campaign
        {
            Id = Guid.CreateVersion7(),
            AdvertiserId = advertiserId,
            Title = title,
            Description = description,
            Brief = brief,
            BudgetTon = budgetTon,
            MaxPricePerPlacement = maxPricePerPlacement,
            Status = CampaignStatusType.Draft,
            CategoryId = categoryId,
            StartsAt = startsAt?.ToUniversalTime(),
            EndsAt = endsAt?.ToUniversalTime(),
            ApplicationDeadline = applicationDeadline?.ToUniversalTime(),
            TargetingJson = targeting,
            CreativeJson = creative,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
    
    public void Update(
        string? title = null,
        string? description = null,
        string? brief = null,
        decimal? budgetTon = null,
        decimal? maxPricePerPlacement = null,
        Guid? categoryId = null,
        CampaignTargetingContract? targeting = null,
        CampaignCreativeContract? creative = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        DateTimeOffset? applicationDeadline = null)
    {
        if (title != null) Title = title;
        if (description != null) Description = description;
        if (brief != null) Brief = brief;
        if (budgetTon.HasValue) BudgetTon = budgetTon.Value;
        if (maxPricePerPlacement.HasValue) MaxPricePerPlacement = maxPricePerPlacement;
        if (categoryId.HasValue) CategoryId = categoryId;
        if (targeting != null) TargetingJson = targeting;
        if (creative != null) CreativeJson = creative;
        if (startsAt.HasValue) StartsAt = startsAt.Value.ToUniversalTime();
        if (endsAt.HasValue) EndsAt = endsAt.Value.ToUniversalTime();
        if (applicationDeadline.HasValue) ApplicationDeadline = applicationDeadline.Value.ToUniversalTime();
        
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void UpdateStatus(CampaignStatusType status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
