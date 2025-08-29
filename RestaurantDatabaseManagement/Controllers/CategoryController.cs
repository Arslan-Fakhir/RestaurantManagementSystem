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
                if (result.Contains("not found") || result.Contains("Failed") || result.Contains("Already exist"))
                {
                    return BadRequest(new { message = result });
                }
                return Ok(new { message = result });
            }
            else
            {
                var result = await _service.PutAsync(category);
                if(result.Contains("not found") || result.Contains("Cannot Update") || result.Contains("Cannot be child") || result.Contains("Nothing to update"))
                {
                    return BadRequest(new { message = result });
                }
                return Ok(new { message = result });
            }
        }

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(DeleteCategoryRequest category)
        {
            
            var result = await _service.DeleteAsync(category);

            if (result.Contains("not found") )
            {
                return NotFound(new { message = result });
            }
            else if (result.Contains("Cannot delete"))
            {
                return Conflict(new { message= result});
            }
            else if (result.Contains("Error"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });

        }
    }
}
