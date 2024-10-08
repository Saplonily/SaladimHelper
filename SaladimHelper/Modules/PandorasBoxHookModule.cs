﻿using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper;

[SaladimModule]
public static class PandorasBoxHookModule
{
    public static ILHook DreamDashRedirectILHook;

    public static void Initialize()
    {
        // System.Boolean Celeste.Mod.PandorasBox.DreamDashController::dreamDashRedirect(Celeste.Player)
        try
        {
            EverestModuleMetadata pandorasBox = new()
            {
                Name = "PandorasBox",
                Version = new Version(1, 0, 47)
            };
            if (Everest.Loader.TryGetDependency(pandorasBox, out var module))
            {
                Logger.Log(LogLevel.Debug, ModuleName, "Found PandorasBox, hooking DreamDashController.dreamDashRedirect...");
                Assembly asm = module.GetType().Assembly;
                Type controllerType = asm.GetType("Celeste.Mod.PandorasBox.DreamDashController");
                MethodInfo method = controllerType.GetMethod("dreamDashRedirect", BindingFlags.Instance | BindingFlags.NonPublic);
                DreamDashRedirectILHook = new(method, OnPandorasBoxDreamDashControllerHook);
            }
        }
        catch (Exception e)
        {
            Logger.LogDetailed(e, ModuleName);
            DreamDashRedirectILHook?.Dispose();
            DreamDashRedirectILHook = null;
        }
    }

    public static void Unload()
    {
        DreamDashRedirectILHook?.Dispose();
    }

    public static void OnPandorasBoxDreamDashControllerHook(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdcR4(0.05f), ins => ins.MatchCall<Celeste>("Freeze")))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.Emit(OpCodes.Ldarg_1);
            cur.Emit(OpCodes.Ldloc_2);
            cur.EmitDelegate((DreamBlock dreamBlock, Player player, bool flag) =>
            {
                DynamicData data = DynamicData.For(dreamBlock);

                if (ModuleSession.SessionFlags.EnabledBetterPandorasFreeze)
                    player.Add(new Coroutine(MakeDoRealRedirectRoutine(dreamBlock, player, flag)));
                else
                    DoRealRedirect(dreamBlock, player, flag);
                return true;

                void DoRealRedirect(DreamBlock dreamBlock, Player player, bool flag)
                {
                    if (flag)
                    {
                        float sameDirectionSpeedMultiplier = data.Get<float>("sameDirectionSpeedMultiplier");
                        player.Speed *= sameDirectionSpeedMultiplier;
                        player.DashDir *= Math.Sign(sameDirectionSpeedMultiplier);
                    }
                    else
                    {
                        player.DashDir = Input.GetAimVector(Facings.Right);
                        player.Speed = player.DashDir * player.Speed.Length();
                    }
                    Input.Dash.ConsumeBuffer();
                }

                IEnumerator MakeDoRealRedirectRoutine(DreamBlock dreamBlock, Player player, bool flag)
                {
                    yield return null;
                    DoRealRedirect(dreamBlock, player, flag);
                    yield break;
                }
            });
            cur.Emit(OpCodes.Ret);
        }
    }
}
