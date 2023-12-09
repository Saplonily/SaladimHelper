Set-Location SaladimHelper
dotnet build -c Release
Compress-Archive ModFolder/* ../SaladimHelper.zip
Set-Location ../