using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.Review;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;
using SweetHotel.API.Enums;

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

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDetailDto>>> GetReviews()
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsWithBookingAsync();
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDetailDto>>(reviews);
            return Ok(reviewsDto);
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
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

        // GET: api/Reviews/ByBooking/bookingId
        [HttpGet("ByBooking/{bookingId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByBooking(string bookingId)
        {
            var reviews = await _unitOfWork.Reviews.GetByBookingIdAsync(bookingId);
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(reviewsDto);
        }

        // GET: api/Reviews/Recent/10
        [HttpGet("Recent/{count}")]
        public async Task<ActionResult<IEnumerable<ReviewDetailDto>>> GetRecentReviews(int count)
        {
            var reviews = await _unitOfWork.Reviews.GetRecentReviewsAsync(count);
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDetailDto>>(reviews);
            return Ok(reviewsDto);
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createReviewDto)
        {
            // Validate booking exists
            var booking = await _unitOfWork.Bookings.GetByIdAsync(createReviewDto.BookingId);
            if (booking == null)
            {
                return BadRequest(new { message = "Booking not found" });
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

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(string id, UpdateReviewDto updateReviewDto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            _mapper.Map(updateReviewDto, review);
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            _unitOfWork.Reviews.Remove(review);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
