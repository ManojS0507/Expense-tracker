using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.Exceptions;
using Xunit;

namespace ExpenseTracker.Tests;

public class ExpenseServiceTests
{
    private readonly IExpenseService _expenseService;

    public ExpenseServiceTests()
    {
        // Each test gets a new instance of the service with its own in-memory storage
        _expenseService = new ExpenseService();
    }

    [Fact]
    public async Task AddExpense_ShouldReturnCreatedExpense()
    {
        // Arrange
        var createDto = new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 250m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 31)
        };

        // Act
        var result = await _expenseService.AddExpenseAsync(createDto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Lunch", result.Title);
        Assert.Equal(250m, result.Amount);
        Assert.Equal("Food", result.Category);
        Assert.Equal(new DateOnly(2026, 7, 31), result.Date);
    }

    [Fact]
    public async Task GetAllExpenses_ReturnsAllExpenses()
    {
        // Arrange
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 250m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 31)
        });
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Bus",
            Amount = 50m,
            Category = "Transport",
            Date = new DateOnly(2026, 7, 31)
        });

        // Act
        var result = await _expenseService.GetAllExpensesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.Title == "Lunch");
        Assert.Contains(result, e => e.Title == "Bus");
    }

    [Fact]
    public async Task GetExpensesByCategory_ReturnsMatchingExpenses()
    {
        // Arrange
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 250m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 31)
        });
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Groceries",
            Amount = 100m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 30)
        });
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Bus",
            Amount = 50m,
            Category = "Transport",
            Date = new DateOnly(2026, 7, 31)
        });

        // Act
        var result = await _expenseService.GetExpensesByCategoryAsync("Food");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, e => Assert.Equal("Food", e.Category, StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetTotalExpenses_ReturnsSumOfAllAmounts()
    {
        // Arrange
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 250m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 31)
        });
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Bus",
            Amount = 50m,
            Category = "Transport",
            Date = new DateOnly(2026, 7, 31)
        });

        // Act
        var result = await _expenseService.GetTotalExpensesAsync();

        // Assert
        Assert.Equal(300m, result);
    }

    [Fact]
    public async Task GetTotalExpensesByCategory_ReturnsSumForCategory()
    {
        // Arrange
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 250m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 31)
        });
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Groceries",
            Amount = 100m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 30)
        });
        await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Bus",
            Amount = 50m,
            Category = "Transport",
            Date = new DateOnly(2026, 7, 31)
        });

        // Act
        var result = await _expenseService.GetTotalExpensesByCategoryAsync("Food");

        // Assert
        Assert.Equal(350m, result);
    }

    [Fact]
    public async Task DeleteExpense_RemovesExpense()
    {
        // Arrange
        var expenseDto = await _expenseService.AddExpenseAsync(new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 250m,
            Category = "Food",
            Date = new DateOnly(2026, 7, 31)
        });

        // Act
        await _expenseService.DeleteExpenseAsync(expenseDto.Id);

        // Assert
        var allExpenses = await _expenseService.GetAllExpensesAsync();
        Assert.DoesNotContain(allExpenses, e => e.Id == expenseDto.Id);
    }

    [Fact]
    public async Task DeleteExpense_WhenNotFound_ThrowsExpenseNotFoundException()
    {
        // Act
        var exception = await Assert.ThrowsAsync<ExpenseNotFoundException>(async () =>
            await _expenseService.DeleteExpenseAsync(999));

        // Assert
        Assert.Contains("Expense with Id 999 not found", exception.Message);
    }

    [Fact]
    public async Task GetExpenseById_ReturnsNullWhenNotFound()
    {
        // Act
        var result = await _expenseService.GetExpenseByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
