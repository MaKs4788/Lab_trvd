using LabsTRVD.DTOs.AdminDTOs;

namespace LabsTRVD.Interfaces.Role
{
    public interface IAdminService
    {
        Task<IEnumerable<UserAdminDtoResponse>> GetAllUsersAsync();
        Task<UserAdminDtoResponse?> GetUserByIdAsync(Guid userId);
        Task<UserAdminDtoResponse> BlockUserAsync(Guid userId);
        Task<UserAdminDtoResponse> UnblockUserAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
        Task<UserAdminDtoResponse> UpdateUserRoleAsync(Guid userId, string newRole);
    }
}
