using ExpenseTracker.DTOs;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Exceptions;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    // POST api/expenses
    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto createExpenseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var expenseDto = await _expenseService.AddExpenseAsync(createExpenseDto);
        return CreatedAtAction(nameof(GetExpenseById), new { id = expenseDto.Id }, expenseDto);
    }

    // GET api/expenses
    // GET api/expenses?category=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses([FromQuery] string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }
        else
        {
            var expenses = await _expenseService.GetExpensesByCategoryAsync(category);
            return Ok(expenses);
        }
    }

    // GET api/expenses/total
    [HttpGet("total")]
    public async Task<ActionResult<object>> GetTotalExpenses()
    {
        var total = await _expenseService.GetTotalExpensesAsync();
        return Ok(new { total });
    }

    // GET api/expenses/total/{category}
    [HttpGet("total/{category}")]
    public async Task<ActionResult<object>> GetTotalExpensesByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return BadRequest(new { error = "Category is required" });
        }

        var total = await _expenseService.GetTotalExpensesByCategoryAsync(category);
        return Ok(new { category, total });
    }

    // DELETE api/expenses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        try
        {
            await _expenseService.DeleteExpenseAsync(id);
            return NoContent();
        }
        catch (ExpenseNotFoundException)
        {
            return NotFound(new { error = $"Expense with Id {id} not found" });
        }
    }

    // Optional: GET api/expenses/{id} - not required but useful for CreatedAtAction
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpenseById(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound(new { error = $"Expense with Id {id} not found" });
        }

        return Ok(expense);
    }
}