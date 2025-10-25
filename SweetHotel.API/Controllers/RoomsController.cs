using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.Room;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _unitOfWork.Rooms.GetRoomsWithCategoryAsync();
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            return Ok(roomsDto);
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDetailDto>> GetRoom(string id)
        {
            var room = await _unitOfWork.Rooms.GetRoomWithCategoryAsync(id);

            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }

            var roomDto = _mapper.Map<RoomDetailDto>(room);
            
            // Get room images
            var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(id);
            roomDto.Images = _mapper.Map<List<RoomImageDto>>(images);

            return Ok(roomDto);
        }

        // GET: api/Rooms/Available
        [HttpGet("Available")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms()
        {
            var rooms = await _unitOfWork.Rooms.GetAvailableRoomsAsync();
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            return Ok(roomsDto);
        }

        // GET: api/Rooms/ByCategory/5
        [HttpGet("ByCategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByCategory(string categoryId)
        {
            var rooms = await _unitOfWork.Rooms.GetByCategoryIdAsync(categoryId);
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            return Ok(roomsDto);
        }

        // GET: api/Rooms/ByPriceRange?minPrice=100&maxPrice=500
        [HttpGet("ByPriceRange")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var rooms = await _unitOfWork.Rooms.GetRoomsByPriceRangeAsync(minPrice, maxPrice);
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            return Ok(roomsDto);
        }

        // POST: api/Rooms
        [HttpPost]
        public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomDto createRoomDto)
        {
            // Validate category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(createRoomDto.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Category not found" });
            }

            var room = _mapper.Map<Room>(createRoomDto);
            
            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            var roomDto = _mapper.Map<RoomDto>(room);
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, roomDto);
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(string id, UpdateRoomDto updateRoomDto)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);

            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }

            // Validate category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(updateRoomDto.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Category not found" });
            }

            _mapper.Map(updateRoomDto, room);
            _unitOfWork.Rooms.Update(room);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);

            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }

            _unitOfWork.Rooms.Remove(room);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
