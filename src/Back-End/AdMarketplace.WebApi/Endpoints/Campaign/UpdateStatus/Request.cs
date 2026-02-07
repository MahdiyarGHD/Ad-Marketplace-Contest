using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.UpdateStatus;

public class Request
{
    public Guid Id { get; set; }
    public required CampaignStatusType Status { get; set; }
}
