using Microsoft.EntityFrameworkCore;

namespace RestaurantDatabaseManagement.Models.Request
{
    [Keyless]
    public class ItemRequest
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public double price { get; set; }
        public bool hasParent { get; set; }
        public string parent_category_name { get; set; }
        public int offset { get; set; }
        public int fetch { get; set; }

    }
}
