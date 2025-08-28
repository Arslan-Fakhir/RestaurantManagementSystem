using Microsoft.EntityFrameworkCore;

namespace RestaurantDatabaseManagement.Models.Request
{
    [Keyless]
    public class DeleteCategoryRequest
    {
        public int category_id { get; set; }
        public string? parent_category_name { get; set; } = null;
    }
}
