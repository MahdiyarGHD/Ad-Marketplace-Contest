using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.GetById;

public class Response
{
    public required Guid Id { get; set; }
    public Guid? CampaignId { get; set; }
    public string? CampaignTitle { get; set; }
    public required Guid ChannelId { get; set; }
    public required string ChannelTitle { get; set; }
    public required Guid AdvertiserId { get; set; }
    public required decimal AmountTon { get; set; }
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public DealStatusType Status { get; set; }
    public DraftStatusType DraftStatus { get; set; }
    public string? AdvertiserFeedback { get; set; }
    public DateTimeOffset? ScheduledPostTime { get; set; }
    public DateTimeOffset? ActualPostTime { get; set; }
    public long? PostedMessageId { get; set; }
    public long? DraftMessageId { get; set; }
    public string? EscrowWalletAddress { get; set; }
    public string? TransactionHash { get; set; }
    public DateTimeOffset? AutoCancelAt { get; set; }
    public DateTimeOffset? FundsReleasedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
