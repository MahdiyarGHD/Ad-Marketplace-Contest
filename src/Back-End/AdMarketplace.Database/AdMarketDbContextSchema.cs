namespace AdMarketplace.Database;

public static class AdMarketDbContextSchema
{
    public const string DefaultSchema = "AdMarketplace";
    public const string DefaultConnectionStringName = "Main";
    
    public static class User
    {
        public const string TableName = "Users";
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
}