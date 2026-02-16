using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.ChannelApplication.MyApplications;

public class Response
{
    public required List<MyChannelApplicationItem> Applications { get; set; }
}

public class MyChannelApplicationItem
{
    public required Guid Id { get; set; }
    public required Guid ChannelId { get; set; }
    public required string ChannelTitle { get; set; }
    public string? ChannelUsername { get; set; }
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
