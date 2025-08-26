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
            return await _ctx.Customers.FromSqlRaw("CALL Customers({0},{1},{2},{3},{4},{5})","select",0,"null","null","null","null").ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _ctx.Customers.FindAsync(id);
        }

        public async Task<int> PostAsync(Customer customer)
        {
            return await _ctx.Database.ExecuteSqlRawAsync("CALL customers({0}, {1}, {2}, {3}, {4}, {5})",
                                             "insert", 0, customer.first_name, customer.last_name, customer.contact, customer.email);
        }

        public async Task<int> PutAsync(Customer customer)
        {

            return await _ctx.Database.ExecuteSqlRawAsync("CALL customers({0},{1},{2},{3},{4},{5})", 
                                            "update", customer.customer_id, customer.first_name, customer.last_name, customer.contact, customer.email);



            /*var existingCustomer = await _ctx.Customers.FindAsync(id);

            if (existingCustomer == null)
                return existingCustomer;

            existingCustomer.customer_id = id;
            if (!string.IsNullOrEmpty(customer.first_name))
                existingCustomer.first_name = customer.first_name;
            if (!string.IsNullOrEmpty(customer.last_name))
                existingCustomer.last_name = customer.last_name;
            if (!string.IsNullOrEmpty(customer.email))
                existingCustomer.email = customer.email;
            if (!string.IsNullOrEmpty(customer.contact))
                existingCustomer.contact = customer.contact;
            await _ctx.SaveChangesAsync();

            return existingCustomer;*/
        }

        public async Task<int> DeleteAsync(int id)
        {
            var result = await _ctx.Database.ExecuteSqlRawAsync("CALL customers({0},{1},{2},{3},{4},{5})",
                                            "delete", id, "null", "null", "null", "null", "null");

            return result;

            /*var affected = await _ctx.Customers
                .Where(e => e.customer_id == id)
                .ExecuteDeleteAsync();

            return affected;*/
        }

    }
}
