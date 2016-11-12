param (
    [parameter(Mandatory=$true)]
    [string] $projectName,
    [string] $configuration = "Debug",
    [string] $outputPath = ".",
    [string] $versionSuffix
)

$project = ls project.json -recurse | `
    ? {
        $_.Directory.Name -eq $projectName
    }

Write-Host -ForegroundColor Cyan "dotnet pack $project"

if ("$versionSuffix" -eq "") {
    dotnet pack $project -c $configuration -o $outputPath
}
else {
    dotnet pack $project -c $configuration -o $outputPath --version-suffix $versionSuffix
}