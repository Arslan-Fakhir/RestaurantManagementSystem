using Hangfire;
using Hangfire.MySql;
// ✅ Correct package for MySqlConnector
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Controllers;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Jobs;
using RestaurantDatabaseManagement.Services.Implementations;
using RestaurantDatabaseManagement.Services.Interfaces;
using Stripe;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerService, RestaurantDatabaseManagement.Services.Implementations.CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<StripePaymentIntentsService>();

builder.Services.AddScoped<EmailService>(); //hangfire jobs service logic
builder.Services.AddTransient<PendingPaymentsJob>(); //hangfire job calls

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//////////////////////////////////////////////////////////
/*                      Quartz.NET                      */

builder.Services.AddInfrastructure();

//////////////////////////////////////////////////////////

// ✅ Register EF Core with MySQL (using MySqlConnector)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);
////////////////////////////////////////////////////////
/*                      Hangfire                      */

/*var options = new MySqlStorageOptions // configure MySQL storage options
{
    TransactionIsolationLevel = (System.Transactions.IsolationLevel?)IsolationLevel.ReadCommitted,
    QueuePollInterval = TimeSpan.FromSeconds(15),
    JobExpirationCheckInterval = TimeSpan.FromHours(1),
    CountersAggregateInterval = TimeSpan.FromMinutes(5),
    PrepareSchemaIfNecessary = true,
    DashboardJobListLimit = 50000
};

var hangfireConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHangfire(config =>
    config.UseStorage(new MySqlStorage(hangfireConnectionString, options))); // Use MySqlStorage for Hangfire

builder.Services.AddHangfireServer(); // Add Hangfire background job server
*/

////////////////////////////////////////////////////////

// Stripe configuration
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//////////////////////////////////////////////////////////
/*          Hangfire dashboard with basic auth          */
// ✅ 
/*app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Hangfire Job Restaurant Application",
    DarkModeEnabled = true,
    DisplayStorageConnectionString = true,
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = builder.Configuration["HangfireSettings:User"],
            Pass = builder.Configuration["HangfireSettings:Pass"],
        }
    }
});*/

//////////////////////////////////////////////////////////
app.MapControllers();

app.Run();
