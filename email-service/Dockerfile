FROM golang:1.26-alpine  AS builder

RUN apk add --no-cache ca-certificates tzdata

WORKDIR /app
COPY go.mod go.sum ./

RUN go mod download

COPY . .

RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build \
    -ldflags="-w -s" \
    -o /email-service \
    .

FROM alpine:3.24.1

RUN addgroup -S appgroup && adduser -S appuser -G appgroup

COPY --from=builder /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/
COPY --from=builder /usr/share/zoneinfo /usr/share/zoneinfo

WORKDIR /app
COPY --from=builder /email-service /app/

RUN chown -R appuser:appgroup /app
USER appuser

ENTRYPOINT ["/app/email-service"]