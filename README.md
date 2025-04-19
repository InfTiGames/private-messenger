# PrivateMessenger

🔐 Приватный кроссплатформенный мессенджер, разработанный с нуля.

## Архитектура

- Backend: ASP.NET Core + Clean Architecture
- Frontend: React (Web), React Native (Mobile)
- DB: MSSQL + EF Core
- Messaging: SignalR + RabbitMQ
- Auth: JWT + Identity
- Infra: Docker, Azure, GitHub Actions

## Особенности

- Приватность: сообщения хранятся только локально
- Реальное время: WebSocket + RabbitMQ
- Безопасность: HTTPS, JWT, кэш только на клиенте