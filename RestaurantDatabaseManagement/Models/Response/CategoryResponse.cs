using Microsoft.EntityFrameworkCore;

namespace RestaurantDatabaseManagement.Models.Response
{
    [Keyless]
    public class CategoryResponse
    {
        public int category_id { get; set; }
        public string category_name { get; set; }
        public string? parent_category { get; set; }
    }
}
