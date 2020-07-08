#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/MobileTelecomsNetwork/MobileTelecomsNetwork.EventHandlers/MobileTelecomsNetwork.EventHandlers.csproj", "src/MobileTelecomsNetwork/MobileTelecomsNetwork.EventHandlers/"]
COPY ["src/Infrastructure/MinimalEventBus/MinimalEventBus.csproj", "src/Infrastructure/MinimalEventBus/"]
COPY ["src/Infrastructure/DapperDataAccess/DapperDataAccess.csproj", "src/Infrastructure/DapperDataAccess/"]
RUN dotnet restore "src/MobileTelecomsNetwork/MobileTelecomsNetwork.EventHandlers/MobileTelecomsNetwork.EventHandlers.csproj"
COPY . .
WORKDIR "/src/src/MobileTelecomsNetwork/MobileTelecomsNetwork.EventHandlers"
RUN dotnet build "MobileTelecomsNetwork.EventHandlers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MobileTelecomsNetwork.EventHandlers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MobileTelecomsNetwork.EventHandlers.dll"]