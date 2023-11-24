using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/StaticFilterTrigger")]
public sealed class StaticFilterTrigger : Trigger
{
    private float strength;
    private FilterEntry entry;

    public StaticFilterTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        strength = data.Float("strength", 100.0f);
        entry = ModuleSession.GetFilterEntry(data.Attr("effect_path"), data.Float("index", 0.0f));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        entry.Strength = strength;
    }
}
