dotnet build -c Release
$sh_v_str = Get-Content -Path SaladimHelper/ModFolder/everest.yaml -Raw
$sh_v = [regex]::Match($sh_v_str, "(?<=Version:\s)(.*?)\n").Value.Trim()
Compress-Archive SaladimHelper/ModFolder/*,Documentation.md "SaladimHelper v$sh_v.zip"
dotnet clean