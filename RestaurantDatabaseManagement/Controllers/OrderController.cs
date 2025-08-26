using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Services.Interfaces;

namespace RestaurantDatabaseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }


        [HttpGet("GetOrders/{id}")]
        public async Task<IActionResult> GetOrders(int id=0)
        {
            if (id == 0)
            {
                var orders =await _service.GetAllAsync();

                if(orders.Count == 0)
                    return NotFound("No Orders Record Found.");

                return Ok(orders);
            }
            else
            {
                var order = await _service.GetByIdAsync(id);

                if (order.Count == 0)
                    return NotFound($"Order with ID {id} not found.");

                return Ok(order);
            }
        }

        [HttpPost("CreateOrUpdateOrder")]
        public async Task<IActionResult> createorupdateorder(OrderRequest order)
        {
            if (order.order_id == 0)
            {
                var result = await _service.PostAsync(order);
                if (result.Contains("not found"))
                {
                    return BadRequest(new { message = result });
                }

                return CreatedAtAction(nameof(GetOrders), new { Id = order.order_id }, order);
            }
            else
            {
                var result = await _service.PutAsync(order);

                if (result == null)
                    NotFound($"order with id {order.order_id} not found.");

                return Ok(order);
            }
        }

        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result == 0)
                return NotFound($"Order with ID {id} not found.");
            return NoContent();
        }
    }
}
