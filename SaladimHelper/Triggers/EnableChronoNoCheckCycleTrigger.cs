using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/EnableChronoNoCheckCycleTrigger")]
public class EnableChronoNoCheckCycleTrigger : Trigger
{
    public bool isEnable = true;

    public EnableChronoNoCheckCycleTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        isEnable = data.Bool("is_enable", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        ModuleSession.SessionFlags.NoChronoCheckCycle = isEnable;
    }
}