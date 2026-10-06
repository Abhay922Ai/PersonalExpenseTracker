using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.ViewModels
{
    public class IncomeViewModel
    {
        public int IncomeId { get; set; }

        [Required]
        public string Source { get; set; } = string.Empty;

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Income Date")]
        public DateTime IncomeDate { get; set; } = DateTime.Today;

        public string? Description { get; set; }
    }
}