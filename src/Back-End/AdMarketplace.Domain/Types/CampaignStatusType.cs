namespace AdMarketplace.Domain.Types;

public enum CampaignStatusType : byte
{
    Draft = 0,
    Active = 1,
    Paused = 2,
    Completed = 3,
    Cancelled = 4
}
