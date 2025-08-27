using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;

namespace RestaurantDatabaseManagement.Services.Interfaces
{
    public interface IItemService
    {
        Task<string?> DeleteAsync(int id);
        Task<List<ItemResponse>> GetAllAsync();
        Task<List<ItemResponse>> GetByIdAsync(int id);
        Task<string?> PostAsync(ItemRequest item);
        Task<string?> PutAsync(ItemRequest item);
    }
}