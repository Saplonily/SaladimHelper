using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/ExplodeFocusArea"), Tracked]
public partial class ExplodeFocusArea : Entity
{
    public enum FocusType
    {
        EightWay,
        FourWay,
        DiagonalFourWay,
        HorizontalTwoWay,
        VerticalTwoWay
    }

    private readonly FocusType focusType;
    private readonly float rotation;

    public ExplodeFocusArea(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, (FocusType)data.Int("focusType"), MathHelper.ToRadians(data.Float("rotation")))
    {
    }

    public ExplodeFocusArea(Vector2 position, int width, int height, FocusType focusType, float rotation)
        : base(position)
    {
        this.focusType = focusType;
        this.rotation = rotation;
        Collider = new Hitbox(width, height);
    }
}
