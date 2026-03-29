using System;
using System.ComponentModel.DataAnnotations;

namespace LabsTRVD.DTOs.ServicesDTOs
{
    public class ExpenseDto
    {
        [Required(ErrorMessage = "Сума обов'язкова")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сума повинна бути більше 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Дата обов'язкова")]
        public DateTime Date { get; set; }
        public int? CategoryId { get; set; }

        [StringLength(250, ErrorMessage = "Опис не може бути довше 250 символів")]
        public string? Description { get; set; }
    }
}