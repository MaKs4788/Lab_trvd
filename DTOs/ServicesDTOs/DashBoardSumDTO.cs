namespace LabsTRVD.DTOs.ServicesDTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;

        public decimal CurrentMonthIncome { get; set; }
        public decimal CurrentMonthExpense { get; set; }
        public decimal CurrentMonthBalance => CurrentMonthIncome - CurrentMonthExpense;
    }
}
