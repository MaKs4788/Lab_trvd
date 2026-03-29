using LabsTRVD.DTOs.AdminDTOs;
using LabsTRVD.Interfaces.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabsTRVD.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>Отримати список всіх користувачів</summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<UserAdminDtoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserAdminDtoResponse>>> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>Отримати користувача за ID</summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(UserAdminDtoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserAdminDtoResponse>> GetUserById(Guid userId)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(new { message = "Користувач не знайдений" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>Заблокувати користувача</summary>
        [HttpPut("users/{userId}/block")]
        [ProducesResponseType(typeof(UserAdminDtoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserAdminDtoResponse>> BlockUser(Guid userId)
        {
            try
            {
                var user = await _adminService.BlockUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Розблокувати користувача</summary>
        [HttpPut("users/{userId}/unblock")]
        [ProducesResponseType(typeof(UserAdminDtoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserAdminDtoResponse>> UnblockUser(Guid userId)
        {
            try
            {
                var user = await _adminService.UnblockUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Видалити користувача</summary>
        [HttpDelete("users/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _adminService.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Змінити роль користувача</summary>
        [HttpPut("users/{userId}/role")]
        [ProducesResponseType(typeof(UserAdminDtoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserAdminDtoResponse>> UpdateUserRole(Guid userId, [FromBody] UpdateRoleDto dto)
        {
            try
            {
                var user = await _adminService.UpdateUserRoleAsync(userId, dto.NewRole);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
