Param(
  [Parameter(Position = 0)]
  [Boolean]$IsBuildAgent = $false,
  [Parameter(Position = 1)]
  [String]$Configuration = "Release",
  [Parameter(Position = 2)]
  [String]$VersionSuffix = ""
)

if((-Not ($IsBuildAgent)) -And ([string]::IsNullOrEmpty($VersionSuffix))) {
  $VersionSuffix = "1"
}

$VersionSuffixCommand = ""
if(-Not ([string]::IsNullOrEmpty($VersionSuffix))) {
  $VersionSuffixCommand = "--version-suffix"
}

Write-Host "Restoring 'TouchPortalApi' component....`n" -foregroundcolor "Magenta"
dotnet restore "TouchPortalApi"
dotnet restore "TouchPortalApi.Tests"

Write-Host "Building 'TouchPortalApi' component...`n" -foregroundcolor "Magenta"
dotnet build "TouchPortalApi" --configuration $Configuration

Write-Host "Cleaning 'TouchPortalApi' packages-dist folder..." -foregroundcolor "Magenta"
$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
if (Test-Path $DistFolderPath) {
  Remove-Item $DistFolderPath -Force -Recurse
}

Write-Host "Publishing 'TouchPortalApi' component...`n" -foregroundcolor "Magenta"
dotnet pack "TouchPortalApi" --output $DistFolderPath --configuration $Configuration --include-symbols $VersionSuffixCommand $VersionSuffix

if ($IsBuildAgent) {
  exit 0
}

Write-Host "Publishing 'TouchPortalApi' component to local NuGet source...`n" -foregroundcolor "Magenta"
dotnet nuget push packages-dist\* --source LocalDev
