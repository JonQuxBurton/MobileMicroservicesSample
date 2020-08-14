# To run  
# .\Launch.ps1

$rootPath = "D:\Projects\GitHub\MobileMicroservicesSample"

# Mobiles.Api
# WebApi Port 5000 
$cdProjectDir = "cd /d $rootPath\src\Mobiles\Mobiles.Api";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400

# SimCards.EventHandlers
# Process
$cdProjectDir = "cd /d $rootPath\src\SimCards\SimCards.EventHandlers";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400

# ExternalSimCardsProvider.Api
# WebApi Port 5001 
$cdProjectDir = "cd /d $rootPath\src\ExternalSimCardsProvider\ExternalSimCardsProvider.Api";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400

# MobileTelecomsNetwork.EventHandlers
# Process
$cdProjectDir = "cd /d $rootPath\src\MobileTelecomsNetwork\MobileTelecomsNetwork.EventHandlers";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400

# ExternalMobileTelecomsNetwork.Api
# WebApi Port 5002
$cdProjectDir = "cd /d $rootPath\src\ExternalMobileTelecomsNetwork\ExternalMobileTelecomsNetwork.Api";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400
