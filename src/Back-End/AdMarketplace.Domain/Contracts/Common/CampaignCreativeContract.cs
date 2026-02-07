namespace AdMarketplace.Domain.Contracts.Common;

public class CampaignCreativeContract
{
    public List<CallToActionButton>? CallToActionButtons { get; set; }
    public bool RequiresApproval { get; set; }
}

public class CallToActionButton
{
    public required string Text { get; set; }
    public required string Url { get; set; }
}
