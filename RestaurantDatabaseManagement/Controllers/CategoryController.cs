using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Services.Interfaces;

namespace RestaurantDatabaseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }   
        
        [HttpGet("GetAllCategoriesOrById/{id}")]
        public async Task<IActionResult> GetCategories(int id=0)
        {
            if (id == 0)
            {
                var categories = await _service.GetAllAsync();
                if (categories.Count == 0)
                    return NotFound("No Categories Record Found.");

                return Ok(categories);
            }
            else
            {
                var category = await _service.GetByIdAsync(id);
                if(category.Count == 0)
                    return NotFound($"Category with ID {id} not found.");

                return Ok(category);
            }
        }

        [HttpPost("CreateOrUpdateCategory")]
        public async Task<IActionResult> CreateCategory(CategoryRequest category)
        {
            if (category.category_id == 0)
            {
                var result = await _service.PostAsync(category);
                if (result.Contains("not found"))
                {
                    return BadRequest(new { message = result });
                }
                return CreatedAtAction(nameof(GetCategories), new { Id = category.category_id }, category);
            }
            else
            {
                var result = await _service.PutAsync(category);
                if(result.Contains("not found"))
                {
                    return BadRequest(new { message = result });
                }
                return Ok(new { message = result });
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.Contains("not found"))
            {
                return NotFound(new { message = result });
            }
            return Ok(new { message = result });
        }
    }
}
