namespace LabsTRVD.Entities
{
    public class Income
    {
        public int IncomeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public User User { get; set; } = null!;
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
