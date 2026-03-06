# FinTrack

Plataforma de transferências financeiras — projeto de portfólio demonstrando Clean Architecture, CQRS, mensageria assíncrona e observabilidade completa em .NET 8.

## Serviços

| Serviço | Responsabilidade | Porta |
|---|---|---|
| Auth Service | Registro, login, geração de JWT | 5001 |
| Account Service | Criação de contas, consulta de saldo | 5002 |
| Transaction Service | Transferências (CQRS) | 5003 |
| Notification Service | Consome eventos, envia notificações | 5004 |

## Stack

| Camada | Tecnologia |
|---|---|
| Framework | .NET 8, ASP.NET Core Minimal APIs |
| ORM | Entity Framework Core + Npgsql |
| Banco | PostgreSQL 16 |
| Mensageria | RabbitMQ 3.13 |
| Logs | Serilog → Seq |
| Métricas | OpenTelemetry → Prometheus → Grafana |
| Traces | OpenTelemetry → Jaeger |

## Decisões arquiteturais

| Decisão | Justificativa |
|---|---|
| Clean Architecture por serviço (4 projetos) | Separação explícita de responsabilidades; domínio sem dependências externas |
| CQRS no Transaction Service | Leitura de extrato e escrita de transferência têm cargas opostas |
| Banco por serviço | Isolamento de dados; schemas evoluem de forma independente |
| Eventos via RabbitMQ | Desacoplamento entre serviços; Notification não conhece Transaction |
| OpenTelemetry como camada de instrumentação | Vendor-neutral; troca de backend sem alterar código de aplicação |
| Serilog com enrichers no middleware | CorrelationId e UserId propagados automaticamente sem poluir domínio |

## Como rodar

```bash
docker-compose up -d
```

## Interfaces

| Interface | URL | Credenciais |
|---|---|---|
| Auth Swagger | http://localhost:5001/swagger | — |
| Seq (Logs) | http://localhost:8080 | — |
| Grafana (Métricas) | http://localhost:3000 | admin / fintrack123 |
| Jaeger (Traces) | http://localhost:16686 | — |
| RabbitMQ | http://localhost:15672 | fintrack / fintrack123 |
| Prometheus | http://localhost:9090 | — |

## Estrutura de um serviço

```
FinTrack.Auth/
  Domain/          → Entities, interfaces de repositório (sem deps externas)
  Application/     → Use cases, interfaces de serviço
  Infrastructure/  → EF Core, BCrypt, JWT (implementações)
  API/             → Program.cs, endpoints, Dockerfile
```
