namespace PersonalExpenseTracker.Models
{
    public class SavingsGoal
    {
        public int GoalId { get; set; }

        public int UserId { get; set; }

        public string GoalName { get; set; } = string.Empty;

        public decimal TargetAmount { get; set; }

        public decimal SavedAmount { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}