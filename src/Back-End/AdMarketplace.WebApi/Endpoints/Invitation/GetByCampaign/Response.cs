using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Invitation.GetByCampaign;

public class Response
{
    public required List<InvitationItem> Invitations { get; set; }
}

public class InvitationItem
{
    public required Guid Id { get; set; }
    public required Guid CampaignId { get; set; }
    public required ChannelInfo Channel { get; set; }
    public required AdFormatType ProposedAdFormat { get; set; }
    public required PriceType ProposedPriceType { get; set; }
    public required decimal ProposedPriceTon { get; set; }
    public DateTimeOffset? ProposedPostingTime { get; set; }
    public string? Message { get; set; }
    public InvitationStatusType Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ChannelInfo
{
    public required Guid Id { get; set; }
    public required long ChatId { get; set; }
    public required string Title { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }
    public required int SubscriberCount { get; set; }
    public required int PremiumCount { get; set; }
    public required int AverageViews { get; set; }
    public List<LanguageDistributionContract>? LanguageDistribution { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public required List<ChannelPricingInfo> Pricings { get; set; }
}

public class ChannelPricingInfo
{
    public required Guid Id { get; set; }
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public required decimal PriceTon { get; set; }
}

