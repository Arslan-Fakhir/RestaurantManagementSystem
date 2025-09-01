using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;
using RestaurantDatabaseManagement.Services.Interfaces;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _ctx;
        public ItemService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<ItemResponse>> GetAllAsync()
        {
            try
            {
                return await _ctx.ItemResponse.FromSqlRaw("Call items ({0},{1},{2},{3},{4},{5},{6},{7})",
                    "select", 0, null, 0, null, null, 0, 10000).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<ItemResponse>();
            }
        }

        public async Task<List<ItemResponse>> GetByIdAsync(int id)
        {
            try
            {
                return await _ctx.ItemResponse.FromSqlRaw("Call items ({0},{1},{2},{3},{4},{5},{6},{7})",
                    "select", id, null, 50, false, null, 0, 10000).ToListAsync();
            }
            catch (Exception)
            {
                return new List<ItemResponse>();
            }
        }

        public async Task<string> PostAsync(ItemRequest item)
        {
            try
            {
                string name = item.parent_category_name;
                string[] parents = name.Split(',');

                foreach (var parent in parents)
                {
                    var category = await _ctx.Category
                    .Where(c => c.category_name == parent)
                    .FirstOrDefaultAsync();

                    if (category == null)
                    {
                        return $"Category {parent} not found.";
                    }
                }

                await _ctx.Database.ExecuteSqlRawAsync("Call items ({0},{1},{2},{3},{4},{5},{6},{7})",
                "insert", 0, item.item_name, item.price, item.hasParent, item.parent_category_name, 0, 10000);

                return "Item added successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private async Task<bool> CheckParentCategoryExist(string[] categories)
        {
            foreach(var category in categories)
            {
                var exist = await _ctx.Category
                    .Where(c => c.category_name == category)
                    .FirstOrDefaultAsync();

                if (exist == null)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<string> PutAsync(ItemRequest item)
        {
            try
            {
                var existingItem = await _ctx.Items
                    .Where(i => i.item_id == item.item_id)
                    .FirstOrDefaultAsync();

                if (existingItem == null)
                {
                    return $"Item ID {item.item_id} not found.";
                }

                string[] parents = item.parent_category_name.Split(',');

                bool isParentExist = await CheckParentCategoryExist(parents); //check if items exist

                if (isParentExist)
                {
                    await _ctx.Database.ExecuteSqlRawAsync("Call items ({0},{1},{2},{3},{4},{5},{6},{7})",
                        "update", item.item_id, item.item_name, item.price, item.hasParent, item.parent_category_name, 0, 10000);

                    return "Item updated successfully.";
                }
                else
                {
                    return $"One or more category(s) not found.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> DeleteAsync(int id)
        {
            try
            {
                var existingItem = await _ctx.Items
                    .Where(i => i.item_id == id)
                    .FirstOrDefaultAsync();

                if (existingItem == null)
                {
                    return $"Item ID {id} not found.";
                }

                var mappings = await _ctx.Item_Mapping.Where(im => im.item_id == id).ToListAsync();

                foreach (var mapping in mappings)
                {
                    _ctx.Item_Mapping.Remove(mapping);
                    await _ctx.SaveChangesAsync();
                }
                _ctx.Items.Remove(existingItem);
                await _ctx.SaveChangesAsync();

                return "Item deleted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
