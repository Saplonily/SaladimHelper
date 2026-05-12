using Celeste.Mod.Entities;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper.Entities;

public readonly struct BitsMomentumRefillKeptSpeed
{
    public readonly Vector2 Speed;
    public readonly float Multiplier;

    public BitsMomentumRefillKeptSpeed(Vector2 speed, float multiplier)
    {
        Speed = speed;
        Multiplier = multiplier;
    }
}

[CustomEntity("SaladimHelper/BitsMomentumRefill")]
public class BitsMomentumRefill : Entity
{
    // lets do happy copying
    internal static ParticleType P_Shatter;
    internal static ParticleType P_Regen;
    internal static ParticleType P_Glow;
    internal static ParticleType P_SpeedField;

    internal static Color MainColor = Calc.HexToColor("FFD500");

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

    private bool recordX;
    private bool recordY;
    private bool isBlooming;
    private float multiplier;

    public static void Load()
    {
        IL.Celeste.Player.CallDashEvents += Player_CallDashEvents;
        Everest.Events.Player.OnAfterUpdate += Player_OnAfterUpdate;
    }

    public static void Unload()
    {
        IL.Celeste.Player.CallDashEvents -= Player_CallDashEvents;
        Everest.Events.Player.OnAfterUpdate -= Player_OnAfterUpdate;
    }

    private static void Player_CallDashEvents(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchStfld<SaveData>(nameof(SaveData.TotalDashes))))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate<Action<Player>>(new(TryAddingMomentum));
        }
        if (cur.TryGotoNext(MoveType.Before, ins => ins.MatchCallvirt<Booster>(nameof(Booster.PlayerBoosted))))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.Emit(OpCodes.Ldfld, typeof(Player).GetField(nameof(Player.CurrentBooster)));
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate(static (Booster booster, Player player) =>
            {
                // it was once a bug...
                if (booster.GetType() == typeof(Booster))
                    TryAddingMomentum(player);
            });
        }

        static void TryAddingMomentum(Player player)
        {
            var sn = GetSpeedKept(player);
            if (sn is not null)
            {
                var s = sn.Value;
                player.Speed += s.Speed * s.Multiplier;
                MakeSpeedField(player.SceneAs<Level>().Particles, player.Center, s.Speed);
                SetSpeedKept(player, null);
                Celeste.Freeze(1 / 60f);
            }
        }
    }

    private static void Player_OnAfterUpdate(Player player)
    {
        if (GetSpeedKept(player) is not null && player.Scene.OnInterval(0.1f))
        {
            Vector2 vector = new(Math.Abs(player.Sprite.Scale.X) * (float)player.Facing, player.Sprite.Scale.Y);
            TrailManager.Add(player, vector, MainColor, 1f);
        }
    }

    public BitsMomentumRefill(Vector2 position, bool oneUse, bool recordX, bool recordY, bool isBlooming, float multiplier)
        : base(position)
    {
        this.oneUse = oneUse;
        this.recordX = recordX;
        this.recordY = recordY;
        this.isBlooming = isBlooming;
        this.multiplier = multiplier;

        Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(new Action<Player>(OnPlayer)));
        Add(outline = new Image(GFX.Game["SaladimHelper/entities/momentumRefill/outline"]));
        outline.CenterOrigin();
        outline.Visible = false;

        Add(sprite = new Sprite(GFX.Game, "SaladimHelper/entities/momentumRefill/idle"));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle", false, false);
        sprite.CenterOrigin();

        Add(flash = new Sprite(GFX.Game, "SaladimHelper/entities/momentumRefill/flash"));
        flash.Add("flash", "", 0.05f);
        flash.OnFinish = _ => flash.Visible = false;
        flash.CenterOrigin();

        Add(wiggler = Wiggler.Create(1f, 4f, v => sprite.Scale = flash.Scale = Vector2.One * (1f + v * 0.2f)));

        Add(new MirrorReflection());
        if (isBlooming)
        {
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
        }
        Add(sine = new SineWave(0.6f));
        sine.Randomize();

        UpdateY();
        Depth = -100;
    }

    public BitsMomentumRefill(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Bool("oneUse", false),
              data.Bool("recordX", true), data.Bool("recordY", true),
              data.Bool("isBlooming", true), data.Float("mul", 1.0f))
    {
    }

    public static void MakeSpeedField(ParticleSystem ps, Vector2 center, Vector2 speed)
    {
        Random r = Calc.Random;
        float length = speed.Length();
        int amount = (int)MathHelper.Min(length * 0.1f, 60f);
        for (int i = 0; i < amount; i++)
        {
            P_SpeedField.SpeedMax = MathHelper.Min(length * 0.6f, 300f);
            P_SpeedField.SpeedMin = MathHelper.Min(length * 0.6f, 200f);
            var spn = speed / length;
            ps.Emit(P_SpeedField, center + new Vector2(r.NextFloat(16f) - 8f, r.NextFloat(20f) - 10f) - spn * length / 60f, speed.Angle());
        }
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
        else if (Scene.OnInterval(0.1f) && isBlooming)
        {
            level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }
        UpdateY();
        if (isBlooming)
        {
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
        }
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
            Audio.Play(SaladimSFX.MomentumRefillReturn, Position);
            level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
        }
    }

    private void UpdateY()
    {
        var v = sine.Value * 2f;
        sprite.Y = v;
        if (isBlooming)
            bloom.Y = sine.Value * 2f;
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
        if (GetSpeedKept(player) is null)
        {
            var playerSpeed = player.Speed;
            if (!recordX) playerSpeed.X = 0;
            if (!recordY) playerSpeed.Y = 0;
            // do speed keeping
            SetSpeedKept(player, new(playerSpeed, multiplier));
            // and do effects
            MakeSpeedField(SceneAs<Level>().Particles, Position, playerSpeed);
            Audio.Play(SaladimSFX.MomentumRefillTouch, Position);
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

    private static BitsMomentumRefillKeptSpeed? GetSpeedKept(Player player)
        => DynamicData.For(player).Get("sh_momentum_speed_kept") as BitsMomentumRefillKeptSpeed?;

    private static void SetSpeedKept(Player player, BitsMomentumRefillKeptSpeed? value)
        => DynamicData.For(player).Set("sh_momentum_speed_kept", value);
}