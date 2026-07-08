# Project Coding Rules (Non-Obvious Only)

## Architecture & Patterns

- Use CQRS pattern: Commands for writes, Queries for reads, Handlers for business logic
- Always use `Result` and `ValueResult<T>` for consistent return types with proper success/failure states
- Controllers must inherit from base classes (`FinTrackContollerBase`, `AuthorizeFinTrackControllerBase`)
- Repository pattern required for all data access operations
- Custom validation implemented in domain entities (not just relying on attributes)

## Authentication & Authorization

- User ID available via `UserId` property in authorized controllers (inherit from `AuthorizeFinTrackControllerBase`)
- Admin role checks done with `Core.Common.UserRoles.Admin`, regular users with `Core.Common.UserRoles.User`
- JWT tokens validated against Hashicorp Vault - never bypass this validation

## Domain Logic

- Password hashes must follow specific format: `{Algorithm}.{Iterations}.{Salt}.{Hash}`
- Custom PBKDF2 password hasher required - don't use standard .NET hashing
- TransferService handles inter-account transactions with built-in validation
- Domain exceptions like `AccountOwnershipException`, `InsufficientFundsException` must be properly handled

## Error Handling

- Use `HandleFailedResult(result)` method in controllers for consistent error responses
- Domain validation occurs in entity constructors - leverage this for input validation
- Custom error messages defined in `OperationStatusMessages` class
