using System.ComponentModel.DataAnnotations;

namespace LabsTRVD.DTOs.AuthDTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Пароль повинен містити мінімум 8 символів")]
        public string Password { get; set; } = string.Empty;
    }
}
