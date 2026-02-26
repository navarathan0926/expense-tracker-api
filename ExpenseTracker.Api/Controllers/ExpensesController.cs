using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponseDto>>> GetExpenses(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var expenses = await _expenseService.GetExpensesAsync(userId, startDate, endDate, page, pageSize);
        return Ok(expenses);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryDto>> GetSummary()
    {
        var userId = GetUserId();
        var summary = await _expenseService.GetSummaryAsync(userId);
        return Ok(summary);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponseDto>> CreateExpense(CreateExpenseDto createExpenseDto)
    {
        var userId = GetUserId();
        var expense = await _expenseService.CreateExpenseAsync(userId, createExpenseDto);
        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var userId = GetUserId();
        var result = await _expenseService.DeleteExpenseAsync(userId, id);
        if (!result) return NotFound();
        return NoContent();
    }
}
