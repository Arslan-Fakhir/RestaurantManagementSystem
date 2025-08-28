using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RestaurantDatabaseManagement.Models;

[Index(nameof(item_name))]

public class Item
{
    [Key]
    public int item_id { get; set; }

    [Required]
    [Display(Name="Item Name")]
    [StringLength(50)]
    public string item_name { get; set; }

    [Required]
    public double price { get; set; }

}
