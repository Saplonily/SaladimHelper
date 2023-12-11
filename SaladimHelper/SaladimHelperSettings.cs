using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSettings : EverestModuleSettings
{
    [DefaultButtonBinding(Buttons.RightShoulder, Keys.A)]
    public ButtonBinding DoTeleport { get; set; } = new();

    public bool AlwaysEnableFrostFreeze { get; set; }
}