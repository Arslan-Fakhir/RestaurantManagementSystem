using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDatabaseManagement.Models;

[Index(nameof(customer_name))]
[Index(nameof(customer_email))]
[Index(nameof(customer_contact))]
public class Order
{
    [Key]
    public int order_id { get; set; }
    [Required]
    public int customer_id { get; set; }
    [Required,StringLength(100)]
    public string customer_name { get; set; }
    [Required,Phone]
    public string customer_contact { get; set; }
    [Required,EmailAddress]
    public string customer_email { get; set; }
    [Required]
    public string order_number { get; set; }

    [ForeignKey("customer_id")]
    public Customer Customer { get; set; } // Navigation property to Customer
}
