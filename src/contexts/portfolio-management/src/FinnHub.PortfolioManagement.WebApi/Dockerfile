FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["../../../building-blocks/FinnHub.Shared.Core\FinnHub.Shared.Core.csproj", "FinnHub.Shared.Core/"]
COPY ["../../../building-blocks/FinnHub.Shared.Kernel/FinnHub.Shared.Kernel.csproj", "FinnHub.Shared.Kernel/"]
COPY ["src/FinnHub.PortfolioManagement.WebApi/FinnHub.PortfolioManagement.WebApi.csproj", "FinnHub.PortfolioManagement.WebApi/"]
COPY ["src/FinnHub.PortfolioManagement.Application/FinnHub.PortfolioManagement.Application.csproj", "FinnHub.PortfolioManagement.Application/"]
COPY ["src/FinnHub.PortfolioManagement.Domain/FinnHub.PortfolioManagement.Domain.csproj", "FinnHub.PortfolioManagement.Domain/"]
COPY ["src/FinnHub.PortfolioManagement.Infrastructure.Authentication/FinnHub.PortfolioManagement.Infrastructure.Authentication.csproj", "FinnHub.PortfolioManagement.Infrastructure.Authentication/"]
COPY ["src/FinnHub.PortfolioManagement.Infrastructure.Caching/FinnHub.PortfolioManagement.Infrastructure.Caching.csproj", "FinnHub.PortfolioManagement.Infrastructure.Caching/"]
COPY ["src/FinnHub.PortfolioManagement.Infrastructure.Telemetry/FinnHub.PortfolioManagement.Infrastructure.Telemetry.csproj", "FinnHub.PortfolioManagement.Infrastructure.Telemetry/"]
COPY ["src/FinnHub.PortfolioManagement.Infrastructure.Messaging/FinnHub.PortfolioManagement.Infrastructure.Messaging.csproj", "FinnHub.PortfolioManagement.Infrastructure.Messaging/"]
COPY ["src/FinnHub.PortfolioManagement.Infrastructure.Persistence/FinnHub.PortfolioManagement.Infrastructure.Persistence.csproj", "FinnHub.PortfolioManagement.Infrastructure.Persistence/"]

RUN dotnet restore "FinnHub.PortfolioManagement.WebApi/FinnHub.PortfolioManagement.WebApi.csproj"

COPY ["src/", "."]

WORKDIR "/src/FinnHub.PortfolioManagement.WebApi"
RUN dotnet build "FinnHub.PortfolioManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FinnHub.PortfolioManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnHub.PortfolioManagement.WebApi.dll"]