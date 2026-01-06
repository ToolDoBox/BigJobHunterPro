[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$installScript = "$env:TEMP\dotnet-install.ps1"
Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile $installScript -UseBasicParsing
& $installScript -Channel 8.0 -InstallDir "$env:USERPROFILE\.dotnet"
