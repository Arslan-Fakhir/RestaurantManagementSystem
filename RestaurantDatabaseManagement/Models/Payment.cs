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
    public int payment_status { get; set; } = 0;

    public string? transaction_id { get; set; }

    [ForeignKey("order_id")]
    public Order Order { get; set; }
}
