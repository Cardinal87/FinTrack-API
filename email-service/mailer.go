package main

import (
	"fmt"

	"gopkg.in/gomail.v2"
)

type Mailer struct {
	cfg *Config
}

func NewMailer(cfg *Config) *Mailer{
	return &Mailer{
		cfg: cfg,
	}
}

func (m *Mailer) Send(to, subject, htmlBody string) error {
	msg := gomail.NewMessage()
	msg.SetHeader("From", msg.FormatAddress(m.cfg.SMTP.Username, m.cfg.SMTP.FromName))
	msg.SetHeader("To", to)
	msg.SetHeader("Subject", subject)
	msg.SetBody("text/html", htmlBody)

	dialer := gomail.NewDialer(
		m.cfg.SMTP.Host,
		m.cfg.SMTP.Port,
		m.cfg.SMTP.Username,
		m.cfg.SMTP.Password,
	)
	
	if err := dialer.DialAndSend(msg); err != nil{
		return fmt.Errorf("Failed to send email to %s: %w", to, err)
	}
	return nil
}