using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/FlagRefill")]
public class FlagRefill : Entity
{
    private Sprite sprite;
    private Sprite flash;
    private Image outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private DashListener dashListener;
    private Level level;
    private SineWave sine;
    private ParticleType p_shatter;
    private ParticleType p_regen;
    private ParticleType p_glow;

    private bool dashRefill;
    private bool oneUse;
    private bool useOnlyNoFlag;
    private float respawnTimer;
    private string flag;
    private string sfxUsed;
    private string sfxRespawned;
    private float removeFlagDelay;

    public FlagRefill(
        Vector2 position,
        bool dashRefill, bool oneUse,
        string outlineTexture,
        string idleTexture,
        string flashTexture,
        string flag,
        bool useOnlyNoFlag,
        string sfxUsed,
        string sfxRespawned,
        float removeFlagDelay
        )
        : base(position)
    {
        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
        this.oneUse = oneUse;
        this.flag = flag;
        this.dashRefill = dashRefill;
        this.useOnlyNoFlag = useOnlyNoFlag;
        this.sfxUsed = sfxUsed;
        this.sfxRespawned = sfxRespawned;
        this.removeFlagDelay = removeFlagDelay;

        p_shatter = Refill.P_Shatter;
        p_regen = Refill.P_Regen;
        p_glow = Refill.P_Glow;

        Add(outline = new Image(GFX.Game[outlineTexture]));
        outline.CenterOrigin();
        outline.Visible = false;

        Add(sprite = new Sprite(GFX.Game, idleTexture));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle", false, false);
        sprite.CenterOrigin();

        Add(flash = new Sprite(GFX.Game, flashTexture));
        flash.Add("flash", "", 0.05f);
        flash.OnFinish = _ => flash.Visible = false;
        flash.CenterOrigin();

        Add(wiggler = Wiggler.Create(1f, 4f, v => sprite.Scale = flash.Scale = Vector2.One * (1f + v * 0.2f), false, false));
        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.8f, 16f));
        Add(light = new VertexLight(Color.White, 1f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));
        dashListener = new DashListener(_ => Add(new Coroutine(DisableFlagRoutine())));
        sine.Randomize();
        UpdateY();
        Depth = -100;
    }

    public IEnumerator DisableFlagRoutine()
    {
        yield return removeFlagDelay;
        SceneAs<Level>().Session.SetFlag(flag, false);
        Remove(dashListener);
    }

    public FlagRefill(EntityData data, Vector2 offset)
        : this(
              data.Position + offset,
              data.Bool("dashRefill", true),
              data.Bool("oneUse", false),
              data.Attr("outlineTexture", "objects/refill/outline"),
              data.Attr("idleTexture", "objects/refill/idle"),
              data.Attr("flashTexture", "objects/refill/flash"),
              data.Attr("flag", ""),
              data.Bool("useOnlyNoFlag", false),
              data.Attr("sfxUsed", "event:/game/general/diamond_touch"),
              data.Attr("sfxRespawned", "event:/game/general/diamond_return"),
              data.Float("removeFlagDelay", 0.1f)
              )
    {
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
                Respawn();
        }
        else if (Scene.OnInterval(0.1f))
        {
            level.ParticlesFG.Emit(p_glow, 1, Position, Vector2.One * 5f);
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
        if (Scene.OnInterval(2f) && sprite.Visible)
        {
            flash.Play("flash", true, false);
            flash.Visible = true;
        }
    }

    private void Respawn()
    {
        if (Collidable)
            return;
        Collidable = true;
        sprite.Visible = true;
        outline.Visible = false;
        Depth = -100;
        wiggler.Start();
        Audio.Play(sfxRespawned, Position);
        level.ParticlesFG.Emit(p_regen, 16, Position, Vector2.One * 2f);
    }

    private void UpdateY()
        => flash.Y = sprite.Y = bloom.Y = sine.Value * 2f;


    public override void Render()
    {
        if (sprite.Visible)
            sprite.DrawOutline(1);
        base.Render();
    }

    private void OnPlayer(Player player)
    {
        if (!level.Session.GetFlag(flag) || !useOnlyNoFlag)
        {
            if (dashRefill)
                player.UseRefill(false);
            Audio.Play(sfxUsed, Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            Add(new Coroutine(RefillRoutine(player), true));
            respawnTimer = 2.5f;
            level.Session.SetFlag(flag);
            Add(dashListener);
        }
    }

    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        level.Shake(0.3f);
        sprite.Visible = (flash.Visible = false);
        if (!oneUse)
            outline.Visible = true;
        Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(p_shatter, 5, Position, Vector2.One * 4f, angle - MathHelper.PiOver2);
        level.ParticlesFG.Emit(p_shatter, 5, Position, Vector2.One * 4f, angle + MathHelper.PiOver2);
        SlashFx.Burst(Position, angle);
        if (oneUse)
            RemoveSelf();
        yield break;
    }
}