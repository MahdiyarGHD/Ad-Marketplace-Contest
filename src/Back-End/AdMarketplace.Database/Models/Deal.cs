using AdMarketplace.Domain.Types;
using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class Deal : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public Guid? CampaignId { get; private set; }
    public Guid? ApplicationId { get; private set; }
    public Guid? InvitationId { get; private set; }
    public Guid? ChannelApplicationId { get; private set; }
    public Guid ChannelId { get; private set; }
    public Guid AdvertiserId { get; private set; }
    
    public decimal AmountTon { get; private set; }
    public AdFormatType AdFormat { get; private set; }
    public PriceType PriceType { get; private set; }
    
    public string? EscrowWalletAddress { get; private set; }
    public string? TransactionHash { get; private set; }
    
    public DealStatusType Status { get; private set; }
    public DateTimeOffset? ScheduledPostTime { get; private set; }
    public DateTimeOffset? ActualPostTime { get; private set; }
    public long? PostedMessageId { get; private set; }
    public string? PostedTextHash { get; private set; }
    
    public long? DraftMessageId { get; private set; }
    public DraftStatusType DraftStatus { get; private set; }
    public string? AdvertiserFeedback { get; private set; }
    
    public DateTimeOffset? LastActivityAt { get; private set; }
    public DateTimeOffset? AutoCancelAt { get; private set; }
    public DateTimeOffset? FundsReleasedAt { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    public Campaign? Campaign { get; private set; }
    public CampaignApplication? Application { get; private set; }
    public CampaignInvitation? Invitation { get; private set; }
    public ChannelApplication? ChannelApplication { get; private set; }
    public Channel Channel { get; private set; } = null!;
    public User Advertiser { get; private set; } = null!;
    
    public static Deal Create(
        Guid? campaignId,
        Guid? applicationId,
        Guid? invitationId,
        Guid? channelApplicationId,
        Guid channelId,
        Guid advertiserId,
        decimal amountTon,
        AdFormatType adFormat,
        PriceType priceType,
        DateTimeOffset? scheduledPostTime = null,
        string? escrowWalletAddress = null)
    {
        var now = DateTimeOffset.UtcNow;
        return new Deal
        {
            Id = Guid.CreateVersion7(),
            CampaignId = campaignId,
            InvitationId = invitationId,
            ApplicationId = applicationId,
            ChannelApplicationId = channelApplicationId,
            ChannelId = channelId,
            AdvertiserId = advertiserId,
            AmountTon = amountTon,
            AdFormat = adFormat,
            PriceType = priceType,
            ScheduledPostTime = scheduledPostTime,
            EscrowWalletAddress = escrowWalletAddress,
            Status = DealStatusType.AwaitingPayment,
            DraftStatus = DraftStatusType.NotSubmitted,
            LastActivityAt = now,
            AutoCancelAt = now.AddDays(7),
            CreatedAt = now
        };
    }
    
    public void UpdateStatus(DealStatusType status)
    {
        Status = status;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void FundEscrow(string transactionHash, string walletAddress)
    {
        TransactionHash = transactionHash;
        EscrowWalletAddress = walletAddress;
        Status = DealStatusType.EscrowFunded;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void SubmitDraft(long messageId)
    {
        DraftMessageId = messageId;
        DraftStatus = DraftStatusType.Submitted;
        Status = DealStatusType.DraftSubmitted;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void ApproveDraft()
    {
        DraftStatus = DraftStatusType.Approved;
        Status = ScheduledPostTime.HasValue && ScheduledPostTime.Value > DateTimeOffset.UtcNow
            ? DealStatusType.Scheduled
            : DealStatusType.DraftApproved;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void RejectDraft(string feedback)
    {
        DraftStatus = DraftStatusType.Rejected;
        Status = DealStatusType.DraftRejected;
        AdvertiserFeedback = feedback;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void MarkAsPosted(long messageId, string? textHash = null)
    {
        PostedMessageId = messageId;
        PostedTextHash = textHash;
        ActualPostTime = DateTimeOffset.UtcNow;
        Status = DealStatusType.Verifying;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void ReleaseFunds()
    {
        Status = DealStatusType.Completed;
        FundsReleasedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void Refund()
    {
        Status = DealStatusType.Refunded;
        FundsReleasedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        Status = DealStatusType.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ResolveDisputeWithRefund()
    {
        Status = DealStatusType.Refunded;
        FundsReleasedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ResolveDisputeWithRelease()
    {
        Status = DealStatusType.Completed;
        FundsReleasedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool HasFundedEscrow =>
        Status != DealStatusType.AwaitingPayment &&
        Status != DealStatusType.Cancelled &&
        TransactionHash is not null;
}
