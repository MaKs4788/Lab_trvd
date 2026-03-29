namespace LabsTRVD.Entities
{
    public class Expense
    {
        public int ExpenseId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
        public User User { get; set; } = null!;
        public Category? Category { get; set; }
    }
}
