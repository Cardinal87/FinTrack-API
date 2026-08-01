package main

import (
	"log/slog"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/nats-io/nats.go"
)

func main(){
	slog.Info("Starting email service")
	cfg := Load()

	nc, err := nats.Connect(cfg.NATS.Url)

	if err != nil {
		slog.Error("Failed to connect NATS", "error", err)
		os.Exit(1)
	}
	defer nc.Close()
	slog.Info("successfully connected to NATS", "url", cfg.NATS.Url)


	js, err := nc.JetStream()

	if err != nil {
		slog.Error("Failed to get JetStream context", "error", err)
	}

	m := NewMailer(cfg)
	h := NewHandler(m)

	sub1, err := js.Subscribe("fintrack.verification.email", h.HandleEmailConfirm, 
		nats.Durable("email_verification_consumer"),
		nats.ManualAck(),
		nats.AckWait(30*time.Second))

	if err != nil {
		slog.Error("Failed to sub on fintrack.verification.email subject", "error", err)
		os.Exit(1)
	}

	defer sub1.Unsubscribe()

	sub2, err := js.Subscribe("fintrack.verification.login", h.Handler2FA, 
		nats.Durable("login_verification_consumer"),
		nats.ManualAck(),
		nats.AckWait(30*time.Second))

	if err != nil {
		slog.Error("Failed to sub on fintrack.verification.login subject", "error", err)
		os.Exit(1)
	}

	defer sub2.Unsubscribe()
	
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	slog.Info("Email service shutting down")
}