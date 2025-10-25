using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.Booking;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;

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

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookings()
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsWithDetailsAsync();
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDetailDto>> GetBooking(string id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            var bookingDto = _mapper.Map<BookingDetailDto>(booking);
            return Ok(bookingDto);
        }

        // GET: api/Bookings/ByUser/userId
        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByUser(string userId)
        {
            var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/ByRoom/roomId
        [HttpGet("ByRoom/{roomId}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByRoom(string roomId)
        {
            var bookings = await _unitOfWork.Bookings.GetByRoomIdAsync(roomId);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/ByStatus/1
        [HttpGet("ByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByStatus(int status)
        {
            var bookings = await _unitOfWork.Bookings.GetByStatusAsync(status);
            var bookingsDto = _mapper.Map<IEnumerable<BookingDetailDto>>(bookings);
            return Ok(bookingsDto);
        }

        // GET: api/Bookings/CheckAvailability?roomId=xxx&startDate=2024-01-01&endDate=2024-01-05
        [HttpGet("CheckAvailability")]
        public async Task<ActionResult<object>> CheckRoomAvailability([FromQuery] string roomId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(roomId, startDate, endDate);
            return Ok(new { isAvailable, roomId, startDate, endDate });
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
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

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
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

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
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
    }
}
