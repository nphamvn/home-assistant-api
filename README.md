## Description
Home Assistant System help you and your family members control your home via web app (Angular) or mobile app (iOS and Android) (not implemented) with two main features as below: 
- Control home devices such as Light, Air Conditioner, Camera, Weather...
- Chat which support private chat (one-to-one) and room chat

## Tech Stack/ Skills
- Backend Framework: ASP.NET Core 6 (Web API application)
- [Frontend Framework](https://github.com/nphamvn/home-assistant-web-client.git): Angular (migrated from version 13 to version 14)
- Database: PostgresSQL for production. SQLite for local development
- ORM Framework: Entity Framework Core 6
- Cache: Redis
- Message Broker: RabbitMQ
- Containerization: Docker
- Authentication/Authorization: JWT
- Realtime communication: SignalR

## Run with Docker
1. Clone the repo
```
git clone https://github.com/nphamvn/home-assistant-api.git
```
2. Change directory 
```
cd home-assistant-api
```
3. Run with docker compose
```
docker compose up -d
```
