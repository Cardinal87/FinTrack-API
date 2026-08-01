package main

import (
	"encoding/json"
	"log/slog"

	"github.com/nats-io/nats.go"
)



type Handlers struct {
	mailer *Mailer
}

func NewHandler(mailer *Mailer) *Handlers {
	return &Handlers {
		mailer: mailer,
	}
}


func (h *Handlers) HandleEmailConfirm(msg *nats.Msg) {
	var request EmailConfirmRequest
	if err := json.Unmarshal(msg.Data,  &request); err != nil {
		slog.Error("Failed to unmarshal message for email confirmation", "error", err)
		msg.Nak()
		return
	}
	slog.Info("Code", "code", request.Code)
	html, err := RenderEmailConfirm(request.Code)
	if err != nil {
		slog.Error("Failed to render email", "error", err)
		msg.Nak()
		return
	}
	if err := h.mailer.Send(request.Email, "Email verification code", html);  err != nil {
		slog.Error("Failed to send email", "error", err, "email", request.Email)
		msg.Nak()
		return
	}
	msg.Ack()
	slog.Debug("Message sent", "email", request.Email)
}


func (h *Handlers) Handler2FA(msg *nats.Msg){
	var request LoginConfirmRequest
	if err := json.Unmarshal(msg.Data, &request); err != nil {
		slog.Error("Failed to unmarshal message for login confirmation", "error", err)
		msg.Nak()
		return
	}

	html, err := Render2FA(request.Code, request.UserAgnet, request.Ip)

	if err != nil {
		slog.Error("Failed to render email", "error", err)
		msg.Nak()
		return
	}

	if err := h.mailer.Send(request.Email, "Login verification code", html); err != nil {
		slog.Error("Failed to send email", "error", err, "email", request.Email)
		msg.Nak()
		return
	}

	msg.Ack()
	slog.Debug("Message sent", "email", request.Email)
}