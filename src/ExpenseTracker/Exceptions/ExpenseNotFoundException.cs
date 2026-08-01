namespace ExpenseTracker.Exceptions;

public class ExpenseNotFoundException : Exception
{
    public ExpenseNotFoundException(int id) : base($"Expense with Id {id} not found")
    {
    }
}