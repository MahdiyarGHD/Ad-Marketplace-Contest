using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.Create;

public class Response
{
    public required Guid Id { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid? ApplicationId { get; set; }
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public required decimal AmountTon { get; set; }
    public DealStatusType Status { get; set; }
    public string? EscrowWalletAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
