using SweetHotel.API.Entities.Entities;

namespace SweetHotel.API.Repositories
{
    public interface IRoomImagesRepository : IRepository<RoomImages>
    {
        Task<IEnumerable<RoomImages>> GetByRoomIdAsync(string roomId);
        Task<RoomImages?> GetImageWithRoomAsync(string id);
        Task<IEnumerable<RoomImages>> GetImagesWithRoomAsync();
    }
}
