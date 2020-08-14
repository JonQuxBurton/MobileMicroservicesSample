FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/Mobiles/Mobiles.Api/Mobiles.Api.csproj", "src/Mobiles/Mobiles.Api/"]
COPY ["src/Infrastructure/MinimalEventBus/MinimalEventBus.csproj", "src/Infrastructure/MinimalEventBus/"]
COPY ["src/Infrastructure/DapperDataAccess/DapperDataAccess.csproj", "src/Infrastructure/DapperDataAccess/"]
RUN dotnet restore "src/Mobiles/Mobiles.Api/Mobiles.Api.csproj"
COPY . .
WORKDIR "/src/src/Mobiles/Mobiles.Api"
RUN dotnet build "Mobiles.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Mobiles.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mobiles.Api.dll"]