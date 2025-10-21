# String Analyzer Service

A RESTful API service that analyzes text strings and stores their computed properties. Built with C# ASP.NET Core and SQLite.

## Features

- Analyze strings and compute multiple properties
- SHA-256 hashing for unique identification
- Palindrome detection (case-insensitive)
- Character frequency analysis
- Advanced filtering with query parameters
- Natural language query support
- Persistent storage with SQLite

## Tech Stack

- C# ASP.NET Core 8.0
- Entity Framework Core
- SQLite Database
- Swagger/OpenAPI for documentation

## Endpoints

### 1. Create/Analyze String
```
POST /strings
```
Analyzes and stores a new string with computed properties.

### 2. Get Specific String
```
GET /strings/{string_value}
```
Retrieves a previously analyzed string.

### 3. Get All Strings with Filtering
```
GET /strings?is_palindrome=true&min_length=5&max_length=20&word_count=2&contains_character=a
```
Returns all strings with optional filtering.

### 4. Natural Language Filtering
```
GET /strings/filter-by-natural-language?query=all%20single%20word%20palindromic%20strings
```
Filter strings using natural language queries.

### 5. Delete String
```
DELETE /strings/{string_value}
```
Removes a string from the database.

## Installation

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Setup Instructions

1. Clone the repository:
```bash
git clone https://github.com/yourusername/string-analyzer-service.git
cd string-analyzer-service
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

4. Access the API:
- Swagger UI: http://localhost:5000/swagger
- API Base: http://localhost:5000/strings

## Dependencies

- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Design
- Swashbuckle.AspNetCore (Swagger)

## Database

The application uses SQLite for data persistence. The database file `strings.db` is automatically created on first run.

## API Response Examples

### Create String
Request:
```json
{
  "value": "racecar"
}
```

Response (201 Created):
```json
{
  "id": "e00f9ef51a95f6e854862eed28dc0f1a68f154d9f75ddd841ab00de6ede9209b",
  "value": "racecar",
  "properties": {
    "length": 7,
    "is_palindrome": true,
    "unique_characters": 4,
    "word_count": 1,
    "sha256_hash": "e00f9ef51a95f6e854862eed28dc0f1a68f154d9f75ddd841ab00de6ede9209b",
    "character_frequency_map": {
      "r": 2,
      "a": 2,
      "c": 2,
      "e": 1
    }
  },
  "created_at": "2025-10-21T00:33:49.4248477Z"
}
```