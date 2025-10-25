using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetHotel.API.Entities.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
    }
}
