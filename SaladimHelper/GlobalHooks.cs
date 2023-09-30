using System.Reflection;

using Celeste.Mod.SaladimHelper.Entities;

using Mono.Cecil.Cil;

using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class GlobalHooks
{
    public static bool FrostHelperLoaded;
    public static bool DeathTrackerLoaded;
    public static ILHook FrostHelperDreamBlockHook = null;

    public static void Load()
    {
        On.Celeste.ParticleTypes.Load += OnLoadParticles;

        EverestModuleMetadata frostHelper = new() { Name = "FrostHelper", Version = new Version(1, 44, 0) };
        if (Everest.Loader.TryGetDependency(frostHelper, out var module))
        {
            Logger.Log(LogLevel.Info, ModuleName, "Found FrostHelper, hooking CustomDreamBlockV2.DreamDashUpdate...");
            FrostHelperLoaded = true;
            Assembly asm = module.GetType().Assembly;
            Type dreamBlockType = asm.GetType("FrostHelper.CustomDreamBlockV2");
            MethodInfo dreamDashUpdate = dreamBlockType.GetMethod("Player_DreamDashUpdate", BindingFlags.NonPublic | BindingFlags.Static);
            FrostHelperDreamBlockHook = new ILHook(dreamDashUpdate, OnFrostHelperDreamBlockHook);
        }

        EverestModuleMetadata deathTracker = new() { Name = "DeathTracker", Version = new Version(1, 0, 0) };
        if (Everest.Loader.DependencyLoaded(deathTracker))
            DeathTrackerLoaded = true;
    }

    public static void OnFrostHelperDreamBlockHook(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(ins => ins.MatchCallvirt<Player>("get_CanDash")))
        {
            cur.Index += 2;
            cur.Emit(OpCodes.Ldloc_0);
            cur.Emit(OpCodes.Ldarg_1);
            cur.EmitDelegate((Entity obj, Player p) =>
            {
                if (!ModuleSession.EnabledFrostFreeze && !ModuleSettings.AlwaysEnableFrostFreeze)
                    return;
                DynamicData data = DynamicData.For(obj);
                Input.Dash.ConsumePress();
                Input.CrouchDash.ConsumePress();
                if (Engine.TimeRate > 0.25f)
                    Celeste.Freeze(0.05f);
                var co = MakeCoroutine(p, data);
                obj.Add(new Coroutine(co));
            });
            cur.Emit(OpCodes.Ldarg_0);
            cur.Emit(OpCodes.Ldarg_1);
            cur.Emit(OpCodes.Callvirt, typeof(On.Celeste.Player.orig_DreamDashUpdate).GetMethod("Invoke"));
            cur.Emit(OpCodes.Ret);
        }
        static IEnumerator MakeCoroutine(Player self, DynamicData data)
        {

            yield return null;
            Vector2 aimVector = Input.GetAimVector(self.Facing);
            bool flag = aimVector == self.DashDir;
            if (!flag || data.Get<bool>("AllowRedirectsInSameDir"))
            {
                self.DashDir = aimVector;
                self.Speed = self.DashDir * self.Speed.Length();
                self.Dashes = Math.Max(0, self.Dashes - 1);
                Audio.Play("event:/char/madeline/dreamblock_enter");
                if (flag)
                {
                    self.Speed *= data.Get<float>("SameDirectionSpeedMultiplier");
                    self.DashDir *= (float)Math.Sign(data.Get<float>("SameDirectionSpeedMultiplier"));
                }
                if (self.Speed.X != 0f)
                    self.Facing = (Facings)Math.Sign(self.Speed.X);
            }
            yield break;
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