# Project Documentation Rules (Non-Obvious Only)

## Architecture Understanding

- The application follows a layered architecture: API -> Application -> Core -> Infrastructure
- CQRS pattern is implemented with MediatR for command/query handling
- Domain entities in Core project contain validation logic in constructors
- Infrastructure layer handles data access and external service integrations

## Configuration & Environment

- Connection string templates use placeholders like {DB_HOST} that get replaced at runtime
- Hashicorp Vault is used for JWT signature validation, not just for secrets
- Environment variables control database connection, logging destinations, and Vault settings
- Multiple configuration sections exist for JWT, Vault, and Serilog settings

## Authorization Model

- Authorization is role-based using custom UserRoles (User/Admin)
- UserId is extracted from JWT claims automatically in base controllers
- Custom middleware enriches logs with user context for audit trails
- Admin endpoints require explicit role authorization while user endpoints use default authorization

## Testing Strategy

- Unit tests focus on domain logic validation and business rules
- Integration tests cover API endpoints and full request/response cycles
- Test mocks are available in FinTrack.API.TestMocks project for repository mocking
- In-memory database is used for faster integration test execution
