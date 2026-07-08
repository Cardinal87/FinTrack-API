# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Technology Stack

- .NET 9 Web API
- CQRS pattern with MediatR
- Entity Framework Core with PostgreSQL
- JWT Authentication & Authorization
- Serilog for logging with Grafana Loki
- OpenTelemetry for metrics with Prometheus
- Swagger/OpenAPI for documentation

## Project Structure

- `FinTrack.API`: Main API layer with controllers
- `FinTrack.API.Application`: Business logic (commands/queries/handlers)
- `FinTrack.API.Core`: Domain entities and interfaces
- `FinTrack.API.Infrastructure`: Data access, repositories, identity services
- `FinTrack.Tests`: Unit tests
- `FinTrack.IntegrationTests`: Integration tests

## Build/Run Commands

```bash
# Run with docker-compose (recommended)
docker-compose --env-file path/to/.env up -d

# Build from source
cd FinTrack/FinTrack.API
dotnet build
dotnet run

# Run tests
dotnet test  # runs both unit and integration tests
dotnet test FinTrack.Tests/  # only unit tests
dotnet test FinTrack.IntegrationTests/  # only integration tests
```

## Configuration Requirements

- Requires `.env` file with environment variables (POSTGRES_HOST, etc.)
- Connection strings use template replacements: {DB_HOST}, {DB_USER}, etc.
- JWT tokens validated against Hashicorp Vault signing keys
- Custom password hasher using PBKDF2

## Code Patterns

- Use `Result` and `ValueResult<T>` for consistent return types with success/failure states
- Controllers inherit from base classes (`FinTrackContollerBase`, `AuthorizeFinTrackControllerBase`)
- CQRS pattern: Commands for writes, Queries for reads, Handlers for business logic
- Repository pattern for data access
- Custom validation in domain entities
- User ID extracted from JWT claims using `UserId` property in authorized controllers

## Testing Approach

- Unit tests in `FinTrack.Tests` using xUnit and FluentAssertions
- Integration tests in `FinTrack.IntegrationTests` using WebApplicationFactory
- Mock repositories in `FinTrack.API.TestMocks`
- Tests use FluentAssertions for assertions

## Authorization

- Admin role required for some endpoints: `Core.Common.UserRoles.Admin`
- Regular users have limited access: `Core.Common.UserRoles.User`
- UserId available via `UserId` property in controllers inheriting from `AuthorizeFinTrackControllerBase`

## Key Gotchas

- Password hashes must follow specific format: `{Algorithm}.{Iterations}.{Salt}.{Hash}`
- Database migrations run automatically on startup
- Custom JWT validation using Hashicorp Vault for signature verification
- Connection string template replacement happens in Program.cs
- Custom logging enrichers in UserEnrichmentMiddleware
- Custom error handling via GlobalExceptionHandler
