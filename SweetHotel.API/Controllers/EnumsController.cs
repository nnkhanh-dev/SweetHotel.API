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
                RoomStatus.Unavailable => "Không khả dụng",
                RoomStatus.Available => "Còn trống, sẵn sàng cho thuê",
                RoomStatus.Occupied => "Đang được thuê",
                RoomStatus.Maintenance => "Đang bảo trì",
                RoomStatus.Cleaning => "Đang dọn dẹp",
                _ => ""
            };
        }

        private string GetBookingStatusDescription(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => "Chờ xác nhận",
                BookingStatus.Confirmed => "Đã xác nhận",
                BookingStatus.Cancelled => "Đã hủy",
                BookingStatus.CheckedIn => "Đang sử dụng (Đã check-in)",
                BookingStatus.Completed => "Đã hoàn thành (Đã check-out)",
                BookingStatus.NoShow => "Không đến (No-show)",
                _ => ""
            };
        }
    }
}
