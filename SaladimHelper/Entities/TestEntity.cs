namespace Celeste.Mod.SaladimHelper.Entities;

public class TestEntity : Entity
{
    public TestEntity(Vector2 position)
    {
        Depth = -114514;
        var lr = new LavaRect(640, 32, 2)
        {
            Fade = 10f,
            SurfaceColor = Color.White,
            EdgeColor = Color.Red,
            CenterColor = Color.Blue,
            OnlyMode = LavaRect.OnlyModes.OnlyTop,
            
        };
        Add(lr);
        Position = position;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Position = (scene as Level).Tracker.GetEntity<Player>().Position;
    }
}
