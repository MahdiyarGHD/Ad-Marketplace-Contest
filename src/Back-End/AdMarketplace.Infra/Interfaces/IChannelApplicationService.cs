using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IChannelApplicationService
{
    Task<ErrorOr<ChannelApplication>> CreateAsync(
        Guid channelId,
        Guid advertiserId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null);

    Task<ErrorOr<ChannelApplication>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<ChannelApplication>>> GetByChannelIdAsync(Guid channelId, int skip, int take);
    Task<ErrorOr<List<ChannelApplication>>> GetByAdvertiserIdAsync(Guid advertiserId, int skip, int take);
    Task<ErrorOr<List<ChannelApplication>>> GetByStatusAsync(
        Guid channelId,
        ApplicationStatusType status,
        int skip,
        int take);

    Task<ErrorOr<ChannelApplication>> AcceptAsync(Guid id, Guid channelOwnerId);
    Task<ErrorOr<ChannelApplication>> RejectAsync(Guid id, Guid channelOwnerId, string? reason = null);
    Task<ErrorOr<ChannelApplication>> WithdrawAsync(Guid id, Guid advertiserId);
    Task<ErrorOr<ChannelApplication>> CounterOfferAsync(
        Guid id,
        Guid userId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon,
        DateTimeOffset? postingTime = null,
        string? message = null);
    Task<ErrorOr<ChannelApplication>> AcceptCounterOfferAsync(Guid id, Guid userId);
}
