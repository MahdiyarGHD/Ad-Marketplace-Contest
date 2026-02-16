using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.ChannelApplication.GetByChannel;

public class Response
{
    public required List<ChannelApplicationItem> Applications { get; set; }
}

public class ChannelApplicationItem
{
    public required Guid Id { get; set; }
    public required Guid AdvertiserId { get; set; }
    public required string AdvertiserName { get; set; }
    public required AdFormatType ProposedAdFormat { get; set; }
    public required PriceType ProposedPriceType { get; set; }
    public required decimal ProposedPriceTon { get; set; }
    public DateTimeOffset? ProposedPostingTime { get; set; }
    public string? Message { get; set; }
    public ApplicationStatusType Status { get; set; }
    public string? RejectionReason { get; set; }
    
    // Counter offer properties
    public AdFormatType? CounterAdFormat { get; set; }
    public PriceType? CounterPriceType { get; set; }
    public decimal? CounterPriceTon { get; set; }
    public DateTimeOffset? CounterPostingTime { get; set; }
    public string? CounterMessage { get; set; }
    public Guid? LastCounterByUserId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}
