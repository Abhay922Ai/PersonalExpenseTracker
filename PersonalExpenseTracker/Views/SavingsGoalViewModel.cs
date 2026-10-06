using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.ViewModels
{
    public class SavingsGoalViewModel
    {
        [Required]
        [Display(Name = "Goal Name")]
        public string GoalName { get; set; } = string.Empty;

        [Required]
        [Range(1, double.MaxValue)]
        [Display(Name = "Target Amount")]
        public decimal TargetAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Saved Amount")]
        public decimal SavedAmount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Target Date")]
        public DateTime? TargetDate { get; set; }
    }
}