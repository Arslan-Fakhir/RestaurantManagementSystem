using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDatabaseManagement.Data;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) 
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderItemRequest>().HasNoKey();
        modelBuilder.Entity<OrderResponse>().HasNoKey();
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<CategoryMapping> Category_Mapping { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemMapping> Item_Mapping { get; set; }
    public DbSet<OrderItem> Order_Items { get; set; }
    public DbSet<Order> Orders { get; set; }

    [NotMapped]
    public DbSet<OrderRequest> OrderRequest { get; set; }
    [NotMapped]
    public DbSet<OrderItemRequest> OrderItemRequest { get; set; }

    [NotMapped]
    public DbSet<OrderResponse> OrderResponse { get; set; }
}
