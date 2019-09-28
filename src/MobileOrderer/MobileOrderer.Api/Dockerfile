FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/MobileOrderer/MobileOrderer.Api/MobileOrderer.Api.csproj", "src/MobileOrderer/MobileOrderer.Api/"]
COPY ["src/Infrastructure/MinimalEventBus/MinimalEventBus.csproj", "src/Infrastructure/MinimalEventBus/"]
COPY ["src/Infrastructure/DapperDataAccess/DapperDataAccess.csproj", "src/Infrastructure/DapperDataAccess/"]
RUN dotnet restore "src/MobileOrderer/MobileOrderer.Api/MobileOrderer.Api.csproj"
COPY . .
WORKDIR "/src/src/MobileOrderer/MobileOrderer.Api"
RUN dotnet build "MobileOrderer.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MobileOrderer.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MobileOrderer.Api.dll"]