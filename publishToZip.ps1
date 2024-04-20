$mod_name = "SaladimHelper"
dotnet build -c Release
$v_str = Get-Content -Path $mod_name/ModFolder/everest.yaml -Raw
$v = [regex]::Match($v_str, "(?<=Version:\s)(.*?)\n").Value.Trim()
Compress-Archive $mod_name/ModFolder/* "$mod_name v$v.zip" -Force
dotnet clean