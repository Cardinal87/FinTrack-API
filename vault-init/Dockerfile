FROM alpine:3.24.1 AS builder

ARG VAULT_VERSION=2.0.3

RUN apk add --no-cache curl unzip

RUN curl -LOf "https://releases.hashicorp.com/vault/${VAULT_VERSION}/vault_${VAULT_VERSION}_linux_amd64.zip" && \
    unzip vault_${VAULT_VERSION}_linux_${TARGETARCH:-amd64}.zip && \
    chmod +x vault


FROM alpine:3.24.1

COPY --from=builder /vault /usr/local/bin/
COPY ./scripts/vault_init.sh /
COPY ./configs/fintrack-api-policy.hcl /configs/

RUN apk add --no-cache jq

RUN vault --version

CMD ["sh", "/vault_init.sh"]