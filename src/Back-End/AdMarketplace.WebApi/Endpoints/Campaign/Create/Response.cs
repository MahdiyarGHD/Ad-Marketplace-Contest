using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.Create;

public class Response
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public CampaignStatusType Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
