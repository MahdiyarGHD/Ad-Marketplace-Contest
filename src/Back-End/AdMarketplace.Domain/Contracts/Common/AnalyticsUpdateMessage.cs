namespace AdMarketplace.Domain.Contracts.Common;

public record AnalyticsUpdateMessage(
    Guid ChannelId,
    DateTimeOffset RequestedAt);
