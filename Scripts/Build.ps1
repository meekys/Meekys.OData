param (
    [string] $configuration = "Debug"
)

ls project.json -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "dotnet build $_"

        dotnet build $_ -c $configuration
    }