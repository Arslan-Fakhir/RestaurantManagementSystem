using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _ctx;

        public CustomerService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<Customer>> GetAllAsync()
        {
            try
            {
                var result= await _ctx.Customers.FromSqlRaw("CALL Customers({0},{1},{2},{3},{4},{5})", "select", 0, "null", "null", "null", "null").ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return new List<Customer>();
            }
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            try
            {
                return await _ctx.Customers.FindAsync(id);
            }
            catch (Exception)
            {
                return new Customer();
            }
        }

        public async Task<int> PostAsync(CustomerRequest customer)
        {
            try
            {
                return await _ctx.Database.ExecuteSqlRawAsync("CALL customers({0}, {1}, {2}, {3}, {4}, {5})",
                                                 "insert", 0, customer.first_name, customer.last_name, customer.contact, customer.email);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> PutAsync(CustomerRequest customer)
        {
            try
            {
                return await _ctx.Database.ExecuteSqlRawAsync("CALL customers({0},{1},{2},{3},{4},{5})",
                                                "update", customer.customer_id, customer.first_name, customer.last_name, customer.contact, customer.email);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _ctx.Database.ExecuteSqlRawAsync("CALL customers({0},{1},{2},{3},{4},{5})",
                                                "delete", id, "null", "null", "null", "null", "null");
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
