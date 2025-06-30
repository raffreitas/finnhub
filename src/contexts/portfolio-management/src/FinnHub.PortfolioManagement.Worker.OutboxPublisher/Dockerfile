FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/FinnHub.PortfolioManagement.Worker.OutboxPublisher/FinnHub.PortfolioManagement.Worker.OutboxPublisher.csproj", "src/FinnHub.PortfolioManagement.Worker.OutboxPublisher/"]
RUN dotnet restore "./src/FinnHub.PortfolioManagement.Worker.OutboxPublisher/FinnHub.PortfolioManagement.Worker.OutboxPublisher.csproj"

COPY . .

WORKDIR "/src/src/FinnHub.PortfolioManagement.Worker.OutboxPublisher"
RUN dotnet build "./FinnHub.PortfolioManagement.Worker.OutboxPublisher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinnHub.PortfolioManagement.Worker.OutboxPublisher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnHub.PortfolioManagement.Worker.OutboxPublisher.dll"]