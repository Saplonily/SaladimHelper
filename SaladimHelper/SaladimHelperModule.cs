using System.Reflection;
using Celeste.Mod.SaladimHelper.Entities;
using Celeste.Mod.SaladimHelper.Triggers;

namespace Celeste.Mod.SaladimHelper;

public sealed class SaladimHelperModule : EverestModule
{
    public const string ModuleName = nameof(SaladimHelper);

    public static SaladimHelperModule ModuleInstance { get; set; }

    public static SaladimHelperSession ModuleSession => ModuleInstance._Session as SaladimHelperSession;
    public override Type SessionType => typeof(SaladimHelperSession);

    public override void Load()
    {
        ModuleInstance = this;
#if DEBUG
        Logger.SetLogLevel(ModuleName, LogLevel.Verbose);
#endif
        ExplodeFocusArea.Load();
        DelayedFallingBlock.Load();
        ReelCamera.Load();
        CommonModule.Load();
        BitsMagicLanternController.Load();
        BitsMomentumRefill.Load();
        CoinCounter.Load();
        GDDShop.Load();
        RustyZipMover.Load();
        TransitionEasingTrigger.Load();
    }

    public override void Unload()
    {
        ExplodeFocusArea.Unload();
        DelayedFallingBlock.Unload();
        ReelCamera.Unload();
        CommonModule.Unload();
        BitsMagicLanternController.Unload();
        BitsMomentumRefill.Unload();
        CoinCounter.Unload();
        GDDShop.Unload();
        RustyZipMover.Unload();
        TransitionEasingTrigger.Unload();

        ChronoHookModule.Unload();
        PandorasBoxHookModule.Unload();
        FrostHookModule.Unload();
    }

    public override void Initialize()
    {
        ThirdPartyHelpers.Initialize();
        ChronoHookModule.Initialize();
        FrostHookModule.Initialize();
        PandorasBoxHookModule.Initialize();
    }
}
