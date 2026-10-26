using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.ViewModels
{
    public class IncomeViewModel
    {
        public int IncomeId { get; set; }

        [Required(ErrorMessage = "Source is required")]
        public string Source { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Income date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Income Date")]
        public DateTime IncomeDate { get; set; } = DateTime.Today;

        public string? Description { get; set; }
    }
}
