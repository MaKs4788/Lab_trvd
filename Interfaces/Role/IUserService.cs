using LabsTRVD.DTOs.AuthDTOs;
using LabsTRVD.Entities;

namespace LabsTRVD.Interfaces.Role
{
    public interface IUserService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
    }
}
