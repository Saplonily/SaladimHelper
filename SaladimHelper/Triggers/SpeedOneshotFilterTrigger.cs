using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper;

[CustomEntity("SaladimHelper/SpeedOneshotFilterTrigger")]
public sealed class SpeedOneshotFilterTrigger : Trigger
{
    private float strengthFrom, strengthTo;
    private float speedThreshold;
    private float duration;
    private bool trigged;
    private FilterEntry entry;
    private Ease.Easer easer;

    public SpeedOneshotFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        speedThreshold = data.Float("speed_threshold", 500.0f);
        duration = data.Float("duration", 5.0f);
        easer = FilterEntry.GetEaserWithName(data.Attr("easing"));
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        float speed = player.Speed.Length();
        if (!trigged && speed > speedThreshold)
        {
            trigged = true;
            Tween.Set(this, Tween.TweenMode.Oneshot, duration, easer, t =>
            {
                float eased = t.Eased;
                entry.Strength = (1.0f - eased) * strengthFrom + eased * strengthTo;
            });
        }
    }
}
