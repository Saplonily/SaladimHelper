namespace Celeste.Mod.SaladimHelper.Entities;

public class ShopBoughtLeader : Entity
{
    private Vector2 from;
    private Vector2 to;
    private float dir;
    private float distance;

    private Entity entityToAdd;

    public ShopBoughtLeader(Vector2 from, Vector2 to, Entity entityToAdd)
    {
        (this.from, this.to) = (from, to);
        this.entityToAdd = entityToAdd;
        dir = (to - from).Angle();
        distance = (to - from).Length();

        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, distance / 120f);
        tween.OnUpdate = t =>
        {
            Position = Vector2.Lerp(this.from, this.to, t.Eased);
            SceneAs<Level>().ParticlesFG.Emit(Player.P_SummitLandA, Position, dir);
        };
        tween.OnComplete = _ =>
        {
            Scene.Add(this.entityToAdd);
            for (int i = 0; i < 20; i++)
                SceneAs<Level>().ParticlesFG.Emit(Player.P_SummitLandB, Position);
            RemoveSelf();
        };
        Add(tween);
        tween.Start();
    }
}