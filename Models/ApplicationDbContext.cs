using Microsoft.EntityFrameworkCore;

namespace Expenses.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Transaction> Transactions {get; set;}        
        public DbSet<Category> Categories {get; set;}        

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}