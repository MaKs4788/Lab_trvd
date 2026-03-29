using System;

namespace LabsTRVD.DTOs.ServicesDTOs
{
    public class CategoryDtoResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
