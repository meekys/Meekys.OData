param (
    [parameter(Mandatory=$true)]
    [string] $version
)

ls project.json -recurse | `
    % {
        Write-Host -ForegroundColor Cyan "SetVersion $_"

        $project = Get-Content $_ -raw | ConvertFrom-Json

        $oldVersion = $project.version
        $newVersion = "$version-*"

        $project.version = $newVersion

        $project | ConvertTo-Json -Depth 32 | Set-Content $_

        Write-Host -ForegroundColor Cyan "`t$oldVersion -> $newVersion"
    }