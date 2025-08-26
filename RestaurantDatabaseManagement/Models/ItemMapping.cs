using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDatabaseManagement.Models;

public class ItemMapping
{
    [Key]
    public int item_mapping_id { get; set; }

    [Required]
    public int item_id { get; set; }

    [Required]
    public int category_id { get; set; }

    [ForeignKey("item_id")]
    public Item Item { get; set; }

    [ForeignKey("category_id")]
    public Category Category { get; set; }
}
