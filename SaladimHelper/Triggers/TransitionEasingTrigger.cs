using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Triggers;

// function implemented in .hook.cs

[CustomEntity("SaladimHelper/TransitionEasingTrigger"), Tracked]
public partial class TransitionEasingTrigger : Trigger
{
    private readonly Ease.Easer easing;
    private readonly float threshold;
    private readonly float duration;
    private readonly float speed;

    public TransitionEasingTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        speed = data.Float("speed");
        easing = Mapper.GetEaser(data.Attr("easing"));
        duration = data.Float("duration");
        threshold = data.Float("threshold");
    }
}