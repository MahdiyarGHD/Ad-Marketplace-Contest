using AdMarketplace.Domain.Contracts.Responses;
using AdMarketplace.Domain.Types;
using AdMarketplace.Endpoints.Category.List;

namespace AdMarketplace.Endpoints.Channel.MyChannels;

public class Response
{
    public Guid Id { get; set; }
    public long ChatId { get; set; }
    public string Title { get; set; }
    public ChannelStatusType Status { get; set; }
    public int AverageViews { get; set; }
    public CategoryResponseDto Category { get; set; }
    public List<ChannelPricingResponseContract> Pricings { get; set; } = [];
}
