using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Extensions;
using LabsTRVD.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabsTRVD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private Guid CurrentUserId => User.GetUserId();

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDtoResponse>>> GetUserCategories()
        {
            var dtos = await _categoryService.GetUserCategoriesAsync(CurrentUserId);
            return Ok(dtos);
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDtoResponse>> GetCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id, CurrentUserId);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult> AddCategory([FromBody] CategoryDto dto)
        {
            try
            {
                var result = await _categoryService.AddCategoryAsync(dto, CurrentUserId);
                return CreatedAtAction(nameof(GetCategory), new { id = result.CategoryId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, dto, CurrentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id, CurrentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}