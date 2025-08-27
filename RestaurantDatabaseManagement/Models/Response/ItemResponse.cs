using Microsoft.EntityFrameworkCore;

namespace RestaurantDatabaseManagement.Models.Response
{
    [Keyless]
    public class ItemResponse
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public double price { get; set; }
        public string? parent_category { get; set; }
    }
}
