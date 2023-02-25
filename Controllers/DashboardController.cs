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
                Amount = x.Sum(x=>x.Amount),
                FormattedAmount = x.Sum(x=>x.Amount).ToString("C0"),
                Category = x.First()?.Category?.Icon + " " + x.First()?.Category?.Title
            }).ToList();

        return View();
    }
}