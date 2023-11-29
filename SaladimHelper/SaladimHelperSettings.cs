using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSettings : EverestModuleSettings
{
    [DefaultButtonBinding(Buttons.RightShoulder, Keys.X)]
    public ButtonBinding DoTeleport { get; set; } = new();

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.X)]
    public ButtonBinding DoLightSwitch { get; set; } = new();

    public bool AlwaysEnableFrostFreeze { get; set; }
}