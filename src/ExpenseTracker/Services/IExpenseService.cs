using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Exceptions;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<ExpenseDto> AddExpenseAsync(CreateExpenseDto createExpenseDto);
    Task<IEnumerable<ExpenseDto>> GetAllExpensesAsync();
    Task<IEnumerable<ExpenseDto>> GetExpensesByCategoryAsync(string category);
    Task<decimal> GetTotalExpensesAsync();
    Task<decimal> GetTotalExpensesByCategoryAsync(string category);
    Task DeleteExpenseAsync(int id);
    Task<ExpenseDto?> GetExpenseByIdAsync(int id);
}