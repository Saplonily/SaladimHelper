﻿using Celeste.Mod.Entities;
using Mono.Cecil.Cil;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/BitsMomentumRefill"), SaladimModule]
public class BitsMomentumRefill : Entity
{
    // lets do happy copying
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    public static ParticleType P_SpeedField;

    public static Color MainColor = Calc.HexToColor("FFD500");

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
    private float mul;

    public static void Load()
    {
        IL.Celeste.Player.CallDashEvents += Player_CallDashEvents;
        On.Celeste.Player.Render += Player_Render;
        Everest.Events.Player.OnSpawn += Player_OnSpawn;
    }

    public static void Unload()
    {
        IL.Celeste.Player.CallDashEvents -= Player_CallDashEvents;
        On.Celeste.Player.Render -= Player_Render;
        Everest.Events.Player.OnSpawn -= Player_OnSpawn;
    }

    private static void Player_OnSpawn(Player obj)
    {
        ModuleSession.MomentumRefillSpeedKept = null;
    }

    private static void Player_CallDashEvents(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchStfld<SaveData>("TotalDashes")))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate(TryAddingMomentum);
        }
        if (cur.TryGotoNext(MoveType.Before, ins => ins.MatchCallvirt<Booster>("PlayerBoosted")))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.Emit(OpCodes.Ldfld, typeof(Player).GetField("CurrentBooster"));
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate((Booster booster, Player player) =>
            {
                if (booster.GetType() == typeof(Booster))
                    TryAddingMomentum(player);
            });
        }
        static void TryAddingMomentum(Player player)
        {
            var sn = ModuleSession.MomentumRefillSpeedKept;
            if (sn is not null)
            {
                var s = sn.Value;
                player.Speed += s.speed * s.mul;
                MakeSpeedField(player.SceneAs<Level>().Particles, player.Center, s.speed);
                ModuleSession.MomentumRefillSpeedKept = null;
                Celeste.Freeze(1 / 60f);
            }
        }
    }

    private static void Player_Render(On.Celeste.Player.orig_Render orig, Player self)
    {
        orig(self);
        if (ModuleSession.MomentumRefillSpeedKept is not null && self.Scene.OnInterval(0.1f))
        {
            Vector2 vector = new(Math.Abs(self.Sprite.Scale.X) * (float)self.Facing, self.Sprite.Scale.Y);
            TrailManager.Add(self, vector, MainColor, 1f);
        }
    }

    public BitsMomentumRefill(Vector2 position, bool oneUse, bool recordX, bool recordY, bool isBlooming, float mul)
        : base(position)
    {
        this.oneUse = oneUse;
        this.recordX = recordX;
        this.recordY = recordY;
        this.isBlooming = isBlooming;
        this.mul = mul;

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
            Audio.Play("event:/momentum_refill/momentum_refill_return", Position);
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
        if (ModuleSession.MomentumRefillSpeedKept is null)
        {
            var sp = player.Speed;
            if (!recordX) sp.X = 0;
            if (!recordY) sp.Y = 0;
            // do speed keeping
            ModuleSession.MomentumRefillSpeedKept = (sp, mul);
            // and do effects
            MakeSpeedField(SceneAs<Level>().Particles, Position, sp);
            Audio.Play("event:/momentum_refill/momentum_refill_touch", Position);
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