using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.FundEscrow;

public class Response
{
    public required Guid Id { get; set; }
    public DealStatusType Status { get; set; }
    public required string TransactionHash { get; set; }
    public required string EscrowWalletAddress { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
