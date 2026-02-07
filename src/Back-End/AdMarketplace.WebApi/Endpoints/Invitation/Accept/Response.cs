using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Invitation.Accept;

public class Response
{
    public required Guid Id { get; set; }
    public InvitationStatusType Status { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
