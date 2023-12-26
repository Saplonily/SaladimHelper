dotnet build -c Release
$sh_v = ((Get-Content -Path SaladimHelper/ModFolder/everest.yaml -TotalCount 2)[1]).SubString(11)
Compress-Archive SaladimHelper/ModFolder/*,Documentation.md "SaladimHelper v$sh_v.zip"