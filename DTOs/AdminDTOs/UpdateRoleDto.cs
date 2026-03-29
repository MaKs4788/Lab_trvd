using System.ComponentModel.DataAnnotations;

namespace LabsTRVD.DTOs.AdminDTOs
{
    public class UpdateRoleDto
    {
        [Required(ErrorMessage = "Роль обов'язкова")]
        [StringLength(50, ErrorMessage = "Роль не може бути довше 50 символів")]
        public string NewRole { get; set; } = string.Empty;
    }
}
