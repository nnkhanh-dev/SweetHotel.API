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

        public async Task<IEnumerable<Room>> GetAvailableRoomsByDateRangeAsync(DateTime startDate, DateTime endDate, string? categoryId = null, int? maxPeople = null)
        {
            // L?y t?t c? các phòng có tr?ng thái Available
            var query = _dbSet
                .Include(r => r.Category)
                .Where(r => r.Status == RoomStatus.Available);

            // L?c theo danh m?c n?u có
            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(r => r.CategoryId == categoryId);
            }

            // L?c theo s? ng??i n?u có
            if (maxPeople.HasValue)
            {
                query = query.Where(r => r.Category != null && r.Category.MaxPeople >= maxPeople.Value);
            }

            var rooms = await query.ToListAsync();

            // L?c các phòng không bị đặt trong kho?ng th?i gian
            var availableRooms = new List<Room>();
            foreach (var room in rooms)
            {
                var hasConflict = await _context.Bookings.AnyAsync(b =>
                    b.RoomId == room.Id &&
                    b.Status != BookingStatus.Cancelled &&
                    b.Status != BookingStatus.NoShow &&
                    ((b.StartDate <= startDate && b.EndDate >= startDate) ||
                     (b.StartDate <= endDate && b.EndDate >= endDate) ||
                     (b.StartDate >= startDate && b.EndDate <= endDate)));

                if (!hasConflict)
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms;
        }
    }
}
