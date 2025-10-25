using Microsoft.EntityFrameworkCore;
using SweetHotel.API.Data;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Enums;

namespace SweetHotel.API.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Room>> GetByCategoryIdAsync(string categoryId)
        {
            return await _dbSet
                .Where(r => r.CategoryId == categoryId)
                .Include(r => r.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
        {
            return await _dbSet
                .Where(r => r.Status == RoomStatus.Available)
                .Include(r => r.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByStatusAsync(int status)
        {
            return await _dbSet
                .Where(r => (int)r.Status == status)
                .Include(r => r.Category)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomWithCategoryAsync(string id)
        {
            return await _dbSet
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> GetRoomsWithCategoryAsync()
        {
            return await _dbSet
                .Include(r => r.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(r => r.Price >= minPrice && r.Price <= maxPrice)
                .Include(r => r.Category)
                .ToListAsync();
        }
    }
}
