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
        BitsMomentumRefill.P_ShatterTwo = new(Refill.P_ShatterTwo);
        BitsMomentumRefill.P_RegenTwo = new(Refill.P_RegenTwo);
        BitsMomentumRefill.P_GlowTwo = new(Refill.P_GlowTwo);
    }
}