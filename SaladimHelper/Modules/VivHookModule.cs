using System.Reflection;
using Celeste.Editor;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class VivHookModule
{
    public static ILHook VivHelperHideRoomHook = null;

    public static void Initialize()
    {
        if (!ModuleSettings.AlwaysShowVivHiddenRooms) return;
        try
        {
            EverestModuleMetadata vivHelper = new()
            {
                Name = "VivHelper",
                Version = new Version(1, 12, 0)
            };
            if (Everest.Loader.TryGetDependency(vivHelper, out var vmodule))
            {
                ThirdPartyHelpers.VivHelperInstalled = true;
                Logger.Log(LogLevel.Debug, ModuleName, "Found VivHelper, hooking MapEditor_ctor...");
                Assembly asm = vmodule.GetType().Assembly;
                Type type = asm.GetType("VivHelper.Entities.SpawnPointHooks");
                MethodInfo method = type.GetMethod("MapEditor_ctor", BindingFlags.NonPublic | BindingFlags.Static);
                VivHelperHideRoomHook = new ILHook(method, OnVivHelperCheckHideRoom);
            }
        }
        catch (Exception e)
        {
            Logger.LogDetailed(e, ModuleName);
            VivHelperHideRoomHook?.Dispose();
            VivHelperHideRoomHook = null;
        }
    }

    public static void Unload()
    {
        VivHelperHideRoomHook?.Dispose();
    }

    public static void OnVivHelperCheckHideRoom(ILContext il)
    {
        ILCursor cur = new(il);

        if (cur.TryGotoNext(ins => ins.MatchCallvirt<List<LevelTemplate>>("Remove")))
        {
            cur.Emit(OpCodes.Pop);
            cur.Emit(OpCodes.Ldnull);
        }
    }
}
