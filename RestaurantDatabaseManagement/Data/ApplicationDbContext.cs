using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;
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
        modelBuilder.Entity<CategoryRequest>().HasNoKey();    
        modelBuilder.Entity<CategoryResponse>().HasNoKey();
        modelBuilder.Entity<ItemRequest>().HasNoKey();
        modelBuilder.Entity<ItemResponse>().HasNoKey();
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<CategoryMapping> Category_Mapping { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemMapping> Item_Mapping { get; set; }
    public DbSet<OrderItem> Order_Items { get; set; }
    public DbSet<Order> Orders { get; set; }


    //   Not Mapped DbSet for Request and Response Models

    [NotMapped]
    public DbSet<OrderRequest> OrderRequest { get; set; }
    [NotMapped]
    public DbSet<OrderItemRequest> OrderItemRequest { get; set; }

    [NotMapped]
    public DbSet<OrderResponse> OrderResponse { get; set; }
    [NotMapped]
    public DbSet<CategoryRequest> CategoryRequest { get; set; }
    [NotMapped]
    public DbSet<CategoryResponse> CategoryResponse { get; set; }
    [NotMapped]
    public DbSet<ItemRequest> ItemRequest { get; set; }
    [NotMapped]
    public DbSet<ItemResponse> ItemResponse { get; set; }
    [NotMapped]
    public DbSet<DeleteCategoryRequest> DeleteCategoryRequest { get; set; }

}
