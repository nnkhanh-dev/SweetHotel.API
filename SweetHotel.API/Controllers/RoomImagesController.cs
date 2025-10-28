using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.RoomImages;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;
using System.IO;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomImagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public RoomImagesController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        private string ToAbsoluteUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return relativePath;
            // If it's already an absolute URL, return as-is
            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute))
                return relativePath;

            var scheme = Request?.Scheme ?? "https";
            var host = Request?.Host.Value ?? string.Empty;
            var basePath = Request?.PathBase.HasValue == true ? Request.PathBase.Value : string.Empty;
            return $"{scheme}://{host}{basePath}{relativePath}";
        }

        // GET: api/RoomImages - T?t c? có th? xem
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomImageDto>>> GetRoomImages()
        {
            var images = await _unitOfWork.RoomImages.GetAllAsync();
            var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
            foreach (var img in imagesDto)
            {
                img.Path = ToAbsoluteUrl(img.Path);
            }
            return Ok(imagesDto);
        }

        // GET: api/RoomImages/{id} - T?t c? có th? xem
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RoomImageDto>> GetRoomImage(string id)
        {
            var image = await _unitOfWork.RoomImages.GetByIdAsync(id);
            if (image == null)
                return NotFound(new { message = "Room image not found" });

            var imageDto = _mapper.Map<RoomImageDto>(image);
            imageDto.Path = ToAbsoluteUrl(imageDto.Path);
            return Ok(imageDto);
        }

        // GET: api/RoomImages/ByRoom/{roomId} - T?t c? có th? xem
        [HttpGet("ByRoom/{roomId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomImageDto>>> GetImagesByRoom(string roomId)
        {
            var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomId);
            var imagesDto = _mapper.Map<List<RoomImageDto>>(images);
            foreach (var img in imagesDto)
            {
                img.Path = ToAbsoluteUrl(img.Path);
            }
            return Ok(imagesDto);
        }

        // POST: api/RoomImages - create by path
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomImageDto>> CreateRoomImage([FromBody] CreateRoomImageDto createRoomImageDto)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(createRoomImageDto.RoomId);
            if (room == null)
                return BadRequest(new { message = "Room not found" });

            var image = _mapper.Map<RoomImages>(createRoomImageDto);
            await _unitOfWork.RoomImages.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            var imageDto = _mapper.Map<RoomImageDto>(image);
            imageDto.Path = ToAbsoluteUrl(imageDto.Path);
            return CreatedAtAction(nameof(GetRoomImage), new { id = image.Id }, imageDto);
        }

        // POST: api/RoomImages/Upload - upload files
        // Form-data: roomId (string), files (IFormFile[])
        [HttpPost("Upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadRoomImages([FromForm] string roomId, [FromForm] List<IFormFile> files)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return BadRequest(new { message = "RoomId is required" });

            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
                return BadRequest(new { message = "Room not found" });

            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files uploaded" });

            var savedImages = new List<RoomImages>();

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsRoot = Path.Combine(webRoot, "uploads", "rooms", roomId);
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;

                var ext = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeWebPath = $"/uploads/rooms/{roomId}/{fileName}";
                // create absolute url for storing/returning so FE can access directly
                var absoluteUrl = Request != null ?
                    $"{Request.Scheme}://{Request.Host}{Request.PathBase}{relativeWebPath}" : relativeWebPath;

                var roomImage = new RoomImages
                {
                    Id = Guid.NewGuid().ToString(),
                    Path = absoluteUrl,
                    RoomId = roomId
                };

                savedImages.Add(roomImage);
            }

            if (savedImages.Any())
            {
                await _unitOfWork.RoomImages.AddRangeAsync(savedImages);
                await _unitOfWork.SaveChangesAsync();
            }

            var result = _mapper.Map<List<RoomImageDto>>(savedImages);
            // ensure returned paths are absolute
            for (int i = 0; i < result.Count; i++)
            {
                result[i].Path = ToAbsoluteUrl(result[i].Path);
            }

            return Ok(result);
        }

        // POST: api/RoomImages/Update/{id}
        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoomImage(string id, [FromBody] UpdateRoomImageDto updateRoomImageDto)
        {
            var image = await _unitOfWork.RoomImages.GetByIdAsync(id);
            if (image == null)
                return NotFound(new { message = "Room image not found" });

            _mapper.Map(updateRoomImageDto, image);
            _unitOfWork.RoomImages.Update(image);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/RoomImages/Delete/{id}
        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomImage(string id)
        {
            var image = await _unitOfWork.RoomImages.GetByIdAsync(id);
            if (image == null)
                return NotFound(new { message = "Room image not found" });

            // try delete file from disk
            try
            {
                string relativePath = image.Path;
                if (Uri.IsWellFormedUriString(image.Path, UriKind.Absolute))
                {
                    var uri = new Uri(image.Path);
                    relativePath = uri.AbsolutePath; // starts with '/'
                }

                var physicalPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            catch
            {
                // ignore file delete errors
            }

            _unitOfWork.RoomImages.Remove(image);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
