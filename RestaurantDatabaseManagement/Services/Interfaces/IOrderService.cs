using RestaurantDatabaseManagement.Models.Request;

namespace RestaurantDatabaseManagement.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> DeleteAsync(int id);
        Task<List<OrderResponse>> GetAllAsync();
        Task<List<OrderResponse>> GetByIdAsync(int id);
        Task<string> PostAsync(OrderRequest order);
        Task<string> PutAsync(OrderRequest order);
    }
}