using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.GetByChannel;

public class Response
{
    public required List<DealItem> Deals { get; set; }
}

public class DealItem
{
    public required Guid Id { get; set; }
    public Guid? CampaignId { get; set; }
    public string? CampaignTitle { get; set; }
    public required Guid ChannelId { get; set; }
    public required string ChannelTitle { get; set; }
    public required decimal AmountTon { get; set; }
    public DealStatusType Status { get; set; }
    public DraftStatusType DraftStatus { get; set; }
    public DateTimeOffset? ScheduledPostTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
