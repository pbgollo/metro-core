.PHONY: watch run test build migration db-update

APP_PROJECT=src/Application/Metro.Application/Metro.Application.csproj
DB_PROJECT=src/Infrastructure/Metro.Infrastructure.PostgreSQL/Metro.Infrastructure.PostgreSQL.csproj

watch:
	dotnet watch run --project $(APP_PROJECT)

run:
	dotnet run --project $(APP_PROJECT)

test:
	dotnet test tests/Metro.Domain.Tests/Metro.Domain.Tests.csproj

build:
	dotnet build Metro.sln

migration:
ifndef name
	$(error Uso: make migration name=NomeDaMigration)
endif
	dotnet ef migrations add $(name) -p $(DB_PROJECT) -s $(APP_PROJECT) -o Migrations

db-update:
	dotnet ef database update -p $(DB_PROJECT) -s $(APP_PROJECT)
