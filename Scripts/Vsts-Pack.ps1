$revision = ($env:BUILD_BUILDNUMBER -split "\.")[-1]
$tag = if ($env:VERSION_TAG -ne $null) { "$($env:VERSION_TAG)$revision"}

./Scripts/Pack.ps1 `
    -configuration $env:BUILD_CONFIGURATION `
    -output "./artifacts" `
    -versionSuffix $tag