param (
    [string] $configuration = "Debug"
)

if (!(Test-Path test)) {
    exit
}

ls test\project.json -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "dotnet test $_"
        
        dotnet test $_ -c $configuration -xml "$($_.DirectoryName)\Test-Results.xml"
    }