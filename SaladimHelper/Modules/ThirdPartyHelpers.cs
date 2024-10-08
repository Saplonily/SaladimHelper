﻿namespace Celeste.Mod.SaladimHelper;

// better naming?
[SaladimModule]
public static class ThirdPartyHelpers
{
    public static bool ChronoHelperInstalled { get; set; }
    public static bool FrostHelperInstalled { get; set; }
    public static bool VivHelperInstalled { get; set; }
    public static bool DeathTrackerInstalled { get; set; }

    public static void Initialize()
    {
        EverestModuleMetadata deathTracker = new()
        {
            Name = "DeathTracker",
            Version = new Version(1, 0, 0)
        };
        if (Everest.Loader.DependencyLoaded(deathTracker))
            DeathTrackerInstalled = true;
    }
}