using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;

namespace RestaurantDatabaseManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderItemRequest>().HasNoKey().ToView(null);
        modelBuilder.Entity<OrderRequest>().HasNoKey().ToView(null);
        modelBuilder.Entity<OrderResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<CategoryRequest>().HasNoKey().ToView(null);
        modelBuilder.Entity<CategoryResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<ItemRequest>().HasNoKey().ToView(null);
        modelBuilder.Entity<ItemResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<DeleteCategoryRequest>().HasNoKey().ToView(null);
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<CategoryMapping> Category_Mapping { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemMapping> Item_Mapping { get; set; }
    public DbSet<OrderItem> Order_Items { get; set; }
    public DbSet<Order> Orders { get; set; }

   
    // Optional: You can keep DbSets for requests/responses if you want to query them directly
    public DbSet<OrderRequest> OrderRequest { get; set; }
    public DbSet<OrderItemRequest> OrderItemRequest { get; set; }
    public DbSet<OrderResponse> OrderResponse { get; set; }
    public DbSet<CategoryRequest> CategoryRequest { get; set; }
    public DbSet<CategoryResponse> CategoryResponse { get; set; }
    public DbSet<ItemRequest> ItemRequest { get; set; }
    public DbSet<ItemResponse> ItemResponse { get; set; }
    public DbSet<DeleteCategoryRequest> DeleteCategoryRequest { get; set; }
}
