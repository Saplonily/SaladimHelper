namespace Celeste.Mod.SaladimHelper;

// contains methods that usually called from lua codes
// but i'm too lazy to add more methods
public static class LuaApi
{
    public static Tween TweenCamera(Player player, float x, float y, float duration, string easer, string tweenMode)
    {
        Level l = player.SceneAs<Level>();
        var ppos = l.Camera.Position;
        var tpos = ppos + new Vector2(x, y);
        return Tween.Set(player, Mapper.GetTweenMode(tweenMode), duration, Mapper.GetEaser(easer), t =>
        {
            l.Camera.Position = Vector2.Lerp(ppos, tpos, t.Eased);
        });
    }

    public static Tween TweenCamera(Player player, float x, float y, float duration, string easer)
        => TweenCamera(player, x, y, duration, easer, null);
}
