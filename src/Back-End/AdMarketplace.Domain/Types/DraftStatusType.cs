namespace AdMarketplace.Domain.Types;

public enum DraftStatusType : byte
{
    NotSubmitted = 0,
    Submitted = 1,
    UnderReview = 2,
    ChangesRequested = 3,
    Approved = 4,
    Rejected = 5
}
