namespace AdMarketplace.Domain.Types;

public enum ChannelStatusType : byte
{
    UnReady = 0,
    PartiallyReady = 1,
    Ready = 2,
    Deactivated = 3,
}