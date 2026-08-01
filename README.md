# Smart Expense Tracker API

A RESTful API for managing personal expenses, built with .NET 8 ASP.NET Core Web API. This project serves as a take-home assignment for a Software Engineering Apprenticeship.

## Features


<tool_call>
<function=Write>
<parameter=content>
# Smart Expense Tracker API

A RESTful API for managing personal expenses, built with .NET 8 ASP.NET Core Web API. This project serves as a take-home assignment for a Software Engineering Apprenticeship.

## Features

- Create, read, update, and delete expenses
- Filter expenses by category
- Calculate total expenses
- Calculate total expenses by category
- Input validation using Data Annotations
- Global error handling
- Swagger/OpenAPI documentation
- In-memory storage (data persists only while the application is running)

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 8 ASP.NET Core Web API
- **Testing**: xUnit
- **Documentation**: Swagger/OpenAPI
- **Validation**: System.ComponentModel.DataAnnotations

## Folder Structure

```
expense-tracker/
│
├── README.md
├── AI_NOTES.md
├── src/
│   ├── Controllers/
│   │   └── ExpensesController.cs
│   ├── Services/
│   │   ├── IExpenseService.cs
│   │   └── ExpenseService.cs
│   ├── Models/
│   │   └── Expense.cs
│   ├── DTOs/
│   │   ├── ExpenseDto.cs
│   │   └── CreateExpenseDto.cs
│   ├── Exceptions/
│   │   └── ExpenseNotFoundException.cs
│   ├── Middleware/
│   │   └── GlobalExceptionHandlerMiddleware.cs
│   ├── Program.cs
│   └── ExpenseTracker.csproj
│
└── tests/
    └── ExpenseTracker.Tests/
        ├── ExpenseServiceTests.cs
        ├── ExpensesControllerTests.cs
        └── ExpenseTracker.Tests.csproj
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Installation

1. Clone the repository
2. Navigate to the project directory:
   ```bash
   cd expense-tracker
   ```

## Build

To build the solution, run:
```bash
dotnet build
```

## Run

To run the API, execute:
```bash
dotnet run --project src/ExpenseTracker.csproj
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000` if HTTPS is not configured).

## Testing

To run the unit tests, execute:
```bash
dotnet test
```

## Swagger UI

Once the application is running, navigate to:
```
https://localhost:5001/swagger
```
to view and interact with the API documentation.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/expenses` | Create a new expense |
| GET | `/api/expenses` | Retrieve all expenses |
| GET | `/api/expenses?category={category}` | Retrieve expenses by category |
| GET | `/api/expenses/total` | Get the total of all expenses |
| GET | `/api/expenses/total/{category}` | Get the total expenses for a specific category |
| DELETE | `/api/expenses/{id}` | Delete an expense by ID |
| GET | `/api/expenses/{id}` | Get an expense by ID (optional, for utility) |

## Example Requests and Responses

### Create an Expense

**Request**
```http
POST /api/expenses
Content-Type: application/json

{
  "title": "Lunch",
  "amount": 25.50,
  "category": "Food",
  "date": "2026-07-31"
}
```

**Response**
```http
HTTP/1.1 201 Created
Location: /api/expenses/1
Content-Type: application/json

{
  "id": 1,
  "title": "Lunch",
  "amount": 25.5,
  "category": "Food",
  "date": "2026-07-31"
}
```

### Get All Expenses

**Request**
```http
GET /api/expenses
```

**Response**
```http
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "id": 1,
    "title": "Lunch",
    "amount": 25.5,
    "category": "Food",
    "date": "2026-07-31"
  }
]
```

### Get Total Expenses

**Request**
```http
GET /api/expenses/total
```

**Response**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "total": 25.5
}
```

### Get Total Expenses by Category

**Request**
```http
GET /api/expenses/total/Food
```

**Response**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "category": "Food",
  "total": 25.5
}
```

### Delete an Expense

**Request**
```http
DELETE /api/expenses/1
```

**Response**
```http
HTTP/1.1 204 No Content
```

## Error Handling

The API returns standardized JSON error responses:

- **400 Bad Request**: Validation errors
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Unexpected errors

Example error response:
```json
{
  "error": "Expense with Id 999 not found"
}
```

## Notes

- This implementation uses in-memory storage, so data is lost when the application stops.
- The API follows RESTful principles and uses appropriate HTTP status codes.
- All input is validated using Data Annotation attributes.

## License

This project is for educational purposes only.