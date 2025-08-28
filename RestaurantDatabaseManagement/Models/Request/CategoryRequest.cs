using Microsoft.EntityFrameworkCore;

namespace RestaurantDatabaseManagement.Models.Request
{
    [Keyless]
    public class CategoryRequest
    {
        public int category_id { get; set; }
        public string? category_name { get; set; }
        public bool sub_category { get; set; } = false;
        public string? parent_category_name { get; set; } = string.Empty;
    }
}
