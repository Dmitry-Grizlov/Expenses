using System.Globalization;
using Expenses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Controllers;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var startDate = DateTime.Today.AddDays(-6);
        var endDate = DateTime.Today;

        var transactions = await _context.Transactions
            .Include(x => x.Category)
            .Where(x => x.Date >= startDate && x.Date <= endDate)
            .ToListAsync();

        var totalIncome = transactions
            .Where(x => x.Category.Type == "Income")
            .Sum(x => x.Amount);
        ViewBag.TotalIncome = totalIncome.ToString("C0");

        var totalExpense = transactions
            .Where(x => x.Category.Type == "Expense")
            .Sum(x => x.Amount);
        ViewBag.TotalExpense = totalExpense.ToString("C0");

        var totalBalance = totalIncome - totalExpense;
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        culture.NumberFormat.CurrencyNegativePattern = 1;
        ViewBag.TotalBalance = string.Format(culture, "{0:C0}", totalBalance);

        ViewBag.DonutChartData = transactions
            .Where(x => x.Category.Type == "Expense")
            .GroupBy(x => x.CategoryId)
            .Select(x => new
            {
                Amount = x.Sum(y => y.Amount),
                FormattedAmount = x.Sum(y => y.Amount).ToString("C0"),
                Category = x.First()?.Category?.Icon + " " + x.First()?.Category?.Title
            })
            .OrderByDescending((x => x.Amount))
            .ToList();

        var incomeSummary = transactions
            .Where(x => x.Category?.Type == "Income")
            .GroupBy(x => x.Date)
            .Select(x => new SplineChartData()
            {
                Day = x.First().Date.ToString("dd-MM"),
                Income = x.Sum(y=>y.Amount)
            }).ToList();
        
        var expenseSummary = transactions
            .Where(x => x.Category?.Type == "Expense")
            .GroupBy(x => x.Date)
            .Select(x => new SplineChartData()
            {
                Day = x.First().Date.ToString("dd-MM"),
                Expense = x.Sum(y=>y.Amount)
            }).ToList();

        var days = Enumerable.Range(0, 7)
            .Select(x => startDate.AddDays(x).ToString("dd-MM")).ToArray();

        ViewBag.SplineChartData = from day in days
            join income in incomeSummary on day equals income.Day
                into dayIncomeJoined
            from income in dayIncomeJoined.DefaultIfEmpty()
            join expense in expenseSummary on day equals expense.Day
                into expenseJoined
            from expense in expenseJoined.DefaultIfEmpty()
            select new
            {
                Day = day,
                Income = income?.Income ?? 0,
                Expense = expense?.Expense ?? 0
            };

        ViewBag.RecentTransactions = await _context.Transactions
            .Include(x => x.Category)
            .OrderByDescending(x => x.Date)
            .Take(5)
            .ToListAsync();
        
        return View();
    }
}