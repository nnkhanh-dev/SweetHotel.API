using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.Enums;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        /// <summary>
        /// L?y danh sách t?t c? Room Status - T?t c? có th? xem
        /// </summary>
        [HttpGet("room-statuses")]
        [AllowAnonymous]
        public IActionResult GetRoomStatuses()
        {
            var statuses = Enum.GetValues(typeof(RoomStatus))
                .Cast<RoomStatus>()
                .Select(s => new
                {
                    value = (int)s,
                    name = s.ToString(),
                    description = GetRoomStatusDescription(s)
                });

            return Ok(statuses);
        }

        /// <summary>
        /// L?y danh sách t?t c? Booking Status - T?t c? có th? xem
        /// </summary>
        [HttpGet("booking-statuses")]
        [AllowAnonymous]
        public IActionResult GetBookingStatuses()
        {
            var statuses = Enum.GetValues(typeof(BookingStatus))
                .Cast<BookingStatus>()
                .Select(s => new
                {
                    value = (int)s,
                    name = s.ToString(),
                    description = GetBookingStatusDescription(s)
                });

            return Ok(statuses);
        }

        private string GetRoomStatusDescription(RoomStatus status)
        {
            return status switch
            {
                RoomStatus.Unavailable => "Không kh? d?ng",
                RoomStatus.Available => "Còn tr?ng, s?n sàng cho thuê",
                RoomStatus.Occupied => "?ang ???c thuê",
                RoomStatus.Maintenance => "?ang b?o trì",
                RoomStatus.Cleaning => "?ang d?n d?p",
                _ => ""
            };
        }

        private string GetBookingStatusDescription(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => "Ch? xác nh?n",
                BookingStatus.Confirmed => "?ã xác nh?n",
                BookingStatus.Cancelled => "?ã h?y",
                BookingStatus.CheckedIn => "?ang s? d?ng (?ã check-in)",
                BookingStatus.Completed => "?ã hoàn thành (?ã check-out)",
                BookingStatus.NoShow => "Không ??n (No-show)",
                _ => ""
            };
        }
    }
}
