using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantDatabaseManagement.Models.Request
{
    [Keyless]
    public class OrderRequest
    {
        public int order_id { get; set; }

        [Required, StringLength(100)]
        public string customer_name { get; set; }

        [Required, Phone]
        public string contact { get; set; }

        [Required, EmailAddress]
        public string email { get; set; }

        [Required]
        public List<OrderItemRequest> Items { get; set; }
    }
    [Keyless]
    public class OrderItemRequest
    {
        [Required, StringLength(50)]
        public string item_name { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}
