using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class ChronoHookModule
{
    private static ILHook GravityBlockSequenceHook;
    private static Hook GravityBlockSwitchOnPlayerHook;

    public static void Load()
    {
        EverestModuleMetadata chronoHelper = new()
        {
            Name = "ChronoHelper",
            Version = new Version(1, 1, 4)
        };
        if (Everest.Loader.TryGetDependency(chronoHelper, out var module))
        {
            ThirdPartyHelpers.ChronoHelperInstalled = true;
            try
            {
                Type type = module.GetType().Assembly.GetType("Celeste.Mod.ChronoHelper.Entities.GravityFallingBlock");
                Type switchType = module.GetType().Assembly.GetType("Celeste.Mod.ChronoHelper.Entities.GravityFallingBlockSwitch");
                MethodInfo sequenceMethod = type.GetMethod("Sequence", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo onPlayerMethod = switchType.GetMethod("OnPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
                if (sequenceMethod is not null)
                {
                    GravityBlockSequenceHook = new(sequenceMethod.GetStateMachineTarget(), SequenceHook);
                    GravityBlockSwitchOnPlayerHook = new(onPlayerMethod, OnPlayerHook);
                }
                else
                {
                    Logger.Log(LogLevel.Error, ModuleName, $"ChronoHelper's GravityFallingBlock.Sequence method not found.");
                }
            }
            catch (Exception e)
            {
                Logger.LogDetailed(e, ModuleName);
            }
        }
    }

    private delegate void orig_OnPlayer(Entity self, Player player);
    private static void OnPlayerHook(orig_OnPlayer orig, Entity self, Player player)
    {
        if (ModuleSession.NoChronoCheckCycle)
            self.SceneAs<Level>().OnEndOfFrame += () => orig(self, player);
        else
            orig(self, player);
    }

    private static void SequenceHook(ILContext context)
    {
        ILCursor cur = new(context);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdcR4(0.1f), ins => ins.MatchBox<float>()))
            _ = cur.EmitDelegate<Func<object, object>>(f => ModuleSession.NoChronoCheckCycle ? null : f);
        cur.Index = 0;
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdcR4(0.2f), ins => ins.MatchBox<float>())
            && cur.TryGotoNext(MoveType.After, ins => ins.MatchLdcR4(0.2f), ins => ins.MatchBox<float>()))
            cur.EmitDelegate<Func<object, object>>(f => ModuleSession.NoChronoCheckCycle ? null : f);
    }

    public static void Unload()
    {
        GravityBlockSequenceHook?.Dispose();
        GravityBlockSwitchOnPlayerHook?.Dispose();
    }
}
