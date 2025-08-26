using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDatabaseManagement.Models;

public class CategoryMapping
{
    [Key]
    public int category_mapping_id { get; set; }

    [Required]
    public int parent_category_id { get; set; }

    [Required]
    public int child_category_id { get; set; }

    [ForeignKey("parent_category_id")]
    public Category ParentCategory { get; set; }

    [ForeignKey("child_category_id")]
    public Category ChildCategory { get; set; }
}
