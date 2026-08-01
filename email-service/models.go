package main

type EmailConfirmRequest struct {
	Email string `json:"email"`
	Code  string `json:"totpCode"`
}

type LoginConfirmRequest struct {
	Email     string `json:"email"`
	Code      string `json:"totpCode"`
	Ip        string `json:"ip"`
	UserAgnet string `json:"userAgent"`
}
