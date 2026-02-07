namespace AdMarketplace.Endpoints.ChannelApplication.GetByChannel;

public class Request
{
    public Guid ChannelId { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}
