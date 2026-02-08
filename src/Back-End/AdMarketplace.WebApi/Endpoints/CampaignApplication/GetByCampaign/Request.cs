namespace AdMarketplace.Endpoints.CampaignApplication.GetByCampaign;

public class Request
{
    public Guid CampaignId { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}
