FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FinnHub.MarketData.Processor/FinnHub.MarketData.Processor.csproj", "src/FinnHub.MarketData.Processor/"]
COPY ["src/FinnHub.MarketData.WebApi/FinnHub.MarketData.WebApi.csproj", "src/FinnHub.MarketData.WebApi/"]
COPY ["src/FinnHub.MarketData.Shared/FinnHub.MarketData.Shared.csproj", "src/FinnHub.MarketData.Shared/"]
RUN dotnet restore "./src/FinnHub.MarketData.Processor/FinnHub.MarketData.Processor.csproj"
COPY . .
WORKDIR "/src/src/FinnHub.MarketData.Processor"
RUN dotnet build "./FinnHub.MarketData.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinnHub.MarketData.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinnHub.MarketData.Processor.dll"]