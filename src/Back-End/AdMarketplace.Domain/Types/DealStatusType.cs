namespace AdMarketplace.Domain.Types;

public enum DealStatusType
{
    AwaitingPayment = 0,
    EscrowFunded = 1,
    DraftSubmitted = 2,
    DraftRejected = 3,
    DraftApproved = 4,
    Scheduled = 5,
    Posted = 6,
    Verifying = 7,
    Completed = 8,
    Refunded = 9,
    Cancelled = 10,
    Disputed = 11
}
