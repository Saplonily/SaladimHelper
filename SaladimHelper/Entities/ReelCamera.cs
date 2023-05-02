using System.Collections;
using System.Linq;
using System.Text;
using Celeste.Mod.Entities;
using YamlDotNet.Core.Tokens;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity($"{ModuleName}/ReelCamera"), Tracked]
public partial class ReelCamera : Entity
{
    public Vector2[] Nodes;
    public Vector2 CameraPosition;

    public bool SquashHorizontalArea = true;

    public float[] MoveTimes;
    public float[] Delays;
    public float StartDelay;
    public float StartMoveTime;

    public bool SetOffsetOnFinished;
    public Vector2 OnFinishedOffset;

    public bool Delaying = false;

    public static ReelCamera ActivedReelCamera;
    public static Player PlayerDoingReel;

    public ReelCamera(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(data.Width, data.Height);
        SquashHorizontalArea = data.Bool("squash_horizontal_area", true);
        SetOffsetOnFinished = data.Bool("set_offset_on_finished", false);
        OnFinishedOffset = new(data.Float("offset_x", 0f), data.Float("offset_y", 0f));

        Nodes = data.NodesOffset(offset);
        for (int i = 0; i < Nodes.Length; i++)
            Nodes[i] = Nodes[i] with { X = Nodes[i].X + Width / 2, Y = Nodes[i].Y + Height / 2 };

        StartDelay = data.Float("start_delay", 1.0f);
        StartMoveTime = data.Float("start_move_time", 1.0f);
        try
        {
            MoveTimes = ParseFloatSequence(data.Attr("move_time_sequence"));
            Delays = ParseFloatSequence(data.Attr("delay_sequence"));
            if (MoveTimes.Length != Nodes.Length - 1)
                throw new Exception($"MoveTime sequence load failed. Expect {Nodes.Length - 1} " +
                    $"but got {MoveTimes.Length}");
            if (Delays.Length != Nodes.Length - 1)
                throw new Exception($"Delay sequence load failed. Expect {Nodes.Length - 1} " +
                    $"but got {Delays.Length}");

            StringBuilder sb = new(15);
            sb.Append(string.Join(",", MoveTimes));
            sb.Append(@"  |  ");
            sb.Append(string.Join(",", Delays));
            Logger.Log(LogLevel.Info, "SaladimHelper", $"Loaded ReelCamera: {sb}");
        }
        catch (Exception e)
        {
            throw new Exception($"Loading sequence failed. Maybe failed parsing numbers. Inner msg:{e.Message}");
        }
    }

    public ReelCamera(
        Vector2 position, Vector2 size,
        Vector2[] nodes,
        float[] moveTimes, float[] delaySequence,
        float startDelay, float startMoveTime
        )
        : base()
    {
        Collider = new Hitbox(size.X, size.Y);
        Position = position;

        Nodes = nodes;
        MoveTimes = moveTimes;
        Delays = delaySequence;
        StartDelay = startDelay;
        StartMoveTime = startMoveTime;
    }

    protected static float[] ParseFloatSequence(string sequenceStr)
        => sequenceStr.Split(',').Select(float.Parse).ToArray();

    public override void Update()
    {
        base.Update();

        Level level = SceneAs<Level>();
        var c = level.Camera;
        if (ActivedReelCamera == this)
        {
            if (Delaying && PlayerDoingReel is not null)
            {
                DoPlayerDieCheck(level, PlayerDoingReel, SquashHorizontalArea, false);
            }
        }
        else
        {
            Player player = CollideFirst<Player>();
            if (player is not null && ActivedReelCamera is null)
            {
                PlayerDoingReel = player;
                ActivedReelCamera = this;

                Add(new Coroutine(MakeMovingCoroutine(level, player)));
                CameraPosition = c.Position + new Vector2(c.Right - c.Left, c.Bottom - c.Top) / 2;
            }
        }
    }

    public IEnumerator MakeMovingCoroutine(Level level, Player player)
    {
        PlayerDoingReel = player;
        Camera c = level.Camera;
        bool startTweenCompleted = false;
        Vector2 startFrom = CameraPosition;
        Vector2 startTo = Nodes[0];
        Tween startTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, StartMoveTime);
        startTween.OnUpdate = t => CameraPosition = Calc.LerpSnap(startFrom, startTo, t.Eased);
        startTween.OnComplete = _ => startTweenCompleted = true;
        Add(startTween);
        startTween.Start();
        Vector2 ps = startTo - startFrom;
        while (!startTweenCompleted)
        {
            if (DoPlayerDieCheck(level, player, SquashHorizontalArea, ps.Y > 0))
                yield break;
            yield return null;
        }
        Delaying = true;
        if (StartDelay != 0.0f)
            yield return StartDelay;
        Delaying = false;

        for (int currentFromNode = 0; currentFromNode < Nodes.Length - 1; currentFromNode++)
        {
            Vector2 from = Nodes[currentFromNode];
            Vector2 to = Nodes[currentFromNode + 1];
            bool motionTweenCompleted = false;
            Tween motionTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, MoveTimes[currentFromNode]);
            motionTween.OnUpdate = t => CameraPosition = Calc.LerpSnap(from, to, t.Eased);
            motionTween.OnComplete = _ => motionTweenCompleted = true;
            Add(motionTween);
            motionTween.Start();
            while (!motionTweenCompleted)
            {
                Vector2 p = to - from;
                if (DoPlayerDieCheck(level, player, SquashHorizontalArea, p.Y > 0))
                    yield break;
                yield return null;
            }
            Delaying = true;
            if (Delays[currentFromNode] != 0.0f)
                yield return Delays[currentFromNode];
            Delaying = false;
        }
        ResetReel();
        if (SetOffsetOnFinished)
            level.CameraOffset = OnFinishedOffset;
        yield break;
    }

    public bool DoPlayerDieCheck(Level level, Player player, bool squash, bool isYPositive)
    {
        if (level.Tracker.GetEntity<Player>() == null) return false;
        if (player is null) return false;
        bool died = false;
        float xx = level.Camera.Left;
        if (player.Left < xx)
        {
            player.Left = xx;
            player.OnBoundsH();

            if (!squash)
            {
                player.Die(Vector2.UnitX);
                died = true;
            }
        }
        xx = level.Camera.Right;
        if (player.Right > xx)
        {
            player.Right = xx;
            player.OnBoundsH();

            if (!squash)
            {
                player.Die(-Vector2.UnitX);
                died = true;
            }
        }
        float yy = level.Camera.Top;
        if (player.Bottom < yy)
        {
            player.Bottom = yy;
            player.OnBoundsV();
            if (isYPositive)
            {
                player.Die(-Vector2.UnitY);
                died = true;
            }

        }
        if (player.Top > level.Camera.Bottom)
        {
            player.Die(Vector2.UnitY);
            died = true;
        }
        if (player.CollideCheck<Solid>())
        {
            player.Die(Vector2.Zero);
        }
        return died;
    }
}