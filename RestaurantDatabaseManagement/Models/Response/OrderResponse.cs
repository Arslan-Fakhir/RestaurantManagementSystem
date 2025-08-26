using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RestaurantDatabaseManagement.Models.Request
{
    [Keyless]
    public class OrderResponse
    {
        public int order_id { get; set; }

        public int customer_id { get; set; }
        public string customer_name { get; set; }
        public string customer_contact { get; set; }
        public string customer_email { get; set; }
        public string Order_Items { get; set; }
        public int Ordered_Quantity { get; set; }
    }
}
