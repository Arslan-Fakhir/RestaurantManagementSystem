using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Services.Interfaces;

namespace RestaurantDatabaseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController( ICustomerService service)
        {
            _service = service;
        }


        [HttpGet("GetAllCustomersOrById/{id}")]
        public async Task<IActionResult> GetCustomer(int id = 0)
        {
            if (id == 0)
            {
                try
                {
                    var customers = await _service.GetAllAsync();

                    if (customers == null || customers.Count == 0)
                        return NotFound("No Customer Found.");

                    return Ok(customers);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return Ok();
            }
            else
            {
                var customer = await _service.GetByIdAsync(id);
                if (customer == null)
                    return NotFound($"Customer with ID {id} not found.");

                return Ok(customer);
            }
        }

        [HttpPost("CreateOrUpdateCustomer")]
        public async Task<IActionResult> CreateOrUpdateCustomer(Customer customer)
        {
            if (customer.customer_id == 0)
            {
                var result = await _service.PostAsync(customer);

                if(result != 1)
                    return BadRequest("Failed to create customer.");

                return CreatedAtAction(nameof(GetCustomer), new { Id = customer.customer_id }, customer);
            }
            else
            {
                var result = await _service.PutAsync(customer);
                if (result == 0)
                    NotFound($"Customer with ID {customer.customer_id} not found.");

                return Ok(customer);
            }
        }

        [HttpDelete("DeleteCustomer{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var affected = await _service.DeleteAsync(id);

            if (affected == 0)
                return NotFound($"Customer with ID {id} not found.");

            return NoContent();
        }
    }
}
