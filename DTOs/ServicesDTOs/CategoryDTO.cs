using System;
using System.ComponentModel.DataAnnotations;

namespace LabsTRVD.DTOs.ServicesDTOs
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва категорії не може бути довше 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
}