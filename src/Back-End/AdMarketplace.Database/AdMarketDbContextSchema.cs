namespace AdMarketplace.Database;

public static class AdMarketDbContextSchema
{
    public const string DefaultSchema = "AdMarketplace";
    public const string DefaultConnectionStringName = "Main";
    
    public static class User
    {
        public const string TableName = "Users";
    }
    
    public static class UserTransaction
    {
        public const string TableName = "UserTransactions";
    }
    
    public static class Agent
    {
        public const string TableName = "Agents";
    }

    public static class Channel
    {
        public const string TableName = "Channels";
    }

    public static class Category
    {
        public const string TableName = "Categories";
    }

    public static class UserChannelLink
    {
        public const string TableName = "UserChannelConnections";
    }

    public static class ChannelPricing
    {
        public const string TableName = "ChannelPricings";
    }
    
    public static class Campaign
    {
        public const string TableName = "Campaigns";
    }
    
    public static class CampaignApplication
    {
        public const string TableName = "CampaignApplications";
    }
    
    public static class CampaignInvitation
    {
        public const string TableName = "CampaignInvitations";
    }
    
    public static class ChannelApplication
    {
        public const string TableName = "ChannelApplications";
    }
    
    public static class Deal
    {
        public const string TableName = "Deals";
    }
}