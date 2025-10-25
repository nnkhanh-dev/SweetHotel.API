using Microsoft.EntityFrameworkCore;
using SweetHotel.API.Data;
using SweetHotel.API.Entities.Entities;

namespace SweetHotel.API.Repositories
{
    public class RoomImagesRepository : Repository<RoomImages>, IRoomImagesRepository
    {
        public RoomImagesRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RoomImages>> GetByRoomIdAsync(string roomId)
        {
            return await _dbSet
                .Where(ri => ri.RoomId == roomId)
                .Include(ri => ri.Room)
                .ToListAsync();
        }

        public async Task<RoomImages?> GetImageWithRoomAsync(string id)
        {
            return await _dbSet
                .Include(ri => ri.Room)
                .FirstOrDefaultAsync(ri => ri.Id == id);
        }

        public async Task<IEnumerable<RoomImages>> GetImagesWithRoomAsync()
        {
            return await _dbSet
                .Include(ri => ri.Room)
                .ToListAsync();
        }
    }
}
