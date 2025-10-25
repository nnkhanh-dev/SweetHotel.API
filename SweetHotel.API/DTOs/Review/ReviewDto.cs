namespace SweetHotel.API.DTOs.Review
{
    public class ReviewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string BookingId { get; set; } = string.Empty;
    }

    public class ReviewDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string BookingId { get; set; } = string.Empty;
        public BookingDto? Booking { get; set; }
    }

    public class CreateReviewDto
    {
        public string Comment { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
    }

    public class UpdateReviewDto
    {
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
