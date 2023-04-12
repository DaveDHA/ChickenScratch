function Write-Status($message, $status) {
    switch ($status) {
        "Success" { $color = "Green" }
        "Failure" { $color = "Red" }                   
        Default { $color = "Cyan" }
    }

    Write-Host "`n---------------------------------------------------------------------------" -ForegroundColor $color
    Write-Host $message -ForegroundColor $color
    Write-Host "---------------------------------------------------------------------------`n" -ForegroundColor $color
}

$currBranch = git branch --show-current

if ($currBranch -ne "master") {
    Write-Status "Not on master branch" Failure
    exit 1
}

Write-Host "`n`nEnter the Nuget API Key: " -ForegroundColor Magenta -NoNewline
$apiKey = Read-Host

Remove-item ./publish -Recurse 

Write-Status "Executing Tests" 
dotnet test ./src/ChickenScratch.sln -c Release

if (-Not $?) {
    Write-Status "Tests Failed" Failure
    exit 1
}

Write-Status "Creating Packages" 
dotnet pack ./src/ChickenScratch.sln -c Release -o ./publish

$pkgs = Get-Item ./publish/*.nupkg 

foreach ($pkg in $pkgs) {
    Write-Status ("Publishing {0}" -f $pkg) 
    dotnet nuget push $pkg.FullName -s https://api.nuget.org/v3/index.json -k $apiKey

    if (-Not $?) {
        Write-Status "Publishing Failed" Failure
        exit 1
    }    
}

Write-Status "Packages Published" Success
