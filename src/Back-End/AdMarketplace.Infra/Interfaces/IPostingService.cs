using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IPostingService
{
    Task<ErrorOr<long>> PostToChannelAsync(long channelChatId, long fromChatId, long messageId, CancellationToken ct = default);
    Task<ErrorOr<PostVerificationResult>> VerifyPostAsync(Guid agentId, string channelUsername, long messageId, string? expectedTextHash, CancellationToken ct = default);
    Task<ErrorOr<string>> GetMessageTextHashAsync(Guid agentId, string channelUsername, long messageId, CancellationToken ct = default);
}

public record PostVerificationResult(bool Exists, bool WasEdited);
