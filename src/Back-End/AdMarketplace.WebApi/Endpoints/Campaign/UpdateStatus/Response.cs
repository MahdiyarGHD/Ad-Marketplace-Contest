using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.UpdateStatus;

public class Response
{
    public required Guid Id { get; set; }
    public CampaignStatusType Status { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
