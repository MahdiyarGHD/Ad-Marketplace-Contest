using AdMarketplace.Database.Models;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface ICategoryService
{
    Task<ErrorOr<Category>> CreateAsync(string name, string? description, string? icon, int displayOrder);
    Task<ErrorOr<Category>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<Category>>> GetAllAsync();
    Task<ErrorOr<Category>> UpdateAsync(Guid id, string name, string? description, string? icon, int displayOrder);
    Task<ErrorOr<bool>> DeleteAsync(Guid id);
}

