# marpo-pest-control-api

Marpo Pest Control API is a .NET 10 ASP.NET Core Web API for managing pest control technicians and service appointments.

## Stack

- .NET 10 / C#
- ASP.NET Core minimal APIs
- Entity Framework Core
- SQLite

## Run locally

```bash
dotnet restore Marpo.PestControl.slnx
dotnet run --project Marpo.PestControl.Api/Marpo.PestControl.Api.csproj
```

The application creates a SQLite database on first run and seeds:

- 3 pest control technicians
- 3 service appointments

## API endpoints

- `GET /api/technicians`
- `GET /api/appointments`
- `GET /api/appointments/{id}`
- `POST /api/appointments`