$compilerPath = "D:\devbinlib\xnaEffectCompiler\XNBCompiler.exee"

if(Test-Path -Path $compilerPath -PathType Leaf)
{
    Set-Location ModFolder/Effects
    Start-Process $compilerPath
    Set-Location ../../
}
else
{
    Write-Host "Error: Compiler '$compilerPath' not found." -ForegroundColor Red
    Write-Host "TODO: Provide a way to get the XNBCompiler.exe." -ForegroundColor Red
}