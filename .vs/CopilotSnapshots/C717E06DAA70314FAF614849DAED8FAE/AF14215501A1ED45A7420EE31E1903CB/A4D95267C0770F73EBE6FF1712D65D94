using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetHotel.API.Entities.Entities
{
    public class RoomImages
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        [ForeignKey("RoomId")]  
        public Room? Room { get; set; }
    }
}
