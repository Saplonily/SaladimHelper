using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/OneshotFilterTrigger")]
public sealed class OneshotFilterTrigger : Trigger
{
    private float strengthFrom, strengthTo;
    private float duration;
    private bool trigged;
    private FilterEntry entry;
    private Ease.Easer easer;

    public OneshotFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        duration = data.Float("duration", 5.0f);
        easer = FilterEntry.GetEaserWithName(data.Attr("easing"));
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (!trigged)
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
