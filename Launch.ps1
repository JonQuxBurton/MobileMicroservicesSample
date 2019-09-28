# To run  
# .\Launch.ps1

$rootPath = "D:\Projects\MobileMicroservicesSample"

# SimCardWholesaler.Api
# WebApi Port 5001 
$cdProjectDir = "cd /d $rootPath\src\SimCardWholesaler\SimCardWholesaler.Api";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400

# MobileOrderer.Api
# WebApi Port 5000 
$cdProjectDir = "cd /d $rootPath\src\MobileOrderer\MobileOrderer.Api";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400

# SimCards.EventHandlers
# Process
$cdProjectDir = "cd /d $rootPath\src\SimCards\SimCards.EventHandlers";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 400