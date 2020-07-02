FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/SimCardWholesaler/SimCardWholesaler.Api/SimCardWholesaler.Api.csproj", "src/SimCardWholesaler/SimCardWholesaler.Api/"]
COPY ["src/Infrastructure/DapperDataAccess/DapperDataAccess.csproj", "src/Infrastructure/DapperDataAccess/"]
RUN dotnet restore "src/SimCardWholesaler/SimCardWholesaler.Api/SimCardWholesaler.Api.csproj"
COPY . .
WORKDIR "/src/src/SimCardWholesaler/SimCardWholesaler.Api"
RUN dotnet build "SimCardWholesaler.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SimCardWholesaler.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SimCardWholesaler.Api.dll"]