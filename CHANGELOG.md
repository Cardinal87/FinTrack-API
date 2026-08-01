# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-08-01
### Added
* Two-Factor Authentication (2FA) support (API endpoints, application use cases, and database migrations)
* Refresh token management (new API endpoints, dedicated services, and database table)
* Hashicorp Vault integration for secure JWT key signing and verification
* Redis caching services and Redis container to docker-compose
* NATS message broker integration (infrastructure publisher and Go-based NATS consumer as a mailer service)
* Healthchecks for docker containers
* AI agent infrastructure documentation

### Changed
* Replaced AutoMapper with MagicMapper for object mapping
* Updated test dependencies and structure (including migration to xUnit v3 compatibility)
* Enhanced logging in identity services
* Updated code documentation and Swagger specifications

### Fixed
* Concurrent access issues with `Log.Logger` in `Program.cs` during testing
* Test suites to align with new caching, 2FA, and refresh token services

## [1.1.0] - 2025-09-28
### Added
* Result pagination for endpoints that returs collections
* Log sink to Grafana/Loki
* Collecting metrics to Prometheus
* Grafana microservice to docker-compose file
* Endpoint for user updating
* Endpoint for deleting user by id
* Endpoints for retrieving users, accounts and transactions collections
* Messages with error description for failed API requests
  

### Fixed
* Handling domain model validation errors
* Handling of unique index constraint violations
* **Security issue:** Different API responses for incorrect login or password
* **Tests critical issue:** Fixed setting up test environment in WebApplicationFactory

## Changed
* Updated tests
* Updated code documentation
* Improved data retrieval for some endpoints for better performance
* Updated test structure
* Updated use-cases structure
