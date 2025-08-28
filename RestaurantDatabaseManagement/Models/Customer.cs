using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RestaurantDatabaseManagement.Models;

[Index(nameof(email), IsUnique = true)]  // Unique index on Email
[Index(nameof(contact), IsUnique = true)]
[Index(nameof(first_name), nameof(last_name))]  // Composite index
public class Customer
{
    [Key]
    public int customer_id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name="First Name")]
    public string first_name { get; set; }

    [StringLength(50)]
    [Required]
    [Display(Name="Last Name")]
    public string last_name { get; set; }

    [Required]
    [EmailAddress]
    public string email { get; set; }

    [Required]
    [Phone]
    public string contact { get; set; }

    public int IsDeleted { get; set; } = 0;

}
