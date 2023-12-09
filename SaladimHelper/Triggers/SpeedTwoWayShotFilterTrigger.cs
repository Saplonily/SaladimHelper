using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper;

[CustomEntity("SaladimHelper/SpeedTwoWayShotFilterTrigger")]
public sealed class SpeedTwoWayShotFilterTrigger : Trigger
{
    private float strengthFrom, strengthTo;
    private float speedThreshold, speedFallThreshold;
    private float duration;
    private bool trigged;
    private FilterEntry entry;
    private Ease.Easer easer;
    private Tween tween;

    public SpeedTwoWayShotFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        speedFallThreshold = data.Float("speed_fall_threshold", 200.0f);
        speedThreshold = data.Float("speed_threshold", 500.0f);
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        duration = data.Float("duration", 5.0f);
        easer = Mapper.GetEaser(data.Attr("easing"));
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        float speed = player.Speed.Length();
        if (!trigged && speed > speedThreshold)
        {
            trigged = true;
            if (tween is not null && tween.Active)
                tween.Active = false;
            float preStrength = entry.Strength;
            tween = Tween.Set(this, Tween.TweenMode.Oneshot, duration, easer, t =>
            {
                float eased = t.Eased;
                entry.Strength = (1.0f - eased) * preStrength + eased * strengthTo;
            });
        }
        else if (trigged && speed < speedFallThreshold)
        {
            trigged = false;
            if (tween is not null && tween.Active)
                tween.Active = false;
            float preStrength = entry.Strength;
            tween = Tween.Set(this, Tween.TweenMode.Oneshot, duration, easer, t =>
            {
                float eased = t.Eased;
                entry.Strength = (1.0f - eased) * preStrength + eased * strengthFrom;
            });
        }
    }
}
