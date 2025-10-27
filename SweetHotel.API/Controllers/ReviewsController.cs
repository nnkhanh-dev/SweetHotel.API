using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.Review;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;
using SweetHotel.API.Enums;
using System.Security.Claims;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Reviews - T?t c? có th? xem
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReviewDetailDto>>> GetReviews()
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsWithBookingAsync();
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDetailDto>>(reviews);
            return Ok(reviewsDto);
        }

        // GET: api/Reviews/5 - T?t c? có th? xem
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReviewDetailDto>> GetReview(string id)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithBookingAsync(id);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            var reviewDto = _mapper.Map<ReviewDetailDto>(review);
            return Ok(reviewDto);
        }

        // GET: api/Reviews/ByBooking/bookingId - T?t c? có th? xem
        [HttpGet("ByBooking/{bookingId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByBooking(string bookingId)
        {
            var reviews = await _unitOfWork.Reviews.GetByBookingIdAsync(bookingId);
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(reviewsDto);
        }

        // GET: api/Reviews/Recent/10 - T?t c? có th? xem
        [HttpGet("Recent/{count}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReviewDetailDto>>> GetRecentReviews(int count)
        {
            var reviews = await _unitOfWork.Reviews.GetRecentReviewsAsync(count);
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDetailDto>>(reviews);
            return Ok(reviewsDto);
        }

        // POST: api/Reviews - Client t?o review
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createReviewDto)
        {
            // Validate booking exists
            var booking = await _unitOfWork.Bookings.GetByIdAsync(createReviewDto.BookingId);
            if (booking == null)
            {
                return BadRequest(new { message = "Booking not found" });
            }

            // Ki?m tra quy?n: ch? ???c review booking c?a chính mình
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.UserId != currentUserId)
            {
                return Forbid();
            }

            // Ki?m tra booking ?ã hoàn thành ch?a
            if (booking.Status != BookingStatus.Completed)
            {
                return BadRequest(new { message = "Ch? có th? ?ánh giá sau khi hoàn thành booking (?ã check-out)" });
            }

            // Ki?m tra ?ã có review cho booking này ch?a
            var existingReviews = await _unitOfWork.Reviews.GetByBookingIdAsync(createReviewDto.BookingId);
            if (existingReviews.Any())
            {
                return BadRequest(new { message = "Booking này ?ã ???c ?ánh giá" });
            }

            var review = _mapper.Map<Review>(createReviewDto);
            review.CreatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var reviewDto = _mapper.Map<ReviewDto>(review);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDto);
        }

        // PUT: api/Reviews/5 - Client s?a review c?a mình ho?c Admin
        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> UpdateReview(string id, UpdateReviewDto updateReviewDto)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithBookingAsync(id);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            // Ki?m tra quy?n: Admin ho?c ch? review
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && review.Booking?.UserId != currentUserId)
            {
                return Forbid();
            }

            _mapper.Map(updateReviewDto, review);
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Reviews/5 - Client xóa review c?a mình ho?c Admin
        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithBookingAsync(id);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            // Ki?m tra quy?n: Admin ho?c ch? review
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && review.Booking?.UserId != currentUserId)
            {
                return Forbid();
            }

            _unitOfWork.Reviews.Remove(review);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
