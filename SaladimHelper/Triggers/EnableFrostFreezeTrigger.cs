using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper;

[CustomEntity("SaladimHelper/EnableFrostFreezeTrigger")]
public class EnableFrostFreezeTrigger : Trigger
{
    public bool isEnable = true;

    public EnableFrostFreezeTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        isEnable = data.Bool("is_enable", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        ModuleSession.EnabledFrostFreeze = isEnable;
    }
}