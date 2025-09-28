# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
