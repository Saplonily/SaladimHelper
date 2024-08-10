using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperModule : EverestModule
{
    public static SaladimHelperModule ModuleInstance { get; set; }

    //public static SaladimHelperSettings ModuleSettings => ModuleInstance._Settings as SaladimHelperSettings;
    //public override Type SettingsType => typeof(SaladimHelperSettings);

    public static SaladimHelperSession ModuleSession => ModuleInstance._Session as SaladimHelperSession;
    public override Type SessionType => typeof(SaladimHelperSession);

    public SaladimHelperModule() => ModuleInstance = this;

    public const string ModuleName = nameof(SaladimHelper);

    public static void CallInitMethods(string methodName)
    {
        Assembly asm = Assembly.GetAssembly(typeof(SaladimHelperModule));
        var types = asm.GetTypes().Where(t => t.GetCustomAttribute<SaladimModuleAttribute>() is not null);
        foreach (var type in types)
            type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public)?.Invoke(null, null);
    }

    public override void Load()
    {
        Logger.Log(LogLevel.Debug, ModuleName, "Invoking module Load methods...");
        CallInitMethods(nameof(Load));
    }

    public override void Initialize()
    {
        Logger.Log(LogLevel.Debug, ModuleName, "Invoking module Initialize methods...");
        CallInitMethods(nameof(Initialize));
    }

    public override void Unload()
    {
        Logger.Log(LogLevel.Debug, ModuleName, "Invoking module Unload methods...");
        CallInitMethods(nameof(Unload));
    }
}
