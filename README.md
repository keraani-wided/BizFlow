# BizFlow

A business management REST API built with ASP.NET Core 8 and PostgreSQL.

## Overview

BizFlow is a backend application designed to manage companies and their customers through a RESTful API. It uses a layered architecture to separate domain models, application logic, infrastructure, and API endpoints.

## Features

- Create and retrieve companies
- Create and retrieve customers
- List customers by company
- Update customer information
- Activate and deactivate customers
- Persist data in PostgreSQL
- Validate API endpoints with Swagger
- Unit tests for core functionality

## Tech Stack

- C# and .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker Compose
- Swagger / OpenAPI
- xUnit

## Architecture

The solution is organized into separate projects:

- `BizFlow.Domain` — domain entities and value objects
- `BizFlow.Application` — application layer
- `BizFlow.Infrastructure` — database context and migrations
- `BizFlow.API` — HTTP endpoints and API configuration
- `BizFlow.Tests` — automated tests

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker Desktop with Docker Compose
- Git

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/keraani-wided/BizFlow.git
   cd BizFlow
   ```

2. Configure the PostgreSQL password in a local `.env` file according to `docker-compose.yml`.

3. Start the database:

   ```bash
   docker compose up -d
   ```

4. Configure the API connection string using .NET User Secrets. From the repository root, run:

   ```powershell
   dotnet user-secrets init --project .\src\BizFlow.API
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=bizflow_db;Username=postgres;Password=YOUR_LOCAL_PASSWORD" --project .\src\BizFlow.API
   ```

   Replace `YOUR_LOCAL_PASSWORD` with your local PostgreSQL password. Never commit real credentials.

5. Build the solution:

   ```bash
   dotnet build BizFlow.sln
   ```

6. Run the tests:

   ```bash
   dotnet test BizFlow.sln
   ```

7. Start the API:

   ```bash
   dotnet run --project .\src\BizFlow.API
   ```

8. Open Swagger at `http://localhost:5032/swagger` if the API is running with the HTTP launch profile.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/companies` | Retrieve companies |
| POST | `/api/companies` | Create a company |
| POST | `/api/customers` | Create a customer |
| GET | `/api/customers?companyId={id}` | List customers for a company |
| GET | `/api/customers/{id}` | Retrieve a customer |
| PUT | `/api/customers/{id}` | Update a customer |
| PATCH | `/api/customers/{id}/deactivate` | Deactivate a customer |
| PATCH | `/api/customers/{id}/activate` | Activate a customer |

## Testing

Run the automated test suite with:

```bash
dotnet test BizFlow.sln
```

## Security

- Local environment variables and secrets should never be committed.
- The `.env` file is excluded through `.gitignore`.
- The API database connection string is configured using .NET User Secrets for local development.

## License

No license has been added yet. All rights are reserved unless a license is provided.

## Project Status

**Status:** In progress.

The current version provides company and customer management endpoints, PostgreSQL persistence, Swagger documentation, and automated tests.

### Roadmap

Planned improvements:

- [ ] Implement application use cases and CQRS with MediatR.
- [ ] Add JWT-based authentication and authorization.
- [ ] Enforce company-level data isolation and access control.
- [ ] Add GitHub Actions CI for automated builds and tests.
- [ ] Improve API validation and integration test coverage.

These items are planned and are not yet represented as completed features.
