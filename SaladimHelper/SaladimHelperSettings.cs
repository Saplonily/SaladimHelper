using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSettings : EverestModuleSettings
{
    private bool alwaysShowVivHiddenRooms = false;

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.A)]
    public ButtonBinding DoTeleport { get; set; } = new();

    [SettingSubText("modoptions_saladimhelper_alwaysenablefrostfreeze_subtext")]
    public bool AlwaysEnableFrostFreeze { get; set; } = false;

    [SettingSubText("modoptions_saladimhelper_alwaysenablepandorasboxdreamdashbetterfreeze_subtext")]
    public bool AlwaysEnablePandorasBoxDreamDashBetterFreeze { get; set; } = false;

    [SettingSubText("modoptions_saladimhelper_alwaysshowvivhiddenrooms_subtext")]
    public bool AlwaysShowVivHiddenRooms
    {
        get => alwaysShowVivHiddenRooms;
        set
        {
            alwaysShowVivHiddenRooms = value;
            if (value)
                VivHookModule.Initialize();
            else
                VivHookModule.Unload();
        }
    }
}