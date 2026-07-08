# Project Debug Rules (Non-Obvious Only)

## Logging & Monitoring

- Logs are written to both console and files in 'logs/' directory, plus sent to Grafana Loki
- Serilog is configured with custom enrichment via UserEnrichmentMiddleware to add user context
- Custom diagnostic context includes Token Id and User Id for tracing requests

## Authentication Debugging

- JWT tokens are validated against Hashicorp Vault - check vault connectivity if auth fails
- Token signatures are verified via JwtSigningService - ensure vault signing key is accessible
- Custom JWT validation occurs in OnTokenValidated event in Program.cs

## Database & Migrations

- Database migrations run automatically on application startup in non-testing environments
- Connection string template replacement happens at runtime using environment variables
- Check DatabaseClient initialization if migration-related issues occur

## Telemetry

- Metrics are exposed via OpenTelemetry and available through Prometheus endpoint
- Service name and version are configured in Program.cs for telemetry purposes
- Multiple meter sources are configured including ASP.NET Core and Runtime instrumentation
