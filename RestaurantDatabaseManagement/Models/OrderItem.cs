using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDatabaseManagement.Models;

public class OrderItem
{
    [Key]
    public int order_item_id { get; set; }

    [Required]
    public int order_id { get; set; }

    [Required]
    public int item_id { get; set; }

    [Required]
    public int quantity { get; set; }

    [ForeignKey("order_id")]
    public Order Order { get; set; }

    [ForeignKey("item_id")]
    public Item Item { get; set; }
}
