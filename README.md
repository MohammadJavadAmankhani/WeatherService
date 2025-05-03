# Weather Service API

A simple and efficient RESTful API service that fetches weather data from OpenMeteo API, stores it in SQL Server, and returns it unchanged to the client.

## Key Features

- Fetches data from OpenMeteo API
- Stores data in SQL Server
- Returns data unchanged to the client
- Error handling with fallback to stored data
- Maximum response time of 5 seconds
- Uses caching for improved performance

## Project Structure

```
WeatherService/
├── WeatherService.API/          # API layer and controllers
├── WeatherService.Application/  # Application services and business logic
├── WeatherService.Core/         # Domain models and interfaces
├── WeatherService.Infrastructure/ # Infrastructure implementations (database and external API)
└── WeatherService.Tests/        # Unit tests and test infrastructure
```

## Prerequisites

- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or Visual Studio Code

## Getting Started

1. **Database Setup**
   - Ensure SQL Server is installed
   - Update connection string in `appsettings.json`

2. **Run the Application**
   ```bash
   dotnet run --project WeatherService.API
   ```

## API Endpoints

- `GET /api/weathers?latitude={latitude}&longitude={longitude}`
  - Get weather information for specified coordinates
  - Parameters:
    - `latitude`: Geographic latitude (default: 52.52)
    - `longitude`: Geographic longitude (default: 13.41)

## Application Behavior

1. **Fetching New Data**:
   - Attempts to fetch data from OpenMeteo API
   - Stores data in database if successful
   - Returns data to client

2. **Error Handling**:
   - If new data fetch fails, returns last stored data
   - If no stored data exists, returns `null`
   - Maximum wait time of 5 seconds

3. **Caching**:
   - Uses Memory Cache for performance optimization
   - Cache duration: 1 hour

## Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WeatherService;Trusted_Connection=True;"
  }
}
```

## Important Notes

- Application is designed to work in an environment with 1% noise
- Data is returned to the client unchanged
- Application can operate autonomously for extended periods
- Follows the KISS (Keep It Simple, Stupid) principle