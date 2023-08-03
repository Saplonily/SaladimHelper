using Celeste.Mod.SaladimHelper.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class GlobalHooks
{
    public static void Load()
    {
        On.Celeste.ParticleTypes.Load += LoadParticles;
    }

    public static void Unload()
    {
        On.Celeste.ParticleTypes.Load -= LoadParticles;
    }

    public static void LoadParticles(On.Celeste.ParticleTypes.orig_Load orig)
    {
        orig();
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
}