#r "System"

using System;
using System.IO;
using System.Diagnostics;

public void CopyFolder(string sourceFolder, string destFolder)
{
    if (Directory.Exists(destFolder) is false)
        Directory.CreateDirectory(destFolder);

    var files = Directory.EnumerateFiles(sourceFolder);
    foreach (string file in files)
        File.Copy(file, Path.Combine(destFolder, Path.GetFileName(file)), true);

    var folders = Directory.EnumerateDirectories(sourceFolder);
    foreach (string folder in folders)
        CopyFolder(folder, Path.Combine(destFolder, Path.GetFileName(folder)));
}

Console.WriteLine("csi Tasks working...");

try
{
    string binaryBasePath = @"SaladimHelper/bin/x86/Debug/net452";
    string modName = "SaladimHelper";
    string celesteModFolder = Path.Combine(@"C:/Program Files (x86)/Steam/steamapps/common/Celeste/Mods", modName);
    File.Copy(Path.Combine(binaryBasePath, $"{modName}.dll"), Path.Combine(@"ModPack", $"{modName}.dll"), true);
    File.Copy(Path.Combine(binaryBasePath, $"{modName}.pdb"), Path.Combine(@"ModPack", $"{modName}.pdb"), true);
    CopyFolder(@"ModPack", celesteModFolder);
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
    return -1;
}

Console.WriteLine("csi Tasks done");