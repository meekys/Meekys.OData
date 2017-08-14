param (
    [parameter(Mandatory=$true)]
    [string] $version
)

ls src\*.csproj -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "SetVersion $_"

        [xml]$project = gc $_ 

        $oldVersion = $project.Project.PropertyGroup.VersionPrefix
        $newVersion = "$version"

        $project.Project.PropertyGroup.VersionPrefix = $newVersion

        $project.Save((Resolve-Path $_))

        Write-Host -ForegroundColor Cyan "`t$oldVersion -> $newVersion"
    }