using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.ViewModels
{
    public class SavingsGoalViewModel
    {
        [Required(ErrorMessage = "Goal name is required")]
        [Display(Name = "Goal Name")]
        public string GoalName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        [Display(Name = "Target Amount")]
        public decimal TargetAmount { get; set; }

        [Required(ErrorMessage = "Saved amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Saved amount cannot be negative")]
        [Display(Name = "Saved Amount")]
        public decimal SavedAmount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime? TargetDate { get; set; }
    }
}
