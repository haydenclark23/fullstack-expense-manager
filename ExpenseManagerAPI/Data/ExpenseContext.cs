using Microsoft.EntityFrameworkCore;
using ExpenseManagerAPI.Models;

namespace ExpenseManagerAPI.Data
{
    public class ExpenseContext : DbContext
    {
        public ExpenseContext(DbContextOptions<ExpenseContext> options) : base(options)
        {
        }
        public DbSet<Expense> Expenses => Set<Expense>();
    }
}