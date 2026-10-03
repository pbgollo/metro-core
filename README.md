# Metro Core

Projeto de API .NET 10 em Clean Architecture.

## Arquitetura

A solução segue Clean Architecture com CQRS (MediatR). A regra de dependência é **de fora para dentro**: Presentation e Infrastructure dependem de Application/Domain; o Domain não conhece frameworks nem banco. O MediatR fica só em `Metro.Application` e `Metro.Api`.

```
src/
├── Presentation/         # Host HTTP (controllers, DI, pipeline)
│   └── Metro.Api
├── Application/          # Use cases (commands, queries, handlers, ApiResult, MediatR)
│   └── Metro.Application
├── Domain/               # Regras de negócio e contratos
│   ├── Metro.Domain      # Entidades, interfaces de repositório/serviços
│   └── Metro.Shared      # Abstrações compartilhadas (Entity, UoW…)
└── Infrastructure/       # Implementações técnicas
    ├── Metro.Infrastructure.PostgreSQL        # EF Core (writes + migrations)
    ├── Metro.Infrastructure.PostgreSQL.Dapper # Dapper (reads)
    ├── Metro.Infrastructure.Auth              # JWT e hash de senha
    ├── Metro.Infrastructure.Email             # SMTP (MailKit)
    └── Metro.Infrastructure.File              # Storage local de arquivos

tests/
├── Metro.Application.Tests  # Testes unitários dos handlers
└── Metro.Domain.Tests       # Testes de regras de domínio
```



### Fluxo de uma request

1. Controller (Presentation) recebe HTTP e monta um `Command` ou `Query`.
2. MediatR despacha para o handler correspondente na Application.
3. Handler usa repositórios/serviços (interfaces do Domain) e retorna `ApiResult<T>`.
4. `ApiController.FromResult` mapeia o status HTTP.

**Writes** (create/update/delete) passam pelo EF Core + Unit of Work.  
**Reads** (get/list) usam Dapper via query repositories.


### Como adicionar um novo módulo

1. Criar entidades/interfaces em `Metro.Domain`.
2. Criar commands/queries/handlers/view models em `Metro.Application`.
3. Criar validators FluentValidation do command/query (pipeline MediatR).
4. Implementar repositórios/mappings na Infrastructure.
5. Expor controller em `Metro.Api/Controllers` no padrão REST (`/{id}` na rota).
6. Se precisar de tabela nova: gerar migration (seção Comandos).

---

## Comandos

Na raiz do repositório:

```bash
make watch                          # API com hot reload
make run                            # API sem watch
make build                          # Compila a solution
make clean                          # Limpa bin/ e obj/
make test                           # Roda os testes unitários
make migration name=NomeDaMigration # Cria uma migration
make db-update                      # Aplica migrations no banco
make docker-up                      # Sobe API + Postgres no Docker
make docker-down                    # Para os containers
make docker-logs                    # Segue os logs da API
```

