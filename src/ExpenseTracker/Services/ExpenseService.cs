using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Exceptions;

namespace ExpenseTracker.Services;

public class ExpenseService : IExpenseService
{
    private int _nextId = 1;
    private readonly Dictionary<int, Expense> _expenses = new();

    public async Task<ExpenseDto> AddExpenseAsync(CreateExpenseDto createExpenseDto)
    {
        var expense = new Expense
        {
            Id = _nextId++,
            Title = createExpenseDto.Title,
            Amount = createExpenseDto.Amount,
            Category = createExpenseDto.Category,
            Date = createExpenseDto.Date
        };

        _expenses[expense.Id] = expense;

        return new ExpenseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date
        };
    }

    public async Task<IEnumerable<ExpenseDto>> GetAllExpensesAsync()
    {
        return _expenses.Values
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                Category = e.Category,
                Date = e.Date
            })
            .ToList();
    }

    public async Task<IEnumerable<ExpenseDto>> GetExpensesByCategoryAsync(string category)
    {
        return _expenses.Values
            .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                Category = e.Category,
                Date = e.Date
            })
            .ToList();
    }

    public async Task<decimal> GetTotalExpensesAsync()
    {
        return _expenses.Values.Sum(e => e.Amount);
    }

    public async Task<decimal> GetTotalExpensesByCategoryAsync(string category)
    {
        return _expenses.Values
            .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .Sum(e => e.Amount);
    }

    public async Task DeleteExpenseAsync(int id)
    {
        if (!_expenses.ContainsKey(id))
        {
            throw new ExpenseNotFoundException(id);
        }

        _expenses.Remove(id);
        await Task.CompletedTask;
    }

    public async Task<ExpenseDto?> GetExpenseByIdAsync(int id)
    {
        if (!_expenses.TryGetValue(id, out var expense))
        {
            return null;
        }

        return new ExpenseDto
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date
        };
    }
}