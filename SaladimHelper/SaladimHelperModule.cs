using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperModule : EverestModule
{
    public static SaladimHelperModule ModuleInstance { get; set; }
    public static SaladimHelperModuleSettings ModuleSettings => ModuleInstance._Settings as SaladimHelperModuleSettings;
    public override Type SettingsType => typeof(SaladimHelperModuleSettings);

    public SaladimHelperModule() => ModuleInstance = this;

    public const string ModuleName = "SaladimHelper";

    public static void CallInitMethods(string methodName)
    {
        Assembly asm = Assembly.GetAssembly(typeof(SaladimHelperModule));
        var types = asm.GetTypes().Where(t => t.GetCustomAttribute<NeedModuleInitAttribute>() is not null);
        foreach (var type in types)
            type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public)?.Invoke(null, null);
    }

    public override void Load()
    {
        Logger.Log(LogLevel.Info, ModuleName, "Loading module hooks...");
        CallInitMethods(nameof(Load));
    }

    public override void Unload()
    {
        Logger.Log(LogLevel.Info, ModuleName, "Unloading module hooks...");
        CallInitMethods(nameof(Unload));
    }
}
