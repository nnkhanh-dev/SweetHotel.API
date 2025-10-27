using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.RoomImages;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomImagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomImagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/RoomImages - T?t c? có th? xem
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomImageDto>>> GetRoomImages()
        {
            var images = await _unitOfWork.RoomImages.GetAllAsync();
            var imagesDto = _mapper.Map<IEnumerable<RoomImageDto>>(images);
            return Ok(imagesDto);
        }

        // GET: api/RoomImages/5 - T?t c? có th? xem
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RoomImageDto>> GetRoomImage(string id)
        {
            var image = await _unitOfWork.RoomImages.GetByIdAsync(id);

            if (image == null)
            {
                return NotFound(new { message = "Room image not found" });
            }

            var imageDto = _mapper.Map<RoomImageDto>(image);
            return Ok(imageDto);
        }

        // GET: api/RoomImages/ByRoom/roomId - T?t c? có th? xem
        [HttpGet("ByRoom/{roomId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomImageDto>>> GetImagesByRoom(string roomId)
        {
            var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomId);
            var imagesDto = _mapper.Map<IEnumerable<RoomImageDto>>(images);
            return Ok(imagesDto);
        }

        // POST: api/RoomImages - Ch? Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomImageDto>> CreateRoomImage(CreateRoomImageDto createRoomImageDto)
        {
            // Validate room exists
            var room = await _unitOfWork.Rooms.GetByIdAsync(createRoomImageDto.RoomId);
            if (room == null)
            {
                return BadRequest(new { message = "Room not found" });
            }

            var image = _mapper.Map<RoomImages>(createRoomImageDto);
            
            await _unitOfWork.RoomImages.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            var imageDto = _mapper.Map<RoomImageDto>(image);
            return CreatedAtAction(nameof(GetRoomImage), new { id = image.Id }, imageDto);
        }

        // PUT: api/RoomImages/5 - Ch? Admin
        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoomImage(string id, UpdateRoomImageDto updateRoomImageDto)
        {
            var image = await _unitOfWork.RoomImages.GetByIdAsync(id);

            if (image == null)
            {
                return NotFound(new { message = "Room image not found" });
            }

            _mapper.Map(updateRoomImageDto, image);
            _unitOfWork.RoomImages.Update(image);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/RoomImages/5 - Ch? Admin
        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomImage(string id)
        {
            var image = await _unitOfWork.RoomImages.GetByIdAsync(id);

            if (image == null)
            {
                return NotFound(new { message = "Room image not found" });
            }

            _unitOfWork.RoomImages.Remove(image);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
