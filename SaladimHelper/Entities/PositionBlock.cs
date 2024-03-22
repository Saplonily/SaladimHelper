using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/PositionBlock")]
public class PositionBlock : Solid
{
    private readonly float range;
    private readonly float speed;
    private readonly Ease.Easer easer;
    private readonly bool leftToRight; // else topToBottom
    private readonly Vector2 startPosition;

    public PositionBlock(EntityData data, Vector2 offset)
        : this(
              data.Position + offset,
              data.Char("tiletype", '3'),
              data.Width, data.Height,
              data.Bool("leftToRight", true), data.Float("range", -16f),
              data.Float("speed", 128f), Mapper.GetEaser(data.Attr("easing"))
              )
    {
    }

    public PositionBlock(Vector2 position, char tile, int width, int height, bool leftToRight, float range, float speed, Ease.Easer easer)
        : base(position, width, height, false)
    {
        Add(GFX.FGAutotiler.GenerateBox(tile, width / 8, height / 8).TileGrid);
        startPosition = position;
        this.leftToRight = leftToRight;
        this.range = range;
        this.speed = speed;
        this.easer = easer;
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        if (player is null) return;
        if (leftToRight)
        {
            float yFrom = startPosition.Y;
            float yTo = startPosition.Y + range;
            float xFrom = Left;
            float xTo = Right;
            float lerp = (player.CenterX - xFrom) / (xTo - xFrom);
            lerp = MathHelper.Clamp(lerp, 0f, 1f);
            float targetY = MathHelper.Lerp(yFrom, yTo, easer(lerp));
            MoveTowardsY(targetY, speed * Engine.DeltaTime);
        }
        else
        {
            float yFrom = Top;
            float yTo = Bottom;
            float xFrom = startPosition.X;
            float xTo = startPosition.X + range;
            float lerp = (player.Bottom - yFrom) / (yTo - yFrom);
            lerp = MathHelper.Clamp(lerp, 0f, 1f);
            float targetX = MathHelper.Lerp(xFrom, xTo, easer(lerp));
            MoveTowardsX(targetX, speed * Engine.DeltaTime);
        }
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        float x = startPosition.X;
        float y = startPosition.Y;
        float w = Width;
        float h = Height;
        if (!leftToRight)
        {
            if (range >= 0)
            { w += range; }
            else
            { w -= range; x += range; }
        }
        else
        {
            if (range >= 0)
            { h += range; }
            else
            { h -= range; y += range; }
        }
        Draw.HollowRect(new Vector2(x, y), w, h, Color.Wheat);
    }
}