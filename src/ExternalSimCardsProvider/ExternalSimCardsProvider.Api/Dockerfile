FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/ExternalSimCardsProvider/ExternalSimCardsProvider.Api/ExternalSimCardsProvider.Api.csproj", "src/ExternalSimCardsProvider/ExternalSimCardsProvider.Api/"]
COPY ["src/Infrastructure/DapperDataAccess/DapperDataAccess.csproj", "src/Infrastructure/DapperDataAccess/"]
RUN dotnet restore "src/ExternalSimCardsProvider/ExternalSimCardsProvider.Api/ExternalSimCardsProvider.Api.csproj"
COPY . .
WORKDIR "/src/src/ExternalSimCardsProvider/ExternalSimCardsProvider.Api"
RUN dotnet build "ExternalSimCardsProvider.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ExternalSimCardsProvider.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ExternalSimCardsProvider.Api.dll"]