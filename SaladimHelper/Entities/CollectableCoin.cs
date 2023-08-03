using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity($"{ModuleName}/CollectableCoin")]
public class CollectableCoin : Entity
{
    private bool collected = false;
    private Sprite spr;
    private string group;
    private int id;

    private BloomPoint bloom;

    public CollectableCoin(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Attr("group", "default"), data.ID)
    {

    }

    public CollectableCoin(Vector2 pos, string group, int id)
    {
        this.group = group;
        this.id = id;
        Position = pos;
        // add some awesome bloom!
        Add(bloom = new BloomPoint(0f, 16f));
        bloom.Alpha = 0.5f;

        Collider = new Hitbox(16, 16, -8, -8);
        Add(spr = new Sprite(GFX.Game, "SaladimHelper/Entities/collectable_coin/idle"));
        Add(new PlayerCollider(OnPlayer));

        spr.Add("idle", "", 0.1f, new Chooser<string>("idle", 1f), 0, 0, 1, 2, 3, 4, 5, 6, 6, 7, 8, 9, 10, 11);
        spr.Play("idle");
        spr.CenterOrigin();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        //collected = ModuleSession.CollectedCoins.Contains(new EntityID(SceneAs<Level>().Session.Level, id));
        if (collected)
            RemoveSelf();
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
        Audio.Play("event:/game/general/touchswitch_any", Center);
        var dic = ModuleSession.GroupToCoinsMap;
        if (!dic.ContainsKey(group))
            dic[group] = 0;
        dic[group]++;
        ModuleSession.CollectedCoins.Add(new EntityID(SceneAs<Level>().Session.Level, id));
        PlayAnim();
        DisplayDisplayerIf();
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
            if(t.Eased>=p)
            {
                float thisEased = (t.Eased - p) / (1f - p);
                spr.Scale.X = Calc.LerpClamp(1.0f, 0.0f, thisEased);
            }
        };
        tw.OnComplete = t =>
        {
            RemoveSelf();
        };
        Add(tw);
        tw.Start();
    }

    public void DisplayDisplayerIf()
    {
        var cur = ModuleSession.CurrentCoinDisplayer;
        if (cur is null || cur?.Scene != Scene)
        {
            cur = new();
            ModuleSession.CurrentCoinDisplayer = cur;
            Scene.Add(cur);
        }
        cur.Group = group;
    }

    public override void Render()
    {
        base.Render();
    }
}