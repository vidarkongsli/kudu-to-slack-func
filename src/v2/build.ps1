$ErrorActionPreference = 'stop'
$distDirectory = "$PSScriptRoot\dist"
function New-TemporaryDirectory {
    $parent = [System.IO.Path]::GetTempPath()
    [string] $name = [System.Guid]::NewGuid()
    New-Item -ItemType Directory -Path (Join-Path $parent $name)
}

function isCurrentScriptDirectoryGitRepository {
    $here = $PSScriptRoot
    if ((Test-Path "$here\.git") -eq $TRUE) {
        return $TRUE
    }
  
    # Test within parent dirs
    $checkIn = (Get-Item $here).parent
    while ($NULL -ne $checkIn) {
        $pathToTest = $checkIn.fullname + '/.git'
        if ((Test-Path $pathToTest) -eq $TRUE) {
            return $TRUE
        }
        else {
            $checkIn = $checkIn.parent
        }
    }
    $FALSE
}

function find-githash {
    $isInGit = isCurrentScriptDirectoryGitRepository
    if (-not($isInGit)) {
        get-date -Format 'yyyy-MM-dd_hh-mm-ss'
    }
    else {
        (git rev-parse HEAD).Substring(0,9)
    }
}

function New-ZipDirIfNotExists($zipdir) {
    if (-not(test-path $zipdir -PathType Container)) {
        mkdir $zipdir | out-null  
    }
}

dotnet restore

$tempDir = New-TemporaryDirectory

try {

dotnet msbuild /t:build /p:deployonbuild=true /p:configuration=release `
    $p:publi/p:publishurl="$tempDir"

# 3. Zip
New-ZipDirIfNotExists $distDirectory
$zipArchive = "$(find-githash).zip"
Write-Output "Compressing content from $tempDir\* to $distDirectory\$zipArchive"
Compress-Archive -Path "$tempDir\*" -DestinationPath $distDirectory\$zipArchive -force
} finally {
    remove-item $tempDir
}
