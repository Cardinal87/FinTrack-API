# FinTrack-API
## Description

Prototype of the financial system API

## Features
* Create and update user
* User authentication and authorization
* Create and manage multiple accounts for one user
* Create transactions between accounts
* Filter transactions by date/interval/account
* Collect metrics to prometheus
* Collect logs to loki

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
### Environment configuration
Create `.env` file to configure environmental variables
Possible options:
| **Variable** | **Description** | **Value example**
| --------------- | --------------- | --------------- |
| POSTGRES_HOST | PostgreSql host | `db`
| POSTGRES_PORT | PostgreSql post | `5432`
| POSTGRES_USER| PostgreSql username | `postgres`
| POSTGRES_PASSWORD | PostgreSql password | `password`
| ASPNETCORE_ENVIRONMENT | runtime environment | `Development`
| GRAFANA_USERNAME | admin username for grafana | `admin` 
| GRAFANA_PASSWORD | admin password for grafana | `password`
| SMTP_HOST | smpt relay url | `smtp.gmail.com`
| SMTP_PORT | smtp relay port | `587`
| SMTP_USERNAME | smtp username for authentication | `your@email.com`
| SMTP_PASSWORD | smtp password for authentication | `password`

Then run API with one of two possible ways:
### Only API (Manual configuration of all service dependencies)
Download [docker image](https://hub.docker.com/repository/docker/cardinal87/fintrack/general) from Docker Hub using 
``` bash
docker pull cardinal87/fintrack:tagname
```
then run docker image with following command:
``` bash
docker run --env-file path/to/.env -p 8080:8080 -v path/to/api/logs:/app/logs cardinal87/fintrack
```
### Using docker-compose (Recommended)
Firstly, initialize appropriate directory structure:
``` bash
mkdir -p fintrack/configs fintrack/deployment
cd fintrack
```

Download all configuration files from [configs](configs/) directory:
``` bash
curl -o configs/fintrack-api-policy.hcl https://raw.githubusercontent.com/Cardinal87/FinTrack-API/main/configs/fintrack-api-policy.hcl
curl -o configs/loki.yaml https://raw.githubusercontent.com/Cardinal87/FinTrack-API/main/configs/loki.yaml
curl -o configs/nats-server.conf https://raw.githubusercontent.com/Cardinal87/FinTrack-API/main/configs/nats-server.conf
curl -o configs/prometheus.yml https://raw.githubusercontent.com/Cardinal87/FinTrack-API/main/configs/prometheus.yml
curl -o configs/redis.conf https://raw.githubusercontent.com/Cardinal87/FinTrack-API/main/configs/redis.conf
curl -o configs/vault.hcl https://raw.githubusercontent.com/Cardinal87/FinTrack-API/main/configs/vault.hcl
```

Download [docker-compose.yml](https://github.com/Cardinal87/FinTrack-API/blob/main/FinTrack/docker-compose.yml) file:
```bash
curl -o deployment/docker-compose.yml https://github.com/Cardinal87/FinTrack-API/blob/main/FinTrack/docker-compose.yml
```

Create the `.env` file in the `deployment` folder according to the [Environment configuration](#environment-configuration) section

Finally run docker containers with:
```bash
cd deployment
docker-compose up -d
```


## Build from source code
clone repo with the next command
``` bash
git clone https://github.com/Cardinal87/FinTrack-API.git
```
then change working dir to 
``` bash
cd FinTrack.API/FinTrack
```
and build docker image
``` bash
docker build -f FinTrack.API/Dockerfile -t fintrack:tag .
```
