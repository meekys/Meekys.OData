param (
    [string] $configuration = "Debug",
    [string] $outputPath = ".",
    [string] $versionSuffix
)

ls src\project.json -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "dotnet pack $_"

        if ("$versionSuffix" -eq "") {
            dotnet pack $_ -c $configuration -o $outputPath
        }
        else {
            dotnet pack $_ -c $configuration -o $outputPath --version-suffix $versionSuffix
        }
    }