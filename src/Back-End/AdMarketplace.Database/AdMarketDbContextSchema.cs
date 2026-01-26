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

}