using Microsoft.EntityFrameworkCore;
using SweetHotel.API.Data;
using SweetHotel.API.Entities.Entities;

namespace SweetHotel.API.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Category>> GetByMaxPeopleAsync(int maxPeople)
        {
            return await _dbSet.Where(c => c.MaxPeople >= maxPeople).ToListAsync();
        }
    }
}
