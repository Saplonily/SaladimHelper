using Celeste.Mod.SaladimHelper.Entities;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class CommonModule
{
    public static bool DeathTrackerLoaded;

    public static void Load()
    {
        On.Celeste.ParticleTypes.Load += OnLoadParticles;
        // it's magical that unpacked mods need this
#if DEBUG
        Everest.LuaLoader.Precache(typeof(SaladimHelperModule).Assembly);
#endif

        EverestModuleMetadata deathTracker = new() { Name = "DeathTracker", Version = new Version(1, 0, 0) };
        if (Everest.Loader.DependencyLoaded(deathTracker))
            DeathTrackerLoaded = true;
    }

    public static void Unload()
    {
        On.Celeste.ParticleTypes.Load -= OnLoadParticles;
    }

    public static void LoadParticles()
    {
        if (BounceBlock.P_FireBreak is null) return;
        RustyZipMover.P_Motion = new(BounceBlock.P_FireBreak)
        {
            Acceleration = Vector2.UnitY * 60f,
            SpeedMax = 40f,
            SpeedMin = 10f,
            SpeedMultiplier = 1f,
            Color = Calc.HexToColor("58421C"),
            Color2 = Calc.HexToColor("866833"),
            Direction = -MathHelper.Pi / 2,
            DirectionRange = 0
        };

        DirtBounceBlock.P_Motion = new(BounceBlock.P_FireBreak)
        {
            Acceleration = Vector2.UnitY * 60f,
            SpeedMax = 40f,
            SpeedMin = 10f,
            SpeedMultiplier = 1f,
            Color = Calc.HexToColor("58421C"),
            Color2 = Calc.HexToColor("866833"),
            Direction = -MathHelper.Pi / 2
        };

        // happy copying
        BitsMomentumRefill.P_Shatter = new(Refill.P_Shatter);
        BitsMomentumRefill.P_Regen = new(Refill.P_Regen);
        BitsMomentumRefill.P_Glow = new(Refill.P_Glow);

        BitsMomentumRefill.P_SpeedField = new(Refill.P_Regen)
        {
            DirectionRange = 5f / 180f * MathHelper.Pi,
            Direction = 0f,
            SpeedMax = 0f,
            SpeedMin = 0f,
            Color = Calc.HexToColor("FFD500"),
            Color2 = Calc.HexToColor("FFFFFF"),
            ColorMode = ParticleType.ColorModes.Fade,
            LifeMin = 0.3f,
            LifeMax = 0.5f,
            Friction = 30f,
        };
    }

    public static void OnLoadParticles(On.Celeste.ParticleTypes.orig_Load orig)
    {
        orig();
        LoadParticles();
    }
}