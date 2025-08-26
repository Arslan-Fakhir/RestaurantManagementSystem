using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RestaurantDatabaseManagement.Models;

[Index(nameof(category_name), IsUnique = true)]
public class Category
{
    [Key]
    public int category_id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name="Category Name")]
    public string category_name { get; set; }
}
