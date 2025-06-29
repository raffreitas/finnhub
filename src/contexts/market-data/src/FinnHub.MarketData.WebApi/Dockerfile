FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/FinnHub.MarketData.WebApi/FinnHub.MarketData.WebApi.csproj", "FinnHub.MarketData.WebApi/"]
COPY ["src/FinnHub.MarketData.Shared/FinnHub.MarketData.Shared.csproj", "FinnHub.MarketData.Shared/"]

RUN dotnet restore "FinnHub.MarketData.WebApi/FinnHub.MarketData.WebApi.csproj"

COPY ["src/", "."]

WORKDIR "/src/FinnHub.MarketData.WebApi"
RUN dotnet build "./FinnHub.MarketData.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinnHub.MarketData.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnHub.MarketData.WebApi.dll"]