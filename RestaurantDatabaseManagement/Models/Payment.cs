using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDatabaseManagement.Models;

public class Payment
{
    [Key]
    public int payment_id { get; set; }

    [Required]
    public int order_id { get; set; }

    [Required]
    public double amount { get; set; }

    [ForeignKey("order_id")]
    public Order Order { get; set; }
}
