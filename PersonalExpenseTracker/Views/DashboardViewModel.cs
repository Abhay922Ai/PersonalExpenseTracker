namespace PersonalExpenseTracker.ViewModels
{
    public class DashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public decimal TotalIncome { get; set; }

        public decimal TotalExpenses { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalSavings { get; set; }

        public decimal SavingsTarget { get; set; }

        public string SavingsGoalName { get; set; } = string.Empty;

        public decimal SavingsPercentage
        {
            get
            {
                if (SavingsTarget <= 0)
                    return 0;

                return (TotalSavings / SavingsTarget) * 100;
            }
        }
    }
}