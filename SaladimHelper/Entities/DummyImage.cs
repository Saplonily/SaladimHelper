using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

// yes, just these, cuz it's dummy!
[CustomEntity($"SaladimHelper/DummyImage")]
public class DummyImage : Entity
{
    public DummyImage(EntityData data, Vector2 offset)
        : this(
              data.Position + offset,
              new(data.Width, data.Height),
              new(data.Int("offset_x"), data.Int("offset_y")),
              data.Attr("texture"),
              data.Int("depth")
              )
    {
    }

    public DummyImage(Vector2 position, Vector2 size, Vector2 offset, string gpTex, int depth)
    {
        Depth = depth;
        Position = position;
        Collider = new Hitbox(size.X, size.Y);

        var img = new Image(GFX.Game[gpTex]);
        img.Position += offset;
        Add(img);
    }
}