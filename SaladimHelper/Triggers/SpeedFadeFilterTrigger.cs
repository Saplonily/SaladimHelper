using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/SpeedFadeFilterTrigger")]
public sealed class SpeedFadeFilterTrigger : Trigger
{
    private float strengthFrom, strengthTo;
    private float speedFrom, speedTo;
    private FilterEntry entry;

    public SpeedFadeFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        speedFrom = data.Float("speed_from", 0.0f);
        speedTo = data.Float("speed_to", 260.0f);
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        float speed = player.Speed.Length();
        entry.Strength = Calc.ClampedMap(speed, speedFrom, speedTo, strengthFrom, strengthTo);
    }
}
