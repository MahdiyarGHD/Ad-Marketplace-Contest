using AdMarketplace.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdMarketplace.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();
        
        await dbContext.Database.MigrateAsync();
        await SeedDataAsync(dbContext);
    }

    private static async Task SeedDataAsync(AdMarketDbContext dbContext)
    {
        await SeedCategoriesAsync(dbContext);
    }

    private static async Task SeedCategoriesAsync(AdMarketDbContext dbContext)
    {
        if (await dbContext.Categories.AnyAsync())
            return;

        var categories = CategorySeedData.GetCategories();
        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();
    }
}

