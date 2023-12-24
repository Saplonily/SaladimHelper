#if DEBUG
namespace Celeste.Mod.SaladimHelper.Entities;
public class TestEntity : Entity
{
    public TestEntity(Vector2 position)
    {
        Depth = -114514;
        Position = position;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Render()
    {
        base.Render();
    }
}
#endif