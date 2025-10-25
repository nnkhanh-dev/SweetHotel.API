using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SweetHotel.API.Enums;

namespace SweetHotel.API.Entities.Entities
{
    public class Room
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public RoomStatus Status { get; set; }
        public string Amenities { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }  
        public int discount { get; set; }   
        public string CategoryId { get; set; } = string.Empty;
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}
