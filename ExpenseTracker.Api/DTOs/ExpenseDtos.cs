namespace ExpenseTracker.Api.DTOs;

public class CreateExpenseDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateExpenseDto
{
    public string? Title { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
}

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class ExpenseSummaryDto
{
    public decimal TotalAmount { get; set; }
    public decimal TotalThisMonth { get; set; }
    public string HighestCategory { get; set; } = string.Empty;
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
}
