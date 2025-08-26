# FinTrack-API
## Description

Prototype of the financial system API

## Features
* User authentication and authorization
* Create and manage multiple accounts for one user
* Create transactions between accounts
* Filter transactions by date/interval/account

## Documentation
### Code documentation
**Core:** [docs/FinTrack.API.Core.xml](docs/FinTrack.API.Core.xml) \
**Application:** [docs/FinTrack.API.Application.xml](docs/FinTrack.API.Application.xml) \
**Infrastructure:** [docs/FinTrack.API.Infrastructure.xml](docs/FinTrack.API.Infrastructure.xml) \
**API Layer:** [docs/FinTrack.API.xml](docs/FinTrack.API.xml) 

### API
**Swagger Spec (JSON):** [docs/swagger.json](docs/swagger.json) \
**Swagger UI:** http://localhost:8080/swagger (when running in develop environment) 

## Quick start
Create `.env` file to configure environmental variables
Possible options:
| **Variable** | **Description** | **Value example**
| --------------- | --------------- | --------------- |
| POSTGRES_HOST | PostgreSql host | `db`
| POSTGRES_PORT | PostgreSql post | `5432`
| POSTGRES_USER| PostgreSql username | `postgres`
| POSTGRES_PASSWORD | PostgreSql password | `password`
| ASPNETCORE_ENVIRONMENT | runtime environment | `Development`

Then run API with one of two possible ways:
### Only API
Download [docker image](https://hub.docker.com/repository/docker/cardinal87/fintrack/general) from Docker Hub using 
``` bash
docker pull cardinal87/fintrack:tagname
```
then run docker image with following command:
``` bash
docker run --env-file path/to/.env -p 8080:8080 -v path/to/api/logs:/app/logs cardinal87/fintrack
```
### Using docker-compose (Recommended)
download [docker-compose.yml](https://github.com/Cardinal87/FinTrack-API/blob/main/FinTrack/docker-compose.yml) file with command:
```bash
curl -O https://github.com/Cardinal87/FinTrack-API/blob/main/FinTrack/docker-compose.yml
```
then, in the same directory, run docker containers with:
```bash
docker-compose --env-file path/to/.env up -d
```
