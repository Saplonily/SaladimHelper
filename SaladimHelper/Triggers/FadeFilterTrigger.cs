using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/FadeFilterTrigger")]
public sealed class FadeFilterTrigger : Trigger
{
    private bool leftToRight;
    private float strengthFrom;
    private float strengthTo;
    private FilterEntry entry;
    private Ease.Easer easer;

    public FadeFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        leftToRight = data.Bool("left_to_right", true);
        easer = Mapper.GetEaser(data.Attr("easing"));
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (leftToRight)
            entry.Strength = EasedClampedMap(player.Center.X, Left, Right, strengthFrom, strengthTo, easer);
        else
            entry.Strength = EasedClampedMap(player.Center.Y, Top, Bottom, strengthFrom, strengthTo, easer);
    }

    public static float EasedClampedMap(float value, float min, float max, float newMin, float newMax, Ease.Easer easer)
        => easer(MathHelper.Clamp((value - min) / (max - min), 0f, 1f)) * (newMax - newMin) + newMin;
}
