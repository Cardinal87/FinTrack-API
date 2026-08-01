package main

import (
	"bytes"
	"html/template"
)

const emailConfirmHTML = `
<!DOCTYPE html>
<html>
<head><meta charset="UTF-8"></head>
<body style="font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto;">
    <div style="background: #f5f5f5; padding: 20px; border-radius: 8px;">
        <h2 style="color: #0056b3;">Подтверждение email</h2>
        <p>Здравствуйте!</p>
        <p>Ваш код подтверждения:</p>
        <div style="background: white; padding: 15px; border-radius: 6px; text-align: center; margin: 20px 0;">
            <strong style="font-size: 2em; color: #0056b3; letter-spacing: 4px;">{{.Code}}</strong>
        </div>
        <p style="font-size: 0.9em; color: #666;">Если вы не запрашивали подтверждение, просто проигнорируйте это письмо.</p>
    </div>
</body>
</html>`

const twoFAHTML = `
<!DOCTYPE html>
<html>
<head><meta charset="UTF-8"></head>
<body style="font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto;">
    <div style="background: #f5f5f5; padding: 20px; border-radius: 8px;">
        <h2 style="color: #d9534f;">Вход в аккаунт (2FA)</h2>
        <p>Здравствуйте!</p>
        <p>Зафиксирована попытка входа. Ваш код:</p>
        <div style="background: white; padding: 15px; border-radius: 6px; text-align: center; margin: 20px 0;">
            <strong style="font-size: 2em; color: #d9534f; letter-spacing: 4px;">{{.Code}}</strong>
        </div>
        <hr style="border: none; border-top: 1px solid #ddd; margin: 20px 0;">
        <p style="font-size: 0.85em; color: #666;">
            <strong>IP адрес:</strong> {{.IP}}<br>
            <strong>Устройство:</strong> {{.UserAgent}}
        </p>
        <p style="font-size: 0.85em; color: #d9534f;">Если это были не вы, смените пароль.</p>
    </div>
</body>
</html>`

func RenderEmailConfirm(code string) (string, error) {
	return render(emailConfirmHTML, map[string]string{"Code": code})
}

func Render2FA(code, userAgent, ip string) (string, error) {
	return render(twoFAHTML, map[string]string{
		"Code":      code,
		"UserAgent": userAgent,
		"IP":        ip,
	})
}

func render(tmplStr string, data interface{}) (string, error) {
	tmpl, err := template.New("email").Parse(tmplStr)
	if err != nil {
		return "", err
	}
	var buf bytes.Buffer
	if err := tmpl.Execute(&buf, data); err != nil {
		return "", err
	}
	return buf.String(), nil
}