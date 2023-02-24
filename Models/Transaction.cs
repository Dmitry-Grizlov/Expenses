using System.ComponentModel.DataAnnotations;

namespace Expenses.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int CategoryId { get; set; }

        public Category Category {get; set;}
        
        public int Amount { get; set; } 

        [MaxLength(75)]
        public string Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}