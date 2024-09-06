using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/CollectableCoin")]
public class CollectableCoin : Entity
{
    private readonly bool persist;

    private bool collected = false;
    private EntityID entityID;
    private Sprite spr;
    private Vector2[] nodes;

    private BloomPoint bloom;

    public CollectableCoin(EntityData data, Vector2 offset, EntityID entityID)
        : this(data.Position + offset, data.NodesOffset(offset), entityID, data.Bool("persist", false))
    {
    }

    public CollectableCoin(Vector2 pos, Vector2[] nodes, EntityID entityID)
        : this(pos, nodes, entityID, false)
    {

    }

    public CollectableCoin(Vector2 pos, Vector2[] nodes, EntityID entityID, bool persist)
    {
        this.entityID = entityID;
        this.persist = persist;
        this.nodes = nodes;
        Position = pos;
        // add some awesome bloom!
        Add(bloom = new BloomPoint(0f, 16f));
        bloom.Alpha = 0.5f;

        Collider = new Hitbox(16, 16, -8, -8);
        Add(spr = new Sprite(GFX.Game, "SaladimHelper/entities/collectableCoin/idle"));
        Add(new PlayerCollider(OnPlayer));

        spr.Add("idle", "", 0.1f, new Chooser<string>("idle", 1f), 0, 0, 1, 2, 3, 4, 5, 6, 6, 7, 8, 9, 10, 11);
        spr.Play("idle");
        spr.CenterOrigin();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
    }

    public void OnPlayer(Player p)
    {
        if (!collected)
        {
            Collect();
            collected = true;
        }
    }

    public override void Update()
    {
        base.Update();
        float l = ((float)Math.Sin(Scene.TimeActive * 4.0f) + 1.0f) / 6.0f;
        spr.Color = Color.Lerp(Color.White, Color.Black, l);
    }

    public void Collect()
    {
        if (collected) return;
        Audio.Play("event:/gddcoin/key_get", Center);

        var session = SceneAs<Level>().Session;
        ModuleSession.CollectedCoinsAmount++;
        if (!persist)
            session.DoNotLoad.Add(entityID);

        PlayAnim();
        CoinDisplayer.Display(Scene);

        if (nodes != null && nodes.Length >= 2)
        {
            Player player = Scene.Tracker.GetEntity<Player>();
            player?.Add(new Coroutine(NodeRoutine(player, nodes[1], nodes[0])));
        }

        static IEnumerator NodeRoutine(Player player, Vector2 node1, Vector2 node0)
        {
            yield return 0.3f;
            if (!player.Dead)
            {
                Audio.Play("event:/game/general/cassette_bubblereturn", player.SceneAs<Level>().Camera.Position + new Vector2(160f, 90f));
                player.StartCassetteFly(node1, node0);
            }
            yield break;
        }
    }

    private void PlayAnim()
    {
        for (int i = 0; i < 8; i++)
        {
            float num = Calc.Random.NextFloat(MathHelper.Pi * 2.0f);
            SceneAs<Level>().Particles.Emit(TouchSwitch.P_FireWhite, Position + Calc.AngleToVector(num, Calc.Random.NextFloat(6f)), num);
        }

        spr.Rate = 4.0f;
        Tween tw = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, 1.25f, false);
        float curY = Y;
        float targetY = Y - 12.0f;
        tw.OnUpdate = t =>
        {
            Y = Calc.LerpClamp(Y, targetY, t.Eased);

            float p = 0.2f;
            if (t.Eased >= p)
            {
                float thisEased = (t.Eased - p) / (1f - p);
                bloom.Alpha = Calc.LerpClamp(1.0f, 0.2f, thisEased);
            }

            p = 0.95f;
            if (t.Eased >= p)
            {
                float thisEased = (t.Eased - p) / (1f - p);
                spr.Scale.X = Calc.LerpClamp(1.0f, 0.0f, thisEased);
            }
        };
        tw.OnComplete = t => RemoveSelf();
        Add(tw);
        tw.Start();
    }
}