namespace PersonalExpenseTracker.Models
{
    public class Income
    {
        public int IncomeId { get; set; }

        public int UserId { get; set; }

        public string Source { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime IncomeDate { get; set; }

        public string? Description { get; set; }
    }
}