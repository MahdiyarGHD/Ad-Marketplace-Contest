using AdMarketplace.Domain.Types;
using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class CampaignApplication : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid ChannelId { get; private set; }
    
    public AdFormatType ProposedAdFormat { get; private set; }
    public PriceType ProposedPriceType { get; private set; }
    public decimal ProposedPriceTon { get; private set; }
    public DateTimeOffset? ProposedPostingTime { get; private set; }
    public string? Message { get; private set; }
    
    public ApplicationStatusType Status { get; private set; }
    public string? RejectionReason { get; private set; }
    
    public AdFormatType? CounterAdFormat { get; private set; }
    public PriceType? CounterPriceType { get; private set; }
    public decimal? CounterPriceTon { get; private set; }
    public DateTimeOffset? CounterPostingTime { get; private set; }
    public string? CounterMessage { get; private set; }
    public Guid? LastCounterByUserId { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    public Campaign Campaign { get; private set; } = null!;
    public Channel Channel { get; private set; } = null!;
    public Deal? Deal { get; private set; }
    
    public static CampaignApplication Create(
        Guid campaignId,
        Guid channelId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null)
    {
        return new CampaignApplication
        {
            Id = Guid.CreateVersion7(),
            CampaignId = campaignId,
            ChannelId = channelId,
            ProposedAdFormat = proposedAdFormat,
            ProposedPriceType = proposedPriceType,
            ProposedPriceTon = proposedPriceTon,
            ProposedPostingTime = proposedPostingTime?.ToUniversalTime(),
            Message = message,
            Status = ApplicationStatusType.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
    
    public void Accept()
    {
        Status = ApplicationStatusType.Accepted;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void Reject(string? reason = null)
    {
        Status = ApplicationStatusType.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void Withdraw()
    {
        Status = ApplicationStatusType.Withdrawn;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void CounterOffer(
        Guid byUserId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon,
        DateTimeOffset? postingTime = null,
        string? message = null)
    {
        CounterAdFormat = adFormat;
        CounterPriceType = priceType;
        CounterPriceTon = priceTon;
        CounterPostingTime = postingTime?.ToUniversalTime();
        CounterMessage = message;
        LastCounterByUserId = byUserId;
        Status = ApplicationStatusType.CounterOffer;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AcceptCounterOffer()
    {
        if (CounterPriceTon.HasValue)
            ProposedPriceTon = CounterPriceTon.Value;
        if (CounterAdFormat.HasValue)
            ProposedAdFormat = CounterAdFormat.Value;
        if (CounterPriceType.HasValue)
            ProposedPriceType = CounterPriceType.Value;
        if (CounterPostingTime.HasValue)
            ProposedPostingTime = CounterPostingTime;

        Status = ApplicationStatusType.Accepted;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
