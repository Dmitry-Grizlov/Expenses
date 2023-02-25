using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expenses.Models
{
    public class Transaction
    {
        [Key] public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please choose a category")] public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount must be grater than 0")]public int Amount { get; set; }

        [MaxLength(75)] public string Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string CategoryTitle => Category == null ? string.Empty : Category.Icon + " " + Category.Title;
        
        [NotMapped]
        public string FormattedAmount => ((Category == null || Category.Type == "Expense" )? "- " : "+ ") + Amount.ToString("C0");
    }
}