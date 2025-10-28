using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.User;
using SweetHotel.API.Entities.Entities;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/User - Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var users = _userManager.Users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar
            }).ToList();

            return Ok(users);
        }

        // GET: api/User/{id} - Admin or owner
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id) return Forbid();

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Roles = roles.ToList()
            };

            return Ok(dto);
        }

        // PUT: api/User/{id} - owner or admin
        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto update)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id) return Forbid();

            if (!string.IsNullOrEmpty(update.FullName)) user.FullName = update.FullName;
            if (!string.IsNullOrEmpty(update.PhoneNumber)) user.PhoneNumber = update.PhoneNumber;
            if (!string.IsNullOrEmpty(update.Avatar)) user.Avatar = update.Avatar;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }

        // POST: api/User/{id}/ChangePassword - owner only
        [HttpPost("{id}/ChangePassword")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDto dto)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id) return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }

        // POST: api/User/{id}/Roles - Admin assign roles
        [HttpPost("{id}/Roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoles(string id, [FromBody] RolesDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var remove = userRoles.Except(dto.Roles);
            var add = dto.Roles.Except(userRoles);

            if (remove.Any())
            {
                var r = await _userManager.RemoveFromRolesAsync(user, remove);
                if (!r.Succeeded) return BadRequest(r.Errors);
            }

            foreach (var role in add)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (add.Any())
            {
                var a = await _userManager.AddToRolesAsync(user, add);
                if (!a.Succeeded) return BadRequest(a.Errors);
            }

            return NoContent();
        }

        [HttpPost("{id}/UploadAvatar")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> UploadAvatar(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id)
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Xác định đường dẫn thư mục uploads/avatars
            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadPath = Path.Combine(webRoot, "uploads", "avatars");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Tạo tên file ngẫu nhiên
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo URL tuyệt đối cho ảnh
            var relativePath = $"/uploads/avatars/{fileName}";
            var absoluteUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{relativePath}";

            // Cập nhật Avatar cho user
            user.Avatar = absoluteUrl;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "Avatar uploaded successfully",
                avatarUrl = absoluteUrl
            });
        }

    }
}
