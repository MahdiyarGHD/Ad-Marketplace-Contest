using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.ChannelApplication.Apply;

public class Response
{
    public required Guid Id { get; set; }
    public required Guid ChannelId { get; set; }
    public required Guid AdvertiserId { get; set; }
    public required AdFormatType ProposedAdFormat { get; set; }
    public required PriceType ProposedPriceType { get; set; }
    public required decimal ProposedPriceTon { get; set; }
    public ApplicationStatusType Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
