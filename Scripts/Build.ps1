param (
    [string] $configuration = "Debug"
)

ls *.csproj -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "dotnet build $_"

        dotnet build $_ -c $configuration
    }