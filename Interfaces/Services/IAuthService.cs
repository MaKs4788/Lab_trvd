using LabsTRVD.DTOs.AuthDTOs;

namespace LabsTRVD.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> SignInAsync(SignInDto dto);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto); 
        Task RevokeTokenAsync(RevokeTokenRequestDto dto);
    }
}
