using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Contracts.Responses;

namespace AdMarketplace.Endpoints.Channel.MyChannels;

public class Response
{
    public required List<ChannelItem> Channels { get; set; }
}

public class ChannelItem
{
    public Guid Id { get; set; }
    public long ChatId { get; set; }
    public string Title { get; set; }
    public int AverageViews { get; set; }
}
