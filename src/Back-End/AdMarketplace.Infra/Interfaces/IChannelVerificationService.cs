using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IChannelVerificationService
{
    /// <summary>
    /// Verifies that the bot has required admin permissions in the channel
    /// </summary>
    Task<ErrorOr<bool>> VerifyBotAdminRightsAsync(long chatId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifies that the agent has required admin permissions in the channel
    /// </summary>
    Task<ErrorOr<bool>> VerifyAgentAdminRightsAsync(long chatId, long agentUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifies that both bot and agent have required admin permissions in the channel
    /// </summary>
    Task<ErrorOr<bool>> VerifyChannelReadinessAsync(Guid channelId, CancellationToken cancellationToken = default);
}

