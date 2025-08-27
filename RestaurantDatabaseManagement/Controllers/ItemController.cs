using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Services.Implementations;
using RestaurantDatabaseManagement.Services.Interfaces;

namespace RestaurantDatabaseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _service;
        public ItemController(IItemService service)
        {
            _service = service;
        }
        [HttpGet("GetAllItemsOrById/{id}")]
        public async Task<IActionResult> GetItems(int id = 0)
        {
            if (id == 0)
            {
                var items = await _service.GetAllAsync();

                if (items.Count == 0)
                    return NotFound("No Items Record Found.");
                return Ok(items);
            }
            else
            {
                var item = await _service.GetByIdAsync(id);
                if (item.Count == 0)
                    return NotFound($"Item with ID {id} not found.");

                return Ok(item);
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostItem(ItemRequest item)
        {
            if(item.item_id==0)
            {
                var result = await _service.PostAsync(item);
                if(result==null)
                    return NotFound(new {message = result});
                return Ok(result);
            }
            else
            {
                var result=await _service.PutAsync(item);
                if(result==null)
                    return NotFound(new { message = result });
                return Ok(result);  
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _service.DeleteAsync(id);
            if(result==null)
                return NotFound(new { message = result });
            return Ok(new { message = result });
        }
    }
}
