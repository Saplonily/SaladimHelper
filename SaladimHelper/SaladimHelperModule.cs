using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperModule : EverestModule
{
    public static SaladimHelperModule Instance { get; set; }
    public static SaladimHelperModuleSettings Settings => Instance._Settings as SaladimHelperModuleSettings;
    public override Type SettingsType => typeof(SaladimHelperModuleSettings);

    public SaladimHelperModule() => Instance = this;

    public const string Name = "SaladimHelper";

    public static void CallInitMethods(string methodName)
    {
        Assembly asm = Assembly.GetAssembly(typeof(SaladimHelperModule));
        var types = asm.GetTypes().Where(t => t.GetCustomAttribute<NeedModuleInitAttribute>() is not null);
        foreach (var type in types)
            type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public)?.Invoke(null, null);
    }

    public override void Load()
    {
        Logger.Log(LogLevel.Info, Name, "Loading module hooks...");
        CallInitMethods(nameof(Load));
    }

    public override void Unload()
    {
        Logger.Log(LogLevel.Info, Name, "Unloading module hooks...");
        CallInitMethods(nameof(Unload));
    }
}
