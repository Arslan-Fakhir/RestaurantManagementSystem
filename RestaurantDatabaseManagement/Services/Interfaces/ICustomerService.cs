using RestaurantDatabaseManagement.Models;

namespace RestaurantDatabaseManagement.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<int> DeleteAsync(int id);
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<int> PostAsync(CustomerRequest customer);
        Task<int> PutAsync(CustomerRequest customer);
    }
}