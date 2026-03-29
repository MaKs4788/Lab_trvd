using System.ComponentModel.DataAnnotations;

namespace LabsTRVD.DTOs.AuthDTOs
{
    public class RevokeTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}