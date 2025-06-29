FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FinnHub.MarketData.Ingestion/FinnHub.MarketData.Ingestion.csproj", "src/FinnHub.MarketData.Ingestion/"]
COPY ["src/FinnHub.MarketData.WebApi/FinnHub.MarketData.WebApi.csproj", "src/FinnHub.MarketData.WebApi/"]
COPY ["src/FinnHub.MarketData.Shared/FinnHub.MarketData.Shared.csproj", "src/FinnHub.MarketData.Shared/"]
RUN dotnet restore "./src/FinnHub.MarketData.Ingestion/FinnHub.MarketData.Ingestion.csproj"
COPY . .
WORKDIR "/src/src/FinnHub.MarketData.Ingestion"
RUN dotnet build "./FinnHub.MarketData.Ingestion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinnHub.MarketData.Ingestion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnHub.MarketData.Ingestion.dll"]