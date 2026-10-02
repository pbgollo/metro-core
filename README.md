# Metro Core

Projeto de API .NET 10 em Clean Architecture.

## Arquitetura

A solução segue Clean Architecture com CQRS (MediatR). A regra de dependência é **de fora para dentro**: Application e Infrastructure dependem do Domain; o Domain não conhece frameworks nem banco.

```
src/
├── Application/          # Host HTTP (controllers, DI, pipeline)
│   └── Metro.Application
├── Domain/               # Regras de negócio e contratos
│   ├── Metro.Domain      # Entidades, commands, queries, handlers, interfaces
│   └── Metro.Shared      # Abstrações compartilhadas (Entity, ApiResult, UoW…)
└── Infrastructure/       # Implementações técnicas
    ├── Metro.Infrastructure.PostgreSQL        # EF Core (writes + migrations)
    ├── Metro.Infrastructure.PostgreSQL.Dapper # Dapper (reads)
    ├── Metro.Infrastructure.Auth              # JWT e hash de senha
    ├── Metro.Infrastructure.Email             # SMTP (MailKit)
    └── Metro.Infrastructure.File              # Storage local de arquivos

tests/
└── Metro.Domain.Tests    # Testes unitários dos handlers (xUnit + NSubstitute)
```



### Fluxo de uma request

1. Controller recebe HTTP e monta um `Command` ou `Query`.
2. MediatR despacha para o handler correspondente no Domain.
3. Handler usa repositórios/serviços (interfaces) e retorna `ApiResult<T>`.
4. `ApiController.FromResult` mapeia o status HTTP.

**Writes** (create/update/delete) passam pelo EF Core + Unit of Work.  
**Reads** (get/list) usam Dapper via query repositories.



### Como adicionar um novo módulo

1. Criar pasta em `Metro.Domain` (entidade, commands/queries, handlers, interfaces).
2. Implementar repositórios/mappings na Infrastructure.
3. Expor controller em `Metro.Application/Controllers` no padrão REST (`/{id}` na rota).
4. Se precisar de tabela nova: gerar migration (seção Comandos).

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

