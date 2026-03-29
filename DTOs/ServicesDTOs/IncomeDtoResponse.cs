using System;

namespace LabsTRVD.DTOs.ServicesDTOs
{
    public class IncomeDtoResponse
    {
        public int IncomeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
    }
}
