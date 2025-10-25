using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SweetHotel.API.DTOs.Category;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Repositories;

namespace SweetHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        // GET: api/Categories/ByMaxPeople/4
        [HttpGet("ByMaxPeople/{maxPeople}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesByMaxPeople(int maxPeople)
        {
            var categories = await _unitOfWork.Categories.GetByMaxPeopleAsync(maxPeople);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            _mapper.Map(updateCategoryDto, category);
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
