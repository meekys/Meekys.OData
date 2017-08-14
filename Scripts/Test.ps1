param (
    [string] $configuration = "Debug"
)

if (!(Test-Path test)) {
    exit
}

ls test\*.csproj -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "dotnet test $_"
        
        pushd $_.Directory
        try {
            dotnet xunit -xml "$($_.DirectoryName)\Test-Results.xml"
        }
        finally
        {
            popd
        }
    }