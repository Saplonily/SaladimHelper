using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper;

[CustomEntity("SaladimHelper/FadeFilterTrigger")]
public sealed class FadeFilterTrigger : Trigger
{
    private bool leftToRight;
    private float strengthFrom;
    private float strengthTo;
    private FilterEntry entry;

    public FadeFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strengthFrom = data.Float("strength_from", 0.0f);
        strengthTo = data.Float("strength_to", 100.0f);
        leftToRight = data.Bool("left_to_right", true);
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (leftToRight)
            entry.Strength = Calc.ClampedMap(player.Center.X, Left, Right, strengthFrom, strengthTo);
        else
            entry.Strength = Calc.ClampedMap(player.Center.Y, Top, Bottom, strengthFrom, strengthTo);
    }
}
