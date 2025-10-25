using System.ComponentModel.DataAnnotations;

namespace SweetHotel.API.DTOs.Review
{
    public class ReviewDto
    {
        public string Id { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string BookingId { get; set; } = string.Empty;
    }

    public class ReviewDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string BookingId { get; set; } = string.Empty;
        public BookingDto? Booking { get; set; }
    }

    public class CreateReviewDto
    {
        [Required(ErrorMessage = "Rating là b?t bu?c")]
        [Range(1, 5, ErrorMessage = "Rating ph?i t? 1 ??n 5")]
        public int Rating { get; set; }
        
        [Required(ErrorMessage = "Comment là b?t bu?c")]
        [StringLength(1000, ErrorMessage = "Comment t?i ?a 1000 ký t?")]
        public string Comment { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "BookingId là b?t bu?c")]
        public string BookingId { get; set; } = string.Empty;
    }

    public class UpdateReviewDto
    {
        [Range(1, 5, ErrorMessage = "Rating ph?i t? 1 ??n 5")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Comment t?i ?a 1000 ký t?")]
        public string Comment { get; set; } = string.Empty;
    }

    public class BookingDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RoomId { get; set; } = string.Empty;
    }
}
