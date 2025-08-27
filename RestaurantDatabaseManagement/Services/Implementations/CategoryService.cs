using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;
using RestaurantDatabaseManagement.Services.Interfaces;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _ctx;

        public CategoryService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            return await _ctx.CategoryResponse.FromSqlRaw("CALL Category({0},{1},{2},{3},{4},{5},{6})",
                "select", 0, null, false, null, 0, 100).ToListAsync();
        }
        public async Task<List<CategoryResponse>> GetByIdAsync(int id)
        {
            return await _ctx.CategoryResponse.FromSqlRaw("CALL Category({0},{1},{2},{3},{4},{5},{6})",
                "select", id, null, false, null, 0, 100).ToListAsync();
        }

        public async Task<string> PostAsync(CategoryRequest category)
        {
            await _ctx.Database.ExecuteSqlRawAsync("CALL Category({0},{1},{2},{3},{4},{5},{6})",
                "insert", category.category_id, category.category_name, category.sub_category, category.parent_category_name, category.offset_number, category.fetch_count);
            return "Category added successfully.";
        }

        private async Task<bool> CheckParentCategory(int child, int id)
        {
            if(id!=0)
            {
                
                var mappings = await _ctx.Category_Mapping //mappings will contain all the parents
                            .Where(cm => cm.child_category_id == id)
                            .ToListAsync();

                foreach(var parent in mappings) //each parent will be checked
                {
                    if (child == parent.parent_category_id) //compare if child already exists as a parent 
                        return false;

                    return await CheckParentCategory(child, parent.parent_category_id); //To check grand parents of parent
                }
            }
            return true;
        }


        public async Task<string> PutAsync(CategoryRequest category)
        {
            var existingCategory = await _ctx.Category
                .Where(c => c.category_id == category.category_id)
                .FirstOrDefaultAsync();

            if (existingCategory == null)
            {
                return $"Category ID {category.category_id} not found.";
            }

            if(category.sub_category==true && category.parent_category_name!=null)
            {
                if (category.category_name != null || category.category_name != "string")
                {
                    existingCategory.category_name = category.category_name;

                    _ctx.Category.Update(existingCategory);
                    await _ctx.SaveChangesAsync();
                }

                var parentCategoryCheck = await _ctx.Category.Where(c=>c.category_name==category.parent_category_name).FirstOrDefaultAsync(); //find parent category if exists

                if (parentCategoryCheck != null)
                {

                    var isParent = await CheckParentCategory(category.category_id,parentCategoryCheck.category_id); //check if the child category already exist as parent 

                    if (isParent==false)
                    {
                        return $"Category '{existingCategory.category_name}' cannot be child of category '{category.parent_category_name}'.";
                    }
                    else
                    {
                        var map = new CategoryMapping()
                        {
                            parent_category_id = parentCategoryCheck.category_id,
                            child_category_id = existingCategory.category_id
                        };
                        _ctx.Category_Mapping.Add(map);
                        await _ctx.SaveChangesAsync();

                        return "Parent-child relation updated sucessfully.";
                    }



                }
                return $"Parent category '{category.parent_category_name}' not found.";

            }
            else
            {
                if (category.category_name != null || category.category_name != "string")
                {
                    existingCategory.category_name = category.category_name;

                    _ctx.Category.Update(existingCategory);
                    await _ctx.SaveChangesAsync();
                    return "Category updated successfully.";
                }

                return "Nothing to update!";
            }
        }

        public async Task<string> DeleteAsync(int id)
        {
            var existingCategory = await _ctx.Category
                .Where(c => c.category_id == id)
                .FirstOrDefaultAsync();
            if (existingCategory == null)
            {
                return $"Category ID {id} not found.";
            }
            await _ctx.Database.ExecuteSqlRawAsync("CALL Category({0},{1},{2},{3},{4},{5},{6})",
                "delete", id, null, false, null, 0, 0);

            return "Category deleted successfully.";
        }
    }
}
