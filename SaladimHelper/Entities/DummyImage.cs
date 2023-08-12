using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

// yes, just these, cuz it's dummy!
[CustomEntity($"{ModuleName}/DummyImage")]
public class DummyImage : Entity
{
    public DummyImage(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Attr("texture"))
    {

    }

    public DummyImage(Vector2 position, float width, float height, string gpTex)
    {
        Position = position;
        Collider = new Hitbox(width, height);

        Add(new Image(GFX.Game[gpTex]));
    }
}