$buildRevision = Get-Date -Format "yyyy.MMdd"

./Scripts/SetVersion.ps1 "$env:VERSION_MAJOR.$env:VERSION_MINOR.$buildRevision"