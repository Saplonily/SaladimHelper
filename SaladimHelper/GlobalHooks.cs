using Celeste.Mod.SaladimHelper.Entities;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class GlobalHooks
{
    public static bool FrostHelperLoaded;
    public static ILHook FrostHelperDreamBlockHook = null;

    public static void Load()
    {
        On.Celeste.ParticleTypes.Load += OnLoadParticles;

        EverestModuleMetadata frostHelper = new()
        {
            Name = "FrostHelper",
            Version = new Version(1, 44, 0)
        };
        if (Everest.Loader.TryGetDependency(frostHelper, out var module))
        {
            Logger.Log(LogLevel.Info, ModuleName, "Found FrostHelper, hooking CustomDreamBlockV2.DreamDashUpdate...");
            FrostHelperLoaded = true;
            Assembly asm = module.GetType().Assembly;
            Type dreamBlockType = asm.GetType("FrostHelper.CustomDreamBlockV2");
            MethodInfo dreamDashUpdate = dreamBlockType.GetMethod("Player_DreamDashUpdate", BindingFlags.NonPublic | BindingFlags.Static);
            FrostHelperDreamBlockHook = new ILHook(dreamDashUpdate, OnFrostHelperDreamBlockHook);
        }
    }

    public static void OnFrostHelperDreamBlockHook(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(
            MoveType.After,
            ins => ins.MatchCall("Celeste.Audio", "Play")
            ))
        {
            Logger.Log(LogLevel.Info, ModuleName, "Hooked CustomDreamBlockV2.DreamDashUpdate.");
            cur.Index++;
            cur.EmitDelegate(() =>
            {
                if (!ModuleSession.EnabledFrostFreeze && !ModuleSettings.AlwaysEnableFrostFreeze) return;
                if (Engine.TimeRate > 0.25f)
                {
                    Celeste.Freeze(0.05f);
                }
            });
        }
    }

    public static void Unload()
    {
        On.Celeste.ParticleTypes.Load -= OnLoadParticles;
        if (FrostHelperLoaded)
        {
            FrostHelperDreamBlockHook.Dispose();
        }
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
            DirectionRange = 0f,
            Direction = 0f,
            SpeedMax = 0f,
            SpeedMin = 0f,
            Color = Calc.HexToColor("FFD500"),
            //Color2 = Calc.HexToColor("FFEF9E"),
            Color2 = Calc.HexToColor("FFFFFF"),
            ColorMode = ParticleType.ColorModes.Fade,
            LifeMin = 0.3f,
            LifeMax = 0.5f,

        };
    }

    public static void OnLoadParticles(On.Celeste.ParticleTypes.orig_Load orig)
    {
        orig();
        LoadParticles();
    }
}