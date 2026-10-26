# Conventions — Metro Core

## Solution layout

- Clean Architecture folders under `src/`: `Presentation`, `Application`, `Domain`, `Infrastructure`
- Dependency rule: **outside → inside**. Domain has no framework or database references
- MediatR is used only in `Metro.Application` and `Metro.Api`
- Tests live under `tests/` (`Metro.Application.Tests`, `Metro.Domain.Tests`)

## Adding a module

1. Entities / repository interfaces in `Metro.Domain`
2. Commands, queries, handlers, view models in `Metro.Application`
3. FluentValidation validators for commands/queries (MediatR pipeline)
4. Repository / mapping implementations in Infrastructure
5. Controller in `Metro.Api/Controllers` (REST, `{id}` on the route)
6. New tables via EF migration (`make migration name=...`)

## API

- Routes: `api/v1/{resource}` (plural), e.g. `api/v1/users`
- Controllers inherit `ApiController` and return `FromResult(...)`
- Controllers stay thin: authorize, bind, `_mediator.Send(...)`, map result
- Prefer `[Authorize("role")]` policies (`user`, `master`, …) at the action

## Request flow

1. Controller builds a `Command` or `Query`
2. MediatR runs `ValidationBehavior` (FluentValidation) then the handler
3. Handler uses Domain contracts only; returns `ApiResult<T>`
4. `FromResult` maps `ResultStatus` → HTTP status

## Commands vs queries

- **Writes** (create/update/delete): EF Core + `IUnityOfWork` (begin / commit / rollback)
- **Reads** (get/list): Dapper via query repositories in Application/Infrastructure
- Do not use EF write models for list/get paths when a query repository exists

## Results and errors

- Handlers return `ApiResult<T>` (`Ok`, `Created`, `NoContent`, `BadRequest`, `Unauthorized`, `NotFound`, `Conflict`, `InternalError`)
- Controllers must not invent HTTP status codes ad hoc — use `FromResult`
- Validation failures go through FluentValidation; business conflicts (e.g. duplicate email) return `Conflict` from the handler
- User-facing validation/conflict messages may be in Portuguese (current codebase style)

## Naming

- Projects: `Metro.{Layer}` or `Metro.Infrastructure.{Tech}`
- Feature folders in Application: `Users/`, `Auth/`, with `Commands`, `Queries`, `Handlers`, `Validators`, `ViewModels`
- Commands: `CreateUserCommand`; Queries: `GetUserQuery`, `ListUserQuery`
- Handlers: `CreateUserHandler`; Validators: `CreateUserCommandValidator`

## Domain and Infrastructure

- Domain defines interfaces (`IUserRepository`, `IPasswordService`, …); Infrastructure implements them
- Persist through repositories + UoW; no direct `DbContext` usage from handlers
- Do not reference ASP.NET types from Application or Domain

## Notifications service (when added)

- Place under `src/Notifications/Metro.Notifications` (separate host)
- Own Dockerfile / compose service; communicate via RabbitMQ events, not in-process calls into `Metro.Api`
- Shared event contracts only (e.g. messaging contracts project) — not Application handlers

## Code style

- Nullable enabled; prefer explicit types at API boundaries
- Keep handlers focused on one use case
- Prefer extending existing module patterns over introducing a parallel style
