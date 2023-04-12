$apiKey = Read-Host -Prompt "Enter the Nuget API key"

Remove-item ./publish -Recurse 

dotnet test ./src/ChickenScratch.sln -c Release

if (-Not $?) {
    Write-Output "!!!!Tests failed"
    exit 1
}

dotnet pack ./src/ChickenScratch.sln -c Release -o ./publish

$pkgs = Get-Item ./publish/*.nupkg 

foreach ($pkg in $pkgs) {
    dotnet nuget push $pkg.FullName -s https://api.nuget.org/v3/index.json -k $apiKey
}