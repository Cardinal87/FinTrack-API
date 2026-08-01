package main

import (
	"log/slog"
	"os"
	"strconv"
)


type Config struct {
	NATS struct {
		Url string
	}
	SMTP struct {
		Host string
		Port int
		Username string
		Password string
		FromName string

	}
}


func Load() *Config {
	port, err := strconv.Atoi(getEnv("SMTP_PORT", "587"))
	if err != nil {
		slog.Error("Failed to parse SMTP_PORT value. default value will be used (SMTP_PORT: 587)", "error", err)
		port = 587
	}
	
	cfg := Config{}

	cfg.NATS.Url = getEnv("NATS_URL", "nats://localhost:4222")

	cfg.SMTP.Host = getEnv("SMTP_HOST", "smtp.gmail.com")
	cfg.SMTP.Port = port
	cfg.SMTP.Username = getEnv("SMTP_USERNAME", "")
	cfg.SMTP.Password = getEnv("SMTP_PASSWORD", "")
	cfg.SMTP.FromName = getEnv("SMTP_FROM_NAME", "FinTrack")

	return &cfg
}

func getEnv(key string, defaultValue string) string{
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return defaultValue
}