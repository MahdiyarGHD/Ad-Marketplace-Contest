using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class CampaignInvitationService(AdMarketDbContext dbContext) : ICampaignInvitationService
{
    public async Task<ErrorOr<CampaignInvitation>> CreateAsync(
        Guid campaignId,
        Guid channelId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null)
    {
        var campaign = await dbContext.Campaigns.FindAsync(campaignId);
        if (campaign is null)
            return Error.NotFound("Campaign.NotFound", "Campaign not found");

        if (campaign.Status != CampaignStatusType.Active)
            return Error.Validation("Campaign.NotActive", "Campaign is not active");

        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.OwnerId == campaign.AdvertiserId)
            return Error.Validation("Invitation.SelfDeal", "You cannot invite your own channel to your own campaign");

        if (proposedPriceTon <= 0)
            return Error.Validation("Invitation.InvalidPrice", "Proposed price must be greater than zero");

        if (campaign.MaxPricePerPlacement.HasValue && proposedPriceTon > campaign.MaxPricePerPlacement.Value)
            return Error.Validation("Invitation.PriceExceedsMax", "Proposed price exceeds the campaign's maximum price per placement");

        if (proposedPostingTime.HasValue && proposedPostingTime.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("Invitation.InvalidPostingTime", "Proposed posting time must be in the future");

        var existingInvitation = await dbContext.CampaignInvitations
            .FirstOrDefaultAsync(i => i.CampaignId == campaignId && i.ChannelId == channelId &&
                                      i.Status != InvitationStatusType.Rejected &&
                                      i.Status != InvitationStatusType.Withdrawn);

        if (existingInvitation is not null)
            return Error.Conflict("Invitation.AlreadyExists", "You have already invited this channel to this campaign");

        var invitation = CampaignInvitation.Create(
            campaignId: campaignId,
            channelId: channelId,
            proposedAdFormat: proposedAdFormat,
            proposedPriceType: proposedPriceType,
            proposedPriceTon: proposedPriceTon,
            proposedPostingTime: proposedPostingTime,
            message: message);

        dbContext.CampaignInvitations.Add(invitation);
        await dbContext.SaveChangesAsync();

        return invitation;
    }

    public async Task<ErrorOr<CampaignInvitation>> GetByIdAsync(Guid id)
    {
        var invitation = await dbContext.CampaignInvitations
            .Include(i => i.Campaign)
            .Include(i => i.Channel)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invitation is null)
            return Error.NotFound("Invitation.NotFound", "Invitation not found");

        return invitation;
    }

    public async Task<ErrorOr<List<CampaignInvitation>>> GetByCampaignIdAsync(Guid campaignId, int skip, int take)
    {
        var invitations = await dbContext.CampaignInvitations
            .Include(i => i.Campaign)
            .Include(i => i.Channel)
            .Where(i => i.CampaignId == campaignId)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return invitations;
    }

    public async Task<ErrorOr<List<CampaignInvitation>>> GetByChannelIdAsync(Guid channelId, int skip, int take)
    {
        var invitations = await dbContext.CampaignInvitations
            .Include(i => i.Campaign)
            .Include(i => i.Channel)
            .Where(i => i.ChannelId == channelId)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return invitations;
    }

    public async Task<ErrorOr<List<CampaignInvitation>>> GetByStatusAsync(
        Guid channelId,
        InvitationStatusType status,
        int skip,
        int take)
    {
        var invitations = await dbContext.CampaignInvitations
            .Include(i => i.Campaign)
            .Include(i => i.Channel)
            .Where(i => i.ChannelId == channelId && i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return invitations;
    }

    public async Task<ErrorOr<CampaignInvitation>> AcceptAsync(Guid id, Guid channelOwnerId)
    {
        var invitation = await dbContext.CampaignInvitations
            .AsTracking()
            .Include(i => i.Campaign)
            .Include(i => i.Channel)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invitation is null)
            return Error.NotFound("Invitation.NotFound", "Invitation not found");

        if (invitation.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("Invitation.Forbidden", "You don't have permission to accept this invitation");

        if (invitation.Status != InvitationStatusType.Pending)
            return Error.Validation("Invitation.InvalidStatus", "Invitation is not pending");

        if (invitation.Campaign.Status != CampaignStatusType.Active)
            return Error.Validation("Campaign.NotActive", "Campaign is no longer active");

        invitation.Accept();
        await dbContext.SaveChangesAsync();

        return invitation;
    }

    public async Task<ErrorOr<CampaignInvitation>> RejectAsync(Guid id, Guid channelOwnerId, string? reason = null)
    {
        var invitation = await dbContext.CampaignInvitations
            .AsTracking()
            .Include(i => i.Channel)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invitation is null)
            return Error.NotFound("Invitation.NotFound", "Invitation not found");

        if (invitation.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("Invitation.Forbidden", "You don't have permission to reject this invitation");

        if (invitation.Status != InvitationStatusType.Pending)
            return Error.Validation("Invitation.InvalidStatus", "Invitation is not pending");

        invitation.Reject(reason);
        await dbContext.SaveChangesAsync();

        return invitation;
    }

    public async Task<ErrorOr<CampaignInvitation>> WithdrawAsync(Guid id, Guid advertiserId)
    {
        var invitation = await dbContext.CampaignInvitations
            .AsTracking()
            .Include(i => i.Campaign)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invitation is null)
            return Error.NotFound("Invitation.NotFound", "Invitation not found");

        if (invitation.Campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("Invitation.Forbidden", "You don't have permission to withdraw this invitation");

        if (invitation.Status != InvitationStatusType.Pending)
            return Error.Validation("Invitation.InvalidStatus", "Invitation is not pending");

        invitation.Withdraw();
        await dbContext.SaveChangesAsync();

        return invitation;
    }
}
