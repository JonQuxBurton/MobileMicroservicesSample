# Run this script as admin to prevent AccessDenied exceptions when creating the MetricsServer 

echo "Launching system..."

rootPath="D:\Projects\GitHub\MobileMicroservicesSample\src"

# Mobiles.Api
# WebApi Port 5000
echo "Starting Mobiles.Api..."
cd "${rootPath}\Mobiles\Mobiles.Api"
dotnet run &

# SimCards.EventHandlers
# Process
echo "Starting ExternalSimCardsProvider.Api..."
cd "${rootPath}\SimCards\SimCards.EventHandlers"
dotnet run &

# ExternalSimCardsProvider.Api
# WebApi Port 5001 
echo "Starting ExternalSimCardsProvider.Api..."
cd "${rootPath}\ExternalSimCardsProvider\ExternalSimCardsProvider.Api"
dotnet run &

# MobileTelecomsNetwork.EventHandlers
# Process
echo "Starting MobileTelecomsNetwork.EventHandlers..."
cd "${rootPath}\MobileTelecomsNetwork\MobileTelecomsNetwork.EventHandlers"
dotnet run &

# ExternalMobileTelecomsNetwork.Api
# WebApi Port 5002
echo "Starting ExternalMobileTelecomsNetwork.Api..."
cd "${rootPath}\ExternalMobileTelecomsNetwork\ExternalMobileTelecomsNetwork.Api"
dotnet run &

wait
echo "System shutdown completed"