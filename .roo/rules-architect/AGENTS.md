# Project Architecture Rules (Non-Obvious Only)

## System Design

- Microservice architecture pattern with internal layered architecture
- CQRS pattern separates read and write operations with different models
- Eventual consistency model used for cross-boundary operations
- Domain-driven design principles applied to business logic organization

## Security Architecture

- Zero-trust authentication model with external JWT validation via Hashicorp Vault
- Passwords hashed using custom PBKDF2 implementation with specific format requirements
- Internal service communication secured through Vault-based token validation
- User identity propagation through request context for audit logging

## Data Management

- Repository pattern abstracts data access with EF Core implementation
- Domain entities enforce business rules at the aggregate level
- Transaction management handled at the infrastructure layer
- Database migrations run automatically during deployment

## Observability

- Structured logging with Serilog and custom enrichment for correlation
- Distributed tracing enabled through request context propagation
- Metrics collection via OpenTelemetry with custom service identification
- Centralized log aggregation through Grafana Loki integration

## Deployment

- Container-first deployment model with Docker Compose orchestration
- Multi-container application with PostgreSQL, Vault, Grafana, and Prometheus
- Environment-specific configuration through variable substitution
- Automatic database migration during container startup
