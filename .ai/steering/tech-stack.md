# Tech Stack — MetroIsJustABus (`metro-core`)

## System components

| Component | Role | Technology |
| --- | --- | --- |
| `metro-web` | Single web app (passengers + ops) | Angular (separate repo) |
| `metro-core` | Domain source of truth + HTTP APIs | C# / .NET 10 |
| `metro-notifications` | Notification delivery (planned) | C# / .NET, separate process/container in this repo |
| PostgreSQL | Domain persistence | PostgreSQL 16 |
| RabbitMQ | Async messaging (planned) | RabbitMQ / AMQP |

## This repository (`metro-core`)

### Runtime and host
- **Language / runtime:** C# on **.NET 10** (`net10.0`)
- **HTTP host:** ASP.NET Core (`Metro.Api`)
- **API docs:** OpenAPI + Swashbuckle
- **Auth:** JWT Bearer (access + refresh tokens)

**Why:** Strong typing and a mature ecosystem for Clean Architecture APIs; JWT fits a separate SPA (`metro-web`) without server-side sessions in the API.

### Application structure
- **Architecture:** Clean Architecture — Presentation, Application, Domain, Infrastructure
- **Use cases:** CQRS via **MediatR** (Commands / Queries + handlers)
- **Validation:** **FluentValidation** in a MediatR pipeline (`ValidationBehavior`)
- **Results:** `ApiResult` in Application; controllers map to HTTP status codes

**Why:** Keeps domain/use cases free of ASP.NET types; one request pipeline for validation; handlers stay testable without the web host.

### Data access
- **Database:** PostgreSQL 16 (Docker image `postgres:16-alpine`)
- **Writes:** EF Core + Unit of Work (`Metro.Infrastructure.PostgreSQL`)
- **Reads:** Dapper (`Metro.Infrastructure.PostgreSQL.Dapper`)
- **Other infrastructure:** Auth hashing/JWT, email (MailKit), local file storage

**Why PostgreSQL:** Relational domain (network, schedules, tickets, validations) needs integrity and a single source of truth for the core.

**Why EF Core for writes:** Migrations, change tracking, and transactional unit of work for commands.

**Why Dapper for reads:** Explicit SQL and lighter queries for list/get paths without loading full write models.

### Messaging and notifications (architecture decision)
- **Broker:** RabbitMQ (AMQP), own container, no business logic
- **Consumer:** `metro-notifications` — same repo, separate project/process/container

**Why:** Meets the distributed-system requirement: sync HTTP for the web app, async events for notifications; notification failure must not block network consultation.

### Packaging and local run
- **Containers:** Docker (`Dockerfile` for the API) + `docker-compose` (API + PostgreSQL today; RabbitMQ/notifications to be added)
- **Dev commands:** `Makefile` (`watch`, `test`, `migration`, `docker-up`, …)

## Versions (majors — this repo)

.NET 10 · MediatR 12 · FluentValidation 11 · EF Core 10 · PostgreSQL 16 · JWT / IdentityModel 8

Exact package versions live in `.csproj` files and change; the **reasons** above should stay accurate. When bumping a major, update this file with decision-relevant notes.

## Related repos

- **`metro-web`:** Angular SPA; consumes `metro-core` HTTP APIs; no domain rules in the frontend.
- Canonical statement: `docs/spec/metro-is-just-a-bus.md`
