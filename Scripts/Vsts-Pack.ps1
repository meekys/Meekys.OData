$revision = ($env:BUILD_BUILDNUMBER -split "\.")[-1]
$tag = if ($env:VERSION_TAG -ne $null) { "$($env:VERSION_TAG)$revision"}

./Scripts/Pack.ps1 `
    -projectName $env:SYSTEM_TEAMPROJECT `
    -configuration $env:BUILD_CONFIGURATION `
    -output "./artifacts" `
    -versionSuffix $tag