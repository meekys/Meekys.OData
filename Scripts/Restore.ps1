ls *.csproj -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "dotnet restore $_"

        dotnet restore -v Minimal $_
    } 