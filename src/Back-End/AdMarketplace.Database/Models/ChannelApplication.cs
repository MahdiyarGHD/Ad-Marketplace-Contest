using AdMarketplace.Domain.Types;
using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class ChannelApplication : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public Guid AdvertiserId { get; private set; }
    
    public AdFormatType ProposedAdFormat { get; private set; }
    public PriceType ProposedPriceType { get; private set; }
    public decimal ProposedPriceTon { get; private set; }
    public DateTimeOffset? ProposedPostingTime { get; private set; }
    public string? Message { get; private set; }
    
    public ApplicationStatusType Status { get; private set; }
    public string? RejectionReason { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    public Channel Channel { get; private set; } = null!;
    public User Advertiser { get; private set; } = null!;
    public Deal? Deal { get; private set; }
    
    public static ChannelApplication Create(
        Guid channelId,
        Guid advertiserId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null)
    {
        return new ChannelApplication
        {
            Id = Guid.CreateVersion7(),
            ChannelId = channelId,
            AdvertiserId = advertiserId,
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
}
