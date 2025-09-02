using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Services.Interfaces;
using Stripe;
using System.Diagnostics;
using System.Linq;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly StripePaymentIntentsService _stripeService;
        public OrderService(ApplicationDbContext ctx, StripePaymentIntentsService stripeService)
        {
            _ctx = ctx;
            _stripeService = stripeService;
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            try
            {
                var result = await _ctx.OrderResponse.FromSqlRaw("Call orders({0},{1},{2},{3},{4},{5},{6},{7})",
                                                        "select", 0, 0, null, null, null, null, 0).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return new List<OrderResponse>();
            }
        }

        public async Task<List<OrderResponse>> GetByIdAsync(int id)
        {
            try
            {
                return await _ctx.OrderResponse.FromSqlRaw("Call orders({0},{1},{2},{3},{4},{5},{6},{7})",
                                                            "select", id, 0, "null", "null", "null", "null", 0).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<OrderResponse>();
            }
        }
        private async Task<bool> CheckItemExist(List<OrderItemRequest> items) // Check if items exist
        {
            foreach (var item in items)
            {
                var existingItem = await _ctx.Items
                    .Where(c => c.item_name == item.item_name)
                    .FirstOrDefaultAsync();

                if (existingItem == null || item.quantity == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static string GenerateOrderId()
        {
            // Timestamp part (e.g. 20250902-153045)
            string date = DateTime.UtcNow.ToString("yyyyMMdd");
            string time = DateTime.UtcNow.ToString("HHmmss");

            // Generate a GUID and convert to bytes
            byte[] guidBytes = Guid.NewGuid().ToByteArray();

            // Convert first 4 bytes into an integer
            int numericPart = BitConverter.ToInt32(guidBytes, 0);

            // Ensure it's positive and take last 4 digits
            numericPart = Math.Abs(numericPart) % 10000;

            // Format to always have 4 digits (e.g. 0042 instead of 42)
            string uniquePart = numericPart.ToString("D4");

            // Combine
            string orderId = $"{date}-{time}-{uniquePart}";

            return orderId;
        }
        public async Task<string> PostAsync(OrderRequest order)
        {
            try
            {

                bool isValidItems = await CheckItemExist(order.Items);

                if (isValidItems == false)
                {
                    return "Either one or more items not found or item_quantity is zero";
                }
                else
                {
                    string transactionId = await _stripeService.CreatePayment(order);
                    if (transactionId.Contains("failed"))
                    {
                        return "Payment failed!";
                    }
                    else // if payment is successful then create order
                    {
                        double totalBillAmount = 0; 

                        var customer = await _ctx.Customers
                        .Where(c => c.email == order.email && c.IsDeleted != 1)
                        .FirstOrDefaultAsync();

                        if (customer == null)
                        {
                            var fullname = order.customer_full_name.Split(" ");

                            await _ctx.Database.ExecuteSqlRawAsync("Call customers({0},{1},{2},{3},{4},{5})",
                                "insert", 0, fullname[0], fullname.Length > 1 ? fullname[1] : " ", order.contact, order.email);

                            customer = await _ctx.Customers
                                .Where(c => c.email == order.email && c.IsDeleted != 1)
                                .FirstOrDefaultAsync();
                        }
                        string orderNumber = GenerateOrderId(); 
                        var newOrder = new Order
                        {
                            customer_id = customer.customer_id,
                            customer_name = order.customer_full_name,
                            customer_contact = order.contact,
                            customer_email = order.email,
                            order_number=orderNumber,
                        };

                        _ctx.Orders.Add(newOrder);
                        await _ctx.SaveChangesAsync();

                        foreach (var item in order.Items)
                        {
                            var existingItem = await _ctx.Items.Where(i => i.item_name == item.item_name).FirstOrDefaultAsync();

                            var newOrderItems = new OrderItem
                            {
                                order_id = newOrder.order_id,
                                item_id = existingItem.item_id,
                                quantity = item.quantity
                            };

                            _ctx.Order_Items.Add(newOrderItems);
                            await _ctx.SaveChangesAsync();

                            totalBillAmount += (existingItem.price * item.quantity);
                        }

                        var newPayment = new Payment
                        {
                            order_id = newOrder.order_id,
                            amount = totalBillAmount,
                            payment_status = 1,
                            transaction_id = transactionId
                        };

                        _ctx.Payments.Add(newPayment);
                        await _ctx.SaveChangesAsync();

                        return "Transaction succeeded and order created";
                    }
                }

            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> PutAsync(OrderRequest order)
        {
            try
            {
                var oldOrder = await _ctx.Orders   // <---------------------     I changed datatype from Order to var here
                    .Where(o => o.order_id == order.order_id)
                    .FirstOrDefaultAsync();

                if (oldOrder == null)
                {
                    return "Order record not found.";
                }

                oldOrder.customer_name = order.customer_full_name;
                oldOrder.customer_email = order.email;
                oldOrder.customer_contact = order.contact;

                _ctx.Orders.Update(oldOrder);
                await _ctx.SaveChangesAsync();

                List<OrderItem> order_items = await _ctx.Order_Items.Where(oi => oi.order_id == oldOrder.order_id).ToListAsync();

                foreach (var item in order_items)
                {
                    _ctx.Order_Items.Remove(item);
                    await _ctx.SaveChangesAsync();
                }
                double totalBillAmount = 0;

                foreach (var item in order.Items)
                {
                    var existingItem = await _ctx.Items.Where(i => i.item_name == item.item_name).FirstOrDefaultAsync();

                    if (existingItem.item_id == 0)
                        return $"Item '{item.item_name}' not found.";

                    //await _ctx.Database.ExecuteSqlInterpolatedAsync($"insert into Order_items(order_id,item_id,quantity) values({oldOrder.order_id},{item_id}, {item.quantity})");
                    var newOrderItems = new OrderItem
                    {
                        order_id = oldOrder.order_id,
                        item_id = existingItem.item_id,
                        quantity = item.quantity
                    };

                    _ctx.Order_Items.Add(newOrderItems);
                    await _ctx.SaveChangesAsync();

                    var price = await _ctx.Items.Where(i => i.item_id == existingItem.item_id).Select(i => i.price).FirstOrDefaultAsync();
                    totalBillAmount = totalBillAmount + (price * item.quantity);
                }

                var p_id = await _ctx.Payments.Where(p => p.order_id == oldOrder.order_id).Select(p => p.payment_id).FirstOrDefaultAsync();

                var TotalBill = new Payment
                {
                    payment_id = p_id,
                    order_id = oldOrder.order_id,
                    amount = totalBillAmount
                };

                _ctx.Payments.Update(TotalBill);
                await _ctx.SaveChangesAsync();

                return "Order updated sucessfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _ctx.Database.ExecuteSqlRawAsync("Call orders({0},{1},{2},{3},{4},{5},{6},{7})",
                                                    "delete", id, 0, "null", "null", "null", "null", 0);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
