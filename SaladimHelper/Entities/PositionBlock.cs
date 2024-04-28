using Celeste.Mod.Entities;
using static Celeste.DashSwitch;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/PositionBlock")]
public class PositionBlock : Solid
{
    private readonly float speed;
    private readonly float range;
    private readonly Ease.Easer easer;
    private readonly Vector2 startPosition;
    private readonly Vector2 targetPosition;

    public PositionBlock(EntityData data, Vector2 offset)
        : this(
              data.Position + offset,
              data.Char("tiletype", '3'),
              data.Width, data.Height,
              data.NodesOffset(offset)[0],
              data.Float("speed", 128f), data.Float("range", 0f),
              Mapper.GetEaser(data.Attr("easing"))
              )
    {
    }

    public PositionBlock(Vector2 position, char tile, int width, int height, Vector2 targetPosition, float speed, float range, Ease.Easer easer)
        : base(position, width, height, false)
    {
        Add(GFX.FGAutotiler.GenerateBox(tile, width / 8, height / 8).TileGrid);
        startPosition = position;
        this.targetPosition = targetPosition;
        this.speed = speed;
        this.range = range;
        this.easer = easer;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Player player = Scene.Tracker.GetEntity<Player>();
        if (player is null) return;

        // FIXME: weird lift speed
        //MoveVCollideSolids(GetYPosition(player) - ExactPosition.Y, true);
        //MoveHCollideSolids(GetXPosition(player) - ExactPosition.X, true);
        float px = X;
        float py = Y;
        X = GetXPosition(player);
        Y = GetYPosition(player);
        MoveStaticMovers(new(X - px, Y - py));
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        if (player is null) return;

        // FIXME: weird lift speed
        //MoveVCollideSolids(Math.Min(GetYPosition(player) - ExactPosition.Y, speed * Engine.DeltaTime), true);
        //MoveHCollideSolids(Math.Min(GetXPosition(player) - ExactPosition.X, speed * Engine.DeltaTime), true);
        MoveTowardsX(GetXPosition(player), speed * Engine.DeltaTime);
        MoveTowardsY(GetYPosition(player), speed * Engine.DeltaTime);
    }

    private float GetXPosition(Player player)
    {
        float xLerpFrom = startPosition.X;
        float xLerpTo = targetPosition.X;
        float yCheckFrom = Y - range;
        float yCheckTo = Y + Height + range;
        float lerp = (player.Bottom - yCheckFrom) / (yCheckTo - yCheckFrom);
        lerp = MathHelper.Clamp(lerp, 0f, 1f);
        return MathHelper.Lerp(xLerpFrom, xLerpTo, easer(lerp));
    }

    private float GetYPosition(Player player)
    {
        float yLerpFrom = startPosition.Y;
        float yLerpTo = targetPosition.Y;
        float xCheckFrom = X - range;
        float xCheckTo = X + Width + range;
        float lerp = (player.CenterX - xCheckFrom) / (xCheckTo - xCheckFrom);
        lerp = MathHelper.Clamp(lerp, 0f, 1f);
        return MathHelper.Lerp(yLerpFrom, yLerpTo, easer(lerp));
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        Draw.HollowRect(startPosition, Width, Height, Color.Wheat);
        Draw.HollowRect(targetPosition, Width, Height, Color.Wheat);
    }
}