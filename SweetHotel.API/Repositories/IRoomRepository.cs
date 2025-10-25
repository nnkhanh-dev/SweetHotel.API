using SweetHotel.API.Entities.Entities;

namespace SweetHotel.API.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<IEnumerable<Room>> GetByCategoryIdAsync(string categoryId);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync();
        Task<IEnumerable<Room>> GetRoomsByStatusAsync(int status);
        Task<Room?> GetRoomWithCategoryAsync(string id);
        Task<IEnumerable<Room>> GetRoomsWithCategoryAsync();
        Task<IEnumerable<Room>> GetRoomsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}
