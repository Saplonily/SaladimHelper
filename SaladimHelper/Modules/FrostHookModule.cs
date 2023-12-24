using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class FrostHookModule
{
    public static ILHook FrostHelperDreamBlockHook = null;

    public static void Load()
    {
        try
        {
            EverestModuleMetadata frostHelper = new()
            {
                Name = "FrostHelper",
                Version = new Version(1, 44, 0)
            };
            if (Everest.Loader.TryGetDependency(frostHelper, out var module))
            {
                Logger.Log(LogLevel.Info, ModuleName, "Found FrostHelper, hooking CustomDreamBlockV2.DreamDashUpdate...");
                Assembly asm = module.GetType().Assembly;
                Type dreamBlockType = asm.GetType("FrostHelper.CustomDreamBlockV2");
                MethodInfo dreamDashUpdate = dreamBlockType.GetMethod("Player_DreamDashUpdate", BindingFlags.NonPublic | BindingFlags.Static);
                FrostHelperDreamBlockHook = new ILHook(dreamDashUpdate, OnFrostHelperDreamDashUpdateHook);
            }
        }
        catch (Exception e)
        {
            Logger.LogDetailed(e, ModuleName);
            FrostHelperDreamBlockHook?.Dispose();
            FrostHelperDreamBlockHook = null;
        }
    }

    public static void Unload()
    {
        FrostHelperDreamBlockHook?.Dispose();
    }

    public static void OnFrostHelperDreamDashUpdateHook(ILContext il)
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
                    return false;
                DynamicData data = DynamicData.For(obj);
                Input.Dash.ConsumePress();
                Input.CrouchDash.ConsumePress();
                if (Engine.TimeRate > 0.25f)
                    Celeste.Freeze(0.05f);
                var co = MakeCoroutine(p, data);
                obj.Add(new Coroutine(co));
                return true;
            });
            var label = cur.DefineLabel();
            cur.Emit(OpCodes.Brfalse, label);
            cur.Emit(OpCodes.Ldarg_0);
            cur.Emit(OpCodes.Ldarg_1);
            cur.Emit(OpCodes.Callvirt, typeof(On.Celeste.Player.orig_DreamDashUpdate).GetMethod("Invoke"));
            cur.Emit(OpCodes.Ret);
            cur.MarkLabel(label);
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

}
