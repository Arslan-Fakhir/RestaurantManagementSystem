using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RestaurantDatabaseManagement.Models;
[Keyless]
public class CustomerRequest
{
    [Key]
    public int customer_id { get; set; }


    public string first_name { get; set; }

    public string last_name { get; set; }

    public string email { get; set; }

    public string contact { get; set; }

}
