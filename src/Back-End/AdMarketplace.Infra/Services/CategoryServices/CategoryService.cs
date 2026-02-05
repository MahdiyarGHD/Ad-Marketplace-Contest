using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CategoryServices;

public class CategoryService(AdMarketDbContext dbContext) : ICategoryService
{
    public async Task<ErrorOr<Category>> CreateAsync(string name, string? description, string? icon, int displayOrder)
    {
        var existingCategory = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Name == name);

        if (existingCategory is not null)
            return Error.Conflict("Category.AlreadyExists", "Category with this name already exists");

        var category = Category.Create(
            name: name,
            description: description,
            icon: icon,
            displayOrder: displayOrder);

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    public async Task<ErrorOr<Category>> GetByIdAsync(Guid id)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return Error.NotFound("Category.NotFound", "Category not found");

        return category;
    }

    public async Task<ErrorOr<List<Category>>> GetAllAsync()
    {
        var categories = await dbContext.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return categories;
    }

    public async Task<ErrorOr<Category>> UpdateAsync(Guid id, string name, string? description, string? icon, int displayOrder)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return Error.NotFound("Category.NotFound", "Category not found");

        var existingCategory = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Name == name && c.Id != id);

        if (existingCategory is not null)
            return Error.Conflict("Category.AlreadyExists", "Category with this name already exists");

        category.Update(name, description, icon, displayOrder);
        await dbContext.SaveChangesAsync();

        return category;
    }

    public async Task<ErrorOr<bool>> DeleteAsync(Guid id)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return Error.NotFound("Category.NotFound", "Category not found");

        var channelsUsingCategory = await dbContext.Channels.AnyAsync(c => c.CategoryId == id);

        if (channelsUsingCategory)
            return Error.Conflict("Category.InUse", "Category is being used by one or more channels");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();

        return true;
    }
}

