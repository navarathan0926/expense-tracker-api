using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Api.Entities;

public class Expense
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Category? Category { get; set; }
    public User? User { get; set; }
}
