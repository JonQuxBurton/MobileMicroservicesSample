$rootPath = "D:\Projects\MobileMicroservicesSample"

$coveragePath = "$rootPath\coverage"
$reports = New-Object System.Collections.Generic.List[hashtable]
$reports.Add(@{ Project="MinimalEventBus"; Folder="Infrastructure"})
$reports.Add(@{ Project="DapperDataAccess"; Folder="Infrastructure"})
$reports.Add(@{ Project="SimCards.EventHandlers"; Folder="SimCards"})
$reports.Add(@{ Project="MobileOrderer.Api"; Folder="MobileOrderer"})
$reports.Add(@{ Project="SimCardWholesaler.Api"; Folder="SimCardWholesaler"})

$reportFiles = New-Object System.Collections.Generic.List[string]
foreach ($report in $reports)
{
    $project = $report.Project
    $reportFiles.Add("$coveragePath\$project-coverage.cobertura.xml") 
}

foreach ($report in $reports)
{
    $project = $report.Project
    $folder = $report.Folder
    coverlet "$rootPath\test\$folder\$project.Tests\bin\Debug\netcoreapp2.2\$project.Tests.dll" --target "dotnet" --targetargs "test $rootPath\test\$folder\$project.Tests --no-build" --format cobertura --output "coverage\$project-coverage.cobertura.xml"
}

$reportsConcat = ($reportFiles -join ";")
reportgenerator "-reports:$reportsConcat" "-targetdir:$rootPath\coverage\report"