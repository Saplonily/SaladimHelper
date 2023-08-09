using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.SaladimHelper;

[SettingName("Saladim helper settings")]
public class SaladimHelperSettings : EverestModuleSettings
{
    [DefaultButtonBinding(Buttons.RightShoulder, Keys.X)]
    public ButtonBinding DoTeleport { get; set; } = new(); 
    
    [DefaultButtonBinding(Buttons.RightShoulder, Keys.X)]
    public ButtonBinding DoLightSwitch { get; set; } = new();
}
