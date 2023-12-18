namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class CommonModule
{
    public static void Load()
    {
        On.Celeste.ParticleTypes.Load += OnLoadParticles;
        // it's magical that unpacked mods need this
#if DEBUG
        Everest.LuaLoader.Precache(typeof(SaladimHelperModule).Assembly);
#endif
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
    }

    public static void OnLoadParticles(On.Celeste.ParticleTypes.orig_Load orig)
    {
        orig();
        LoadParticles();
    }
}