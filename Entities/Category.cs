namespace LabsTRVD.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
