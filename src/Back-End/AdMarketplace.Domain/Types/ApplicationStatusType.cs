namespace AdMarketplace.Domain.Types;

public enum ApplicationStatusType : byte
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
    Withdrawn = 3,
    CounterOffer = 4
}
