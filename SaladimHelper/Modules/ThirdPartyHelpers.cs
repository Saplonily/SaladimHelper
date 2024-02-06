namespace Celeste.Mod.SaladimHelper;

// better naming?
[NeedModuleInit]
public static class ThirdPartyHelpers
{
    public static bool ChronoHelperInstalled { get; private set; }
    public static bool FrostHelperInstalled { get; private set; }
    public static bool VivHelperInstalled { get; private set; }
    public static bool DeathTrackerInstalled { get; private set; }

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