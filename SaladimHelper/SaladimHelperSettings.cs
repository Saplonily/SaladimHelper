using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSettings : EverestModuleSettings
{
    private bool alwaysShowVivHiddenRooms = false;

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.A)]
    public ButtonBinding DoTeleport { get; set; } = new();

    public bool AlwaysEnableFrostFreeze { get; set; } = false;

    public bool AlwaysEnablePandorasBoxDreamDashBetterFreeze { get; set; } = false;

    public bool AlwaysShowVivHiddenRooms
    {
        get => alwaysShowVivHiddenRooms;
        set
        {
            alwaysShowVivHiddenRooms = value;
            if (value)
                VivHookModule.Load();
            else
                VivHookModule.Unload();
        }
    }
}