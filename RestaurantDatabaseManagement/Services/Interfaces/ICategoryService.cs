using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;

namespace RestaurantDatabaseManagement.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<string> DeleteAsync(DeleteCategoryRequest category);
        Task<List<CategoryResponse>> GetAllAsync();
        Task<List<CategoryResponse>> GetByIdAsync(int id);
        Task<string> PostAsync(CategoryRequest category);
        Task<string> PutAsync(CategoryRequest category);
    }
}