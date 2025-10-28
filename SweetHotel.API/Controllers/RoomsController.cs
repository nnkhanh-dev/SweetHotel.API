using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public RoomsController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        private string ToAbsoluteUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return relativePath;
            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute)) return relativePath;

            var scheme = Request?.Scheme ?? _config["App:BaseUrlScheme"] ?? "https";
            var host = Request?.Host.Value ?? _config["App:BaseUrl"] ?? string.Empty;
            var basePath = Request?.PathBase.HasValue == true ? Request.PathBase.Value : string.Empty;
            if (string.IsNullOrEmpty(host)) return relativePath; // fallback to relative if no host
            return $"{scheme}://{host}{basePath}{relativePath}";
        }

        // GET: api/Rooms - T?t c? có th? xem
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _unitOfWork.Rooms.GetRoomsWithCategoryAsync();
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            
            // Load images cho t?ng phòng
            foreach (var roomDto in roomsDto)
            {
                var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
                var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
                // convert to absolute urls
                foreach (var img in imagesDto)
                {
                    img.Path = ToAbsoluteUrl(img.Path);
                }
                roomDto.Images = imagesDto;
            }
            
            return Ok(roomsDto);
        }

        // GET: api/Rooms/5 - T?t c? có th? xem
        [HttpGet("{id}")]
        [AllowAnonymous]
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
            var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
            foreach (var img in imagesDto)
            {
                img.Path = ToAbsoluteUrl(img.Path);
            }
            roomDto.Images = imagesDto;

            return Ok(roomDto);
        }

        // GET: api/Rooms/Available - T?t c? có th? xem
        [HttpGet("Available")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms()
        {
            var rooms = await _unitOfWork.Rooms.GetAvailableRoomsAsync();
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            
            // Load images cho t?ng phòng
            foreach (var roomDto in roomsDto)
            {
                var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
                var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
                foreach (var img in imagesDto)
                {
                    img.Path = ToAbsoluteUrl(img.Path);
                }
                roomDto.Images = imagesDto;
            }
            
            return Ok(roomsDto);
        }

        // GET: api/Rooms/ByCategory/5 - T?t c? có th? xem
        [HttpGet("ByCategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByCategory(string categoryId)
        {
            var rooms = await _unitOfWork.Rooms.GetByCategoryIdAsync(categoryId);
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            
            // Load images cho t?ng phòng
            foreach (var roomDto in roomsDto)
            {
                var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
                var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
                foreach (var img in imagesDto)
                {
                    img.Path = ToAbsoluteUrl(img.Path);
                }
                roomDto.Images = imagesDto;
            }
            
            return Ok(roomsDto);
        }

        // GET: api/Rooms/ByPriceRange?minPrice=100&maxPrice=500 - T?t c? có th? xem
        [HttpGet("ByPriceRange")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var rooms = await _unitOfWork.Rooms.GetRoomsByPriceRangeAsync(minPrice, maxPrice);
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            
            // Load images cho t?ng phòng
            foreach (var roomDto in roomsDto)
            {
                var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
                var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
                foreach (var img in imagesDto)
                {
                    img.Path = ToAbsoluteUrl(img.Path);
                }
                roomDto.Images = imagesDto;
            }
            
            return Ok(roomsDto);
        }

        // GET: api/Rooms/AvailableByDateRange - T?t c? có th? xem (cho Client tìm phòng)
        [HttpGet("AvailableByDateRange")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRoomsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] string? categoryId = null, 
            [FromQuery] int? maxPeople = null)
        {
            if (startDate >= endDate)
            {
                return BadRequest(new { message = "Ngày b?t ??u ph?i nh? h?n ngày k?t thúc" });
            }

            var rooms = await _unitOfWork.Rooms.GetAvailableRoomsByDateRangeAsync(startDate, endDate, categoryId, maxPeople);
            var roomsDto = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            
            // Load images cho t?ng phòng
            foreach (var roomDto in roomsDto)
            {
                var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
                var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
                foreach (var img in imagesDto)
                {
                    img.Path = ToAbsoluteUrl(img.Path);
                }
                roomDto.Images = imagesDto;
            }
            
            return Ok(roomsDto);
        }

        // POST: api/Rooms - Ch? Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
            
            // Load images
            var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
            var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
            foreach (var img in imagesDto)
            {
                img.Path = ToAbsoluteUrl(img.Path);
            }
            roomDto.Images = imagesDto;
            
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, roomDto);
        }

        // PUT: api/Rooms/5 - Ch? Admin
        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin")]
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

        // DELETE: api/Rooms/5 - Ch? Admin
        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin")]
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
