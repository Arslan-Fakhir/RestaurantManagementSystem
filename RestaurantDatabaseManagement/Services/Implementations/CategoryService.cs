using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Models;
using RestaurantDatabaseManagement.Models.Request;
using RestaurantDatabaseManagement.Models.Response;
using RestaurantDatabaseManagement.Services.Interfaces;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

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
            try
            {
                return await _ctx.CategoryResponse.FromSqlRaw("CALL Category({0},{1},{2},{3},{4},{5},{6})",
                    "select", 0, null, false, null, 0, 10000).ToListAsync();
            }catch(Exception ex)
            {
                return new List<CategoryResponse>();
            }

        }
        public async Task<List<CategoryResponse>> GetByIdAsync(int id)
        {
            try
            {
                return await _ctx.CategoryResponse.FromSqlRaw("CALL Category({0},{1},{2},{3},{4},{5},{6})",
                    "select", id, null, false, null, 0, 10000).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<CategoryResponse>();
            }
        }

        public async Task<string> PostAsync(CategoryRequest category)
        {
            try
            {
                if(category.sub_category==true)
                {
                    if (category.parent_category_name != "string" && category.parent_category_name != "")
                    {
                        var exists = new Category();
                        exists = await _ctx.Category.Where(c => c.category_name == category.category_name).FirstOrDefaultAsync(); //Check if the caregory already exists

                        if (exists?.category_name == category.category_name)
                        {
                            return $"Category '{category.category_name}' already exist in database.";
                        }
                        else
                        {
                            string[] parents = category.parent_category_name.Split(',');
                            var isCategory = await CheckCategoryExist(parents);

                            if (isCategory)
                            {
                                var newCategory = new Category
                                {
                                    category_name = category.category_name
                                };
                                _ctx.Category.Add(newCategory);
                                _ctx.SaveChanges();

                                var childCategory = await _ctx.Category.Where(c => c.category_name == category.category_name).FirstOrDefaultAsync();

                                foreach (string parent in parents)
                                {
                                    var parentCategory = await _ctx.Category.Where(c => c.category_name == parent).FirstOrDefaultAsync();
                                    var mapping = new CategoryMapping()
                                    {
                                        child_category_id = childCategory.category_id,
                                        parent_category_id = parentCategory.category_id
                                    };
                                    _ctx.Category_Mapping.Add(mapping);
                                }
                                await _ctx.SaveChangesAsync();
                                return "Category and it's mapping created successfully.";
                            }
                            else
                            {
                                return "Category creation failed! One or more parent_category_name not found";
                            }
                        }
 
                    }
                    else
                    {
                        return $"Failed to create! If (sub-category = true) you must enter parent_category_name.";
                    }
                }
                else
                {
                    var exists = await _ctx.Category.Where(c => c.category_name == category.category_name).FirstOrDefaultAsync();
                    if (exists?.category_name == category.category_name)
                    {
                        return $"Category '{exists.category_name}' already exist in database.";
                    }
                    var newCategory = new Category()
                    {
                        category_name=category.category_name
                    };
                    _ctx.Category.Add(newCategory);
                    await _ctx.SaveChangesAsync();
                    return "Category created successfully.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private async Task<bool> CheckParentCategory(int child, int id)
        {
            if (id != 0)
            {

                var mappings = await _ctx.Category_Mapping //mappings will contain all the parents
                            .Where(cm => cm.child_category_id == id)
                            .ToListAsync();

                foreach (var parent in mappings) //each parent will be checked
                {
                    if (child == parent.parent_category_id) //compare if child already exists as a parent 
                        return false;

                    return await CheckParentCategory(child, parent.parent_category_id); //To check grand parents of parent
                }
            }
            return true;
        }

        private async Task<bool> CheckCategoryExist(string[] categories)
        {
            foreach (var category in categories)
            {
                var existingCategory = await _ctx.Category
                    .Where(c => c.category_name == category)
                    .FirstOrDefaultAsync();

                if (existingCategory == null)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<string> PutAsync(CategoryRequest category)
        {
            try
            {
                var existingCategory = await _ctx.Category
                    .Where(c => c.category_id == category.category_id)
                    .FirstOrDefaultAsync();

                if (existingCategory == null)
                {
                    return $"Category ID {category.category_id} not found.";
                }

                if (category.sub_category == true && category.parent_category_name != null)
                {
                    if (category.category_name != null)
                    {
                        existingCategory.category_name = category.category_name;

                        _ctx.Category.Update(existingCategory);
                        await _ctx.SaveChangesAsync();
                    }

                    string[] parentCategories = category.parent_category_name.Split(','); // add split function to update multiple parents

                    bool isCategory = await CheckCategoryExist(parentCategories); //find parent category if exists
                    if (isCategory == false)
                        return "Cannot update! One or more parent categories were not found.";

                    var isDeleted = await _ctx.Database
                        .ExecuteSqlRawAsync($"delete from category_mapping where child_category_id= {category.category_id}"); //delete existing parent-child relation

                    foreach (var parent in parentCategories)
                    {
                        var parentCategoryCheck = await _ctx.Category.Where(c => c.category_name == parent).FirstOrDefaultAsync(); //get category details of parent 

                        if (parentCategoryCheck != null)
                        {
                            var isParent = await CheckParentCategory(category.category_id, parentCategoryCheck.category_id); //check if the child category already exist as parent 

                            if (isParent == false)
                            {
                                return $"Category '{existingCategory.category_name}' cannot be child of category '{parent}'.";
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
                            }
                        }
                    }
                    return "Parent-child relation updated sucessfully.";
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
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> DeleteAsync(DeleteCategoryRequest category)
        {
            try
            {
                var existingParent = new Category();

                var existingCategory = await _ctx.Category
                    .Where(c => c.category_id == category.category_id) //check if category exists
                    .FirstOrDefaultAsync();

                if (existingCategory == null)
                {
                    return $"Category ID {category.category_id} not found.";
                }

                var childCount = await _ctx.Category_Mapping.CountAsync(cm => cm.parent_category_id == category.category_id); //check if category is parent to any other category
                
                if (childCount != 0)
                {
                    return $"Cannot delete {existingCategory.category_name}! It is a parent to other categories.";
                }

                if (category.parent_category_name == "" || category.parent_category_name == "string")
                {
                    var parentCount = await _ctx.Category_Mapping.CountAsync(cm => cm.child_category_id == category.category_id); //check if category is child to any other category

                    if (parentCount != 0)
                    {
                        return $"Cannot delete {existingCategory.category_name}! It has parent category mapping(s). Please specify parent_category_name to delete specific mapping.";
                    }

                    _ctx.Category.Remove(existingCategory); //remove category if it has no parent mapping
                    await _ctx.SaveChangesAsync();
                    return "Category deleted successfully.";
                }


                existingParent = await _ctx.Category
                    .Where(c => c.category_name == category.parent_category_name) //check if parent category exists
                    .FirstOrDefaultAsync();

                if (existingParent.category_name == null)
                {
                    return $"Parent Category '{category.parent_category_name}' not found.";
                }
                var isMyParent = await _ctx.Category_Mapping.Where(cm => cm.child_category_id == category.category_id && cm.parent_category_id == existingParent.category_id).FirstOrDefaultAsync();
                if (isMyParent != null)
                {
                    _ctx.Category_Mapping.RemoveRange(_ctx.Category_Mapping
                        .Where(cm => cm.child_category_id == category.category_id && cm.parent_category_id == existingParent.category_id));//remove specific parent-child mapping
                    await _ctx.SaveChangesAsync();
                }
                else
                {
                    return $"Category '{existingCategory.category_name}' is not a child of category '{category.parent_category_name}'.";
                }
                    var pCount = await _ctx.Category_Mapping.CountAsync(cm => cm.child_category_id == category.category_id); //check if category is child to any other category
                if (pCount != 0)
                {
                    return $"Category mapping deleted successfully.";
                }

                _ctx.Category.Remove(existingCategory); //remove category if it has no other parent mapping
                await _ctx.SaveChangesAsync();

                return "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error deleting category: {ex.Message}";
            }
        }
    }
}
