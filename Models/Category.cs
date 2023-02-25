using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expenses.Models
{
    public class Category
    {
        [Key] public int CategoryId { get; set; }

        [MaxLength(50)] public string Title { get; set; }

        [MaxLength(5)] public string Icon { get; set; }

        [MaxLength(10)] public string Type { get; set; } = "Expense";

        [NotMapped]
        public string IconWithTitle
        {
            get
            {
                return Icon + " " + Title;
            }
        }
    }
}