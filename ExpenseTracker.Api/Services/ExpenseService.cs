using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseResponseDto>> GetExpensesAsync(int userId, DateTime? startDate, DateTime? endDate, int page, int pageSize);
    Task<ExpenseResponseDto?> CreateExpenseAsync(int userId, CreateExpenseDto createExpenseDto);
    Task<bool> DeleteExpenseAsync(int userId, int expenseId);
    Task<ExpenseSummaryDto> GetSummaryAsync(int userId);
}

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ExpenseResponseDto>> GetExpensesAsync(int userId, DateTime? startDate, DateTime? endDate, int page, int pageSize)
    {
        var query = _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new ExpenseResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                Date = e.Date,
                CategoryName = e.Category != null ? e.Category.Name : "Uncategorized"
            })
            .ToListAsync();

        return expenses;
    }

    public async Task<ExpenseResponseDto?> CreateExpenseAsync(int userId, CreateExpenseDto createExpenseDto)
    {
        var expense = new Expense
        {
            Title = createExpenseDto.Title,
            Amount = createExpenseDto.Amount,
            Date = createExpenseDto.Date,
            Description = createExpenseDto.Description,
            CategoryId = createExpenseDto.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        var savedExpense = await _context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == expense.Id);

        return new ExpenseResponseDto
        {
            Id = savedExpense!.Id,
            Title = savedExpense.Title,
            Amount = savedExpense.Amount,
            Date = savedExpense.Date,
            CategoryName = savedExpense.Category?.Name ?? "Uncategorized"
        };
    }

    public async Task<bool> DeleteExpenseAsync(int userId, int expenseId)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
        if (expense == null) return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ExpenseSummaryDto> GetSummaryAsync(int userId)
    {
        var expenses = await _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var totalAmount = expenses.Sum(e => e.Amount);
        var totalThisMonth = expenses.Where(e => e.Date >= startOfMonth).Sum(e => e.Amount);

        var categoryBreakdown = expenses
            .GroupBy(e => e.Category?.Name ?? "Uncategorized")
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        var highestCategory = categoryBreakdown.Any()
            ? categoryBreakdown.OrderByDescending(x => x.Value).First().Key
            : "N/A";

        return new ExpenseSummaryDto
        {
            TotalAmount = totalAmount,
            TotalThisMonth = totalThisMonth,
            HighestCategory = highestCategory,
            CategoryBreakdown = categoryBreakdown
        };
    }
}
