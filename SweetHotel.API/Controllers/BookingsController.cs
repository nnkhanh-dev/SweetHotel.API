using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.Booking;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;
using SweetHotel.API.Enums;
using System.Security.Claims;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Bookings - Ch? Admin xem t?t c?
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookings()
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsWithDetailsAsync();
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/5 - Admin ho?c ch? booking
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<ActionResult<BookingDetailDto>> GetBooking(string id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Ki?m tra quy?n: Admin ho?c ch? booking
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && booking.UserId != userId)
            {
                return Forbid();
            }

            var bookingDto = _mapper.Map<BookingDetailDto>(booking);
            return Ok(bookingDto);
        }

        // GET: api/Bookings/ByUser/userId - Admin ho?c chính user ?ó
        [HttpGet("ByUser/{userId}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByUser(string userId)
        {
            // Ki?m tra quy?n: Admin ho?c chính user ?ó
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && currentUserId != userId)
            {
                return Forbid();
            }

            var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/MyBookings/{userId} - Client xem l?ch s? c?a mình
        [HttpGet("MyBookings/{userId}")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<object>> GetMyBookingHistory(string userId)
        {
            // Ki?m tra quy?n: ch? ???c xem booking c?a chính mình
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (currentUserId != userId)
            {
                return Forbid();
            }

            var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            
            // Phân lo?i bookings
            var result = new
            {
                upcoming = bookingsDto.Where(b => 
                    b.Status == BookingStatus.Pending || 
                    b.Status == BookingStatus.Confirmed).OrderBy(b => b.StartDate),
                current = bookingsDto.Where(b => b.Status == BookingStatus.CheckedIn).OrderByDescending(b => b.StartDate),
                completed = bookingsDto.Where(b => b.Status == BookingStatus.Completed).OrderByDescending(b => b.EndDate),
                cancelled = bookingsDto.Where(b => b.Status == BookingStatus.Cancelled).OrderByDescending(b => b.StartDate),
                all = bookingsDto.OrderByDescending(b => b.StartDate)
            };

            return Ok(result);
        }

        // GET: api/Bookings/ByRoom/roomId - Admin
        [HttpGet("ByRoom/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByRoom(string roomId)
        {
            var bookings = await _unitOfWork.Bookings.GetByRoomIdAsync(roomId);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/ByStatus/1 - Admin
        [HttpGet("ByStatus/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByStatus(int status)
        {
            var bookings = await _unitOfWork.Bookings.GetByStatusAsync(status);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/CheckAvailability - T?t c? có th? check
        [HttpGet("CheckAvailability")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CheckRoomAvailability([FromQuery] string roomId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(roomId, startDate, endDate);
            return Ok(new { isAvailable, roomId, startDate, endDate });
        }

        // POST: api/Bookings - Client ??t phòng
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            // Ki?m tra userId trong request ph?i trùng v?i user hi?n t?i
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (createBookingDto.UserId != currentUserId)
            {
                return BadRequest(new { message = "Không th? ??t phòng cho ng??i khác" });
            }

            // Validate room exists
            var room = await _unitOfWork.Rooms.GetByIdAsync(createBookingDto.RoomId);
            if (room == null)
            {
                return BadRequest(new { message = "Room not found" });
            }

            // Check room availability
            var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(
                createBookingDto.RoomId, 
                createBookingDto.StartDate, 
                createBookingDto.EndDate);

            if (!isAvailable)
            {
                return BadRequest(new { message = "Room is not available for the selected dates" });
            }

            var booking = _mapper.Map<Booking>(createBookingDto);
            
            // Calculate total price
            var days = (createBookingDto.EndDate - createBookingDto.StartDate).Days;
            var discountedPrice = room.Price * (1 - room.discount / 100m);
            booking.TotalPrice = discountedPrice * days;

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var bookingDto = _mapper.Map<BookingDto>(booking);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, bookingDto);
        }

        // PUT: api/Bookings/5 - Admin c?p nh?t
        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBooking(string id, UpdateBookingDto updateBookingDto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            _mapper.Map(updateBookingDto, booking);
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Bookings/5 - Admin xóa
        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            _unitOfWork.Bookings.Remove(booking);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Bookings/5/Cancel - Client h?y booking c?a mình
        [HttpPost("Cancel/{id}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CancelBooking(string id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Ki?m tra quy?n: ch? ???c h?y booking c?a chính mình
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.UserId != currentUserId)
            {
                return Forbid();
            }

            // Ki?m tra tr?ng thái có th? h?y
            if (booking.Status == BookingStatus.Cancelled)
            {
                return BadRequest(new { message = "Booking ?ã ???c h?y tr??c ?ó" });
            }

            if (booking.Status == BookingStatus.Completed)
            {
                return BadRequest(new { message = "Không th? h?y booking ?ã hoàn thành" });
            }

            if (booking.Status == BookingStatus.CheckedIn)
            {
                return BadRequest(new { message = "Không th? h?y booking ?ang s? d?ng" });
            }

            // C?p nh?t tr?ng thái
            booking.Status = BookingStatus.Cancelled;
            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "?ã h?y booking thành công", bookingId = id });
        }
    }
}
