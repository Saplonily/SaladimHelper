namespace Celeste.Mod.SaladimHelper;

[MonoMod.ModInterop.ModExportName("SaladimHelper.Exports")]
public static class SaladimHelperModuleExports
{
    public static int GetCoinsCount() => ModuleSession.CollectedCoinsCount;
}
