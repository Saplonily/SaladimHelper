using Celeste.Mod.Entities;
using MonoMod.Utils;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity($"{ModuleName}/BitsMomentumRefill"), NeedModuleInit]
public class BitsMomentumRefill : Entity
{
    // lets do happy copying
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    public static ParticleType P_ShatterTwo;
    public static ParticleType P_RegenTwo;
    public static ParticleType P_GlowTwo;

    private Sprite sprite;
    private Sprite flash;
    private Image outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Level level;
    private SineWave sine;
    private bool oneUse;

    private float respawnTimer;

#if DEBUG // hot reloading makes our static fields to null, so, load them again here
    static BitsMomentumRefill()
    {
        GlobalHooks.LoadParticles(() => { });
    }
#endif

    public static void Load()
    {
        On.Celeste.Player.CallDashEvents += Player_CallDashEvents;
    }

    public static void Unload()
    {
        On.Celeste.Player.CallDashEvents -= Player_CallDashEvents;
    }

    private static void Player_CallDashEvents(On.Celeste.Player.orig_CallDashEvents orig, Player self)
    {
        DynData<Player> d = new(self);
        if (!(bool)d["calledDashEvents"])
        {
            var s = ModuleSession.MomentumRefillSpeedKept;
            if (s is not null)
            {
                self.Speed += s.Value;
                ModuleSession.MomentumRefillSpeedKept = null;
            }
        }
        orig(self);
    }

    public BitsMomentumRefill(Vector2 position, bool oneUse)
        : base(position)
    {
        this.oneUse = oneUse;

        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(new Action<Player>(OnPlayer)));
        Add(outline = new Image(GFX.Game["SaladimHelper/Entities/momentum_refill/outline"]));
        outline.CenterOrigin();
        outline.Visible = false;

        Add(sprite = new Sprite(GFX.Game, "SaladimHelper/Entities/momentum_refill/idle"));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle", false, false);
        sprite.CenterOrigin();

        Add(flash = new Sprite(GFX.Game, "SaladimHelper/Entities/momentum_refill/flash"));
        flash.Add("flash", "", 0.05f);
        flash.OnFinish = _ => flash.Visible = false;
        flash.CenterOrigin();

        Add(wiggler = Wiggler.Create(1f, 4f, v => sprite.Scale = flash.Scale = Vector2.One * (1f + v * 0.2f)));

        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.8f, 16f));
        Add(light = new VertexLight(Color.White, 1f, 16, 48));
        Add(sine = new SineWave(0.6f));
        sine.Randomize();

        UpdateY();
        Depth = -100;
    }

    public BitsMomentumRefill(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Bool("oneUse", false))
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
            {
                Respawn();
            }
        }
        else if (Scene.OnInterval(0.1f))
        {
            level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
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
        if (!Collidable)
        {
            Collidable = true;
            sprite.Visible = true;
            outline.Visible = false;
            Depth = -100;
            wiggler.Start();
            Audio.Play("event:/game/general/diamond_return", Position);
            level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
        }
    }

    private void UpdateY()
    {
        flash.Y = sprite.Y = bloom.Y = sine.Value * 2f;
    }

    public override void Render()
    {
        if (sprite.Visible)
            sprite.DrawOutline(1);
        base.Render();
    }

    private void OnPlayer(Player player)
    {
        // no speed kept
        if (ModuleSession.MomentumRefillSpeedKept is null)
        {
            // do speed keeping
            ModuleSession.MomentumRefillSpeedKept = player.Speed;

            // and do effects
            Audio.Play("event:/game/general/diamond_touch", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            Add(new Coroutine(RefillRoutine(player)));
            respawnTimer = 2.5f;
        }
        // else do nothing
    }

    private IEnumerator RefillRoutine(Player player)
    {
        // freeeeeeze
        Celeste.Freeze(0.05f);
        yield return null;

        level.Shake(0.3f);
        sprite.Visible = flash.Visible = false;
        if (!oneUse)
            outline.Visible = true;
        Depth = 8999;
        yield return 0.05f;
        float dir = player.Speed.Angle();
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, dir - MathHelper.PiOver2);
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, dir + MathHelper.PiOver2);
        SlashFx.Burst(Position, dir);
        if (oneUse)
            RemoveSelf();
        yield break;
    }
}