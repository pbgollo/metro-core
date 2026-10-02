# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Metro.sln ./
COPY src/Domain/Metro.Shared/Metro.Shared.csproj src/Domain/Metro.Shared/
COPY src/Domain/Metro.Domain/Metro.Domain.csproj src/Domain/Metro.Domain/
COPY src/Application/Metro.Application/Metro.Application.csproj src/Application/Metro.Application/
COPY src/Infrastructure/Metro.Infrastructure.Auth/Metro.Infrastructure.Auth.csproj src/Infrastructure/Metro.Infrastructure.Auth/
COPY src/Infrastructure/Metro.Infrastructure.Email/Metro.Infrastructure.Email.csproj src/Infrastructure/Metro.Infrastructure.Email/
COPY src/Infrastructure/Metro.Infrastructure.File/Metro.Infrastructure.File.csproj src/Infrastructure/Metro.Infrastructure.File/
COPY src/Infrastructure/Metro.Infrastructure.PostgreSQL/Metro.Infrastructure.PostgreSQL.csproj src/Infrastructure/Metro.Infrastructure.PostgreSQL/
COPY src/Infrastructure/Metro.Infrastructure.PostgreSQL.Dapper/Metro.Infrastructure.PostgreSQL.Dapper.csproj src/Infrastructure/Metro.Infrastructure.PostgreSQL.Dapper/
COPY src/Presentation/Metro.Api/Metro.Api.csproj src/Presentation/Metro.Api/
COPY tests/Metro.Domain.Tests/Metro.Domain.Tests.csproj tests/Metro.Domain.Tests/
COPY tests/Metro.Application.Tests/Metro.Application.Tests.csproj tests/Metro.Application.Tests/

RUN dotnet restore src/Presentation/Metro.Api/Metro.Api.csproj

COPY src/ src/
COPY tests/Metro.Domain.Tests/ tests/Metro.Domain.Tests/
COPY tests/Metro.Application.Tests/ tests/Metro.Application.Tests/

RUN dotnet publish src/Presentation/Metro.Api/Metro.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
    CMD curl -f http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "Metro.Api.dll"]
