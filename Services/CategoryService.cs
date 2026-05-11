using FindYourMusic.Data;
using FindYourMusic.Models;
using Microsoft.EntityFrameworkCore;

namespace FindYourMusic.Services {
    public class CategoryService {
        private readonly DatabaseContext _dbContext;

        public CategoryService(DatabaseContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryGroup>> GetAllCategoryGroups() {
            return await _dbContext.CategoryGroup.ToListAsync();
        }

        public async Task<List<Category>> GetAllCategories() {
            return await _dbContext.Category.ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesByIds(List<int> categoryIds){
            return await _dbContext.Category.Where(c => categoryIds.Contains(c.id)).ToListAsync();
        }
    }
}
