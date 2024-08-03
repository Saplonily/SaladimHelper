using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/EnableBetterPandorasFreezeTrigger")]
public class EnableBetterPandorasFreezeTrigger : Trigger
{
    public bool isEnable = true;

    public EnableBetterPandorasFreezeTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        isEnable = data.Bool("is_enable", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        ModuleSession.SessionFlags.EnabledBetterPandorasFreeze = isEnable;
    }
}