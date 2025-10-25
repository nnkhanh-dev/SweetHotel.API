using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetHotel.API.Entities.Entities
{
    public class Review
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string BookingId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public Booking? Booking { get; set; }
    }
}
