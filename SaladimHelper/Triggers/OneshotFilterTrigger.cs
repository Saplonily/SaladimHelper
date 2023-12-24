using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/OneshotFilterTrigger")]
public sealed class OneshotFilterTrigger : Trigger
{
    private float strengthFrom, strengthTo;
    private bool strengthFromCurrent;
    private float duration;
    private Ease.Easer easer;
    private bool once;

    private bool trigged;
    private FilterEntry entry;

    public OneshotFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        strengthFromCurrent = data.Bool("strength_from_current", false);
        duration = data.Float("duration", 5.0f);
        once = data.Bool("once", true);
        easer = Mapper.GetEaser(data.Attr("easing"));
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (once && trigged) return;
        trigged = true;
        float from = strengthFromCurrent ? entry.Strength : strengthFrom;
        Tween.Set(this, Tween.TweenMode.Oneshot, duration, easer, t =>
        {
            float eased = t.Eased;
            entry.Strength = (1.0f - eased) * from + eased * strengthTo;
        });
    }
}
