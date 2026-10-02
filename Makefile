APP_PROJECT=src/Presentation/Metro.Api/Metro.Api.csproj
DB_PROJECT=src/Infrastructure/Metro.Infrastructure.PostgreSQL/Metro.Infrastructure.PostgreSQL.csproj

.PHONY: watch run test build clean migration db-update docker-up docker-down docker-logs

watch:
	cd src/Presentation/Metro.Api && dotnet watch run

run:
	dotnet run --project $(APP_PROJECT)

test:
	dotnet test tests/Metro.Application.Tests/Metro.Application.Tests.csproj
	dotnet test tests/Metro.Domain.Tests/Metro.Domain.Tests.csproj

build:
	dotnet build Metro.sln

clean:
	dotnet clean Metro.sln

migration:
ifndef name
	$(error Uso: make migration name=NomeDaMigration)
endif
	dotnet ef migrations add $(name) -p $(DB_PROJECT) -s $(APP_PROJECT) -o Migrations

db-update:
	dotnet ef database update -p $(DB_PROJECT) -s $(APP_PROJECT)

docker-up:
	docker compose up --build -d

docker-down:
	docker compose down

docker-logs:
	docker compose logs -f api
