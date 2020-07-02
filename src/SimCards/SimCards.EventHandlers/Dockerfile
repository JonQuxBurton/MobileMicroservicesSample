FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/SimCards/SimCards.EventHandlers/SimCards.EventHandlers.csproj", "src/SimCards/SimCards.EventHandlers/"]
COPY ["src/Infrastructure/MinimalEventBus/MinimalEventBus.csproj", "src/Infrastructure/MinimalEventBus/"]
COPY ["src/Infrastructure/DapperDataAccess/DapperDataAccess.csproj", "src/Infrastructure/DapperDataAccess/"]
RUN dotnet restore "src/SimCards/SimCards.EventHandlers/SimCards.EventHandlers.csproj"
COPY . .
WORKDIR "/src/src/SimCards/SimCards.EventHandlers"
RUN dotnet build "SimCards.EventHandlers.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SimCards.EventHandlers.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SimCards.EventHandlers.dll"]