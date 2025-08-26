using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Services.Interfaces;
using System.Diagnostics;
using System.Linq;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _ctx;
        public OrderService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            return await _ctx.OrderResponse.FromSqlRaw("Call orders({0},{1},{2},{3},{4},{5},{6},{7})",
                                                    "select", 0, 0, "null", "null", "null", "null", 0).ToListAsync();
        }

        public async Task<List<OrderResponse>> GetByIdAsync(int id)
        {
            return await _ctx.OrderResponse.FromSqlRaw("Call orders({0},{1},{2},{3},{4},{5},{6},{7})",
                                                        "select", id, 0, "null", "null", "null", "null", 0).ToListAsync();
        }

        public async Task<string> PostAsync(OrderRequest order)
        {

            Customer customer = await _ctx.Customers
                .Where(c => c.contact == order.contact && c.email ==order.email)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                if (order.customer_name.Contains(" "))
                {
                    await _ctx.Database.ExecuteSqlRawAsync("Call customers({0},{1},{2},{3},{4},{5})",
                        "insert", 0, order.customer_name.Split(' ')[0], order.customer_name.Split(' ')[1], order.contact, order.email);
                }
                else
                {
                    await _ctx.Database.ExecuteSqlRawAsync("Call customers({0},{1},{2},{3},{4},{5})",
                        "insert", 0, order.customer_name, " ", order.contact, order.email);
                }

                    customer = await _ctx.Customers
                        .Where(c => c.contact == order.contact && c.email == order.email)
                        .FirstOrDefaultAsync();
            }

            var newOrder = new Order
            {
                customer_id = customer.customer_id,
                customer_name = order.customer_name,
                customer_contact = order.contact,
                customer_email = order.email
            };

            _ctx.Orders.Add(newOrder);
            await _ctx.SaveChangesAsync();

            double totalBillAmount = 0;

            foreach (var item in order.Items)
            {
                var item_id = await _ctx.Items.Where(i => i.item_name == item.item_name).Select(c => c.item_id).FirstOrDefaultAsync();
                if (item_id == 0)
                    return $"Item '{item.item_name}' not found.";


                await _ctx.Database.ExecuteSqlInterpolatedAsync($"insert into Order_items(order_id,item_id,quantity) values({newOrder.order_id},{item_id}, {item.quantity})");

                var price = await _ctx.Items.Where(i => i.item_id == item_id).Select(i => i.price).FirstOrDefaultAsync();
                totalBillAmount = totalBillAmount + (price * item.quantity);
            }

            var TotalBill = new Payment
            {
                order_id = newOrder.order_id,
                amount = totalBillAmount
            };

            _ctx.Payments.Add(TotalBill);
            await _ctx.SaveChangesAsync();

            return "Order created successfully.";
        }

        public async Task<string> PutAsync(OrderRequest order)
        {
            Order oldOrder = await _ctx.Orders
                .Where(o => o.order_id == order.order_id)
                .FirstOrDefaultAsync();

            if (oldOrder == null)
            {
                return "Order record not found.";
            }

            oldOrder.customer_name = order.customer_name;
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
            double totalBillAmount =0 ;

            foreach (var item in order.Items)
            {
                var item_id = await _ctx.Items.Where(i => i.item_name == item.item_name).Select(c => c.item_id).FirstOrDefaultAsync();

                if (item_id == 0)
                    return $"Item '{item.item_name}' not found.";

                await _ctx.Database.ExecuteSqlInterpolatedAsync($"insert into Order_items(order_id,item_id,quantity) values({oldOrder.order_id},{item_id}, {item.quantity})");

                var price = await _ctx.Items.Where(i => i.item_id == item_id).Select(i => i.price).FirstOrDefaultAsync();
                totalBillAmount = totalBillAmount + (price * item.quantity);
            }

            var p_id = await _ctx.Payments.Where(p=>p.order_id==oldOrder.order_id).Select(p=>p.payment_id).FirstOrDefaultAsync();

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
        public async Task<int> DeleteAsync(int id)
        {
            return await _ctx.Database.ExecuteSqlRawAsync("Call orders({0},{1},{2},{3},{4},{5},{6},{7})",
                                                "delete", id, 0, "null", "null", "null", "null", 0);
        }


    }
}
