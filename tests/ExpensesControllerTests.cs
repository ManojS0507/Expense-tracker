using ExpenseTracker.Controllers;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Tests;

public class ExpensesControllerTests
{
    private readonly ExpensesController _controller;
    private readonly IExpenseService _expenseService;

    public ExpensesControllerTests()
    {
        _expenseService = new ExpenseService();
        _controller = new ExpensesController(_expenseService);
    }

    [Fact]
    public async Task GetExpenses_ReturnsAllExpenses()
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
        var result = await _controller.GetExpenses(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnDto = Assert.IsType<List<ExpenseDto>>(okResult.Value);
        Assert.Equal(2, returnDto.Count);
        Assert.Contains(returnDto, e => e.Title == "Lunch");
        Assert.Contains(returnDto, e => e.Title == "Bus");
    }

    [Fact]
    public async Task GetExpenses_ReturnsMatchingExpenses()
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
        var result = await _controller.GetExpenses("Food");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnDto = Assert.IsType<List<ExpenseDto>>(okResult.Value);
        Assert.Equal(2, returnDto.Count);
        Assert.All(returnDto, e => Assert.Equal("Food", e.Category, StringComparer.OrdinalIgnoreCase));
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
        var result = await _controller.GetTotalExpenses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        var value = okResult.Value;
        var prop = value.GetType().GetProperty("total");
        Assert.NotNull(prop);
        var total = (decimal)prop.GetValue(value);
        Assert.Equal(300m, total);
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
        var result = await _controller.GetTotalExpensesByCategory("Food");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        var value = okResult.Value;
        var categoryProp = value.GetType().GetProperty("category");
        var totalProp = value.GetType().GetProperty("total");
        Assert.NotNull(categoryProp);
        Assert.NotNull(totalProp);
        var category = (string)categoryProp.GetValue(value);
        var total = (decimal)totalProp.GetValue(value);
        Assert.Equal("Food", category);
        Assert.Equal(350m, total);
    }

    [Fact]
    public async Task DeleteExpense_ReturnsNoContent()
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
        var result = await _controller.DeleteExpense(expenseDto.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteExpense_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteExpense(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        var value = notFoundResult.Value;
        var prop = value.GetType().GetProperty("error");
        Assert.NotNull(prop);
        var error = (string)prop.GetValue(value);
        Assert.Equal($"Expense with Id 999 not found", error);
    }

    [Fact]
    public async Task CreateExpense_ReturnsCreatedAtAction()
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
        var result = await _controller.CreateExpense(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetExpenseById), createdResult.ActionName);
        var returnDto = Assert.IsType<ExpenseDto>(createdResult.Value);
        Assert.Equal(1, returnDto.Id);
        Assert.Equal("Lunch", returnDto.Title);
        Assert.Equal(250m, returnDto.Amount);
        Assert.Equal("Food", returnDto.Category);
        Assert.Equal(new DateOnly(2026, 7, 31), returnDto.Date);
    }

    [Fact]
    public async Task CreateExpense_ValidationFailure_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateExpenseDto
        {
            Title = "", // Title is required
            Amount = 0, // Amount must be greater than zero
            Category = "", // Category is required
            Date = new DateOnly() // Date is required (but DateOnly default is min value, which is not null, but we rely on [Required] attribute)
        };
        // Manually invalidate the model state to test validation
        _controller.ModelState.AddModelError("Title", "Title is required");
        _controller.ModelState.AddModelError("Amount", "Amount must be greater than zero");
        _controller.ModelState.AddModelError("Category", "Category is required");
        _controller.ModelState.AddModelError("Date", "Date is required");

        // Act
        var result = await _controller.CreateExpense(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        // The return value is a SerializableError dictionary
        var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(modelState.ContainsKey("Title"));
        Assert.True(modelState.ContainsKey("Amount"));
        Assert.True(modelState.ContainsKey("Category"));
        Assert.True(modelState.ContainsKey("Date"));
    }
}
