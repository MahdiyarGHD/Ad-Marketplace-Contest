namespace AdMarketplace.Domain.Types;

public enum InvitationStatusType : byte
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
    Withdrawn = 3,
    CounterOffer = 4
}
