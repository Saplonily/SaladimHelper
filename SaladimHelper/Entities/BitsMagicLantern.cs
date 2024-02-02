using Celeste.Mod.Entities;

// modified and refactored from ChineseNewYear2024Helper by optimize-2
// https://github.com/optimize-2/ChineseNewYear2024Helper

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/BitsMagicLantern"), Tracked]
public class BitsMagicLantern : Actor
{
    public static ParticleType P_Impact;

    public Vector2 Speed;
    public Holdable Holdable;
    public float Radius;
    public bool IsHoldable;

    private Sprite sprite;
    private Collision onCollideH;
    private Collision onCollideV;
    private float noGravityTimer;
    private Vector2 prevLiftSpeed;
    private Vector2 previousPosition;
    private HoldableCollider hitSeeker;
    private float swatTimer;
    private float hardVerticalHitSoundCooldown;
    private VertexLight vertexLight;

    public BitsMagicLantern(EntityData e, Vector2 offset) : base(e.Position + offset)
    {
        Radius = e.Float("radius", 20f);
        IsHoldable = e.Bool("holdable", false);

        previousPosition = e.Position + offset;
        Collider = new Hitbox(8f, 10f, -4f, -10f);

        Add(sprite = GFX.SpriteBank.Create("sal_bitsMagicLanternLight"));
        sprite.Y += -4;
        sprite.Scale.X = -1f;
        if (!IsHoldable)
        {
            sprite.Play("static", false, false);
        }
        else
        {
            sprite.Play("holdable");
            Add(Holdable = new Holdable());
            Holdable.PickupCollider = new Hitbox(16f, 22f, -8f, -16f);
            Holdable.SlowFall = false;
            Holdable.SlowRun = true;
            Holdable.OnPickup = OnPickup;
            Holdable.OnRelease = OnRelease;
            Holdable.DangerousCheck = Dangerous;
            Holdable.OnHitSeeker = HitSeeker;
            Holdable.OnSwat = Swat;
            Holdable.OnHitSpring = HitSpring;
            Holdable.OnHitSpinner = HitSpinner;
            Holdable.SpeedGetter = () => Speed;
            onCollideH = OnCollideH;
            onCollideV = OnCollideV;
            LiftSpeedGraceTime = 0.1f;
        }

        Depth = Depths.TheoCrystal;
        Add(vertexLight = new VertexLight(Collider.Center, Color.White, 0.92f, 32, 64));
        Add(new BloomPoint(0.2f, 24f));
        Add(new MirrorReflection());
    }

    public override void Update()
    {
        base.Update();
        if (!IsHoldable) return;

        if (swatTimer > 0f)
            swatTimer -= Engine.DeltaTime;

        hardVerticalHitSoundCooldown -= Engine.DeltaTime;
        if (Holdable.IsHeld)
        {
            prevLiftSpeed = Vector2.Zero;
            vertexLight.Visible = true;
            var solids = Scene.Tracker.GetEntities<Solid>().Cast<Solid>();
            bool any = false;
            foreach (var solid in solids)
            {
                if (solid.CollideRect(new Rectangle(-5, -5, 10, 10)))
                {
                    any = true;
                    break;
                }
            }
            vertexLight.Visible = !any;
        }
        else
        {
            if (OnGround())
            {
                float target;
                if (!OnGround(Position + Vector2.UnitX * 3f))
                    target = 20f;
                else if (!OnGround(Position - Vector2.UnitX * 3f))
                    target = -20f;
                else
                    target = 0f;
                Speed.X = Calc.Approach(Speed.X, target, 800f * Engine.DeltaTime);

                Vector2 liftSpeed = LiftSpeed;
                if (liftSpeed == Vector2.Zero && prevLiftSpeed != Vector2.Zero)
                {
                    Speed = prevLiftSpeed;
                    prevLiftSpeed = Vector2.Zero;
                    Speed.Y = Math.Min(Speed.Y * 0.6f, 0f);
                    if (Speed.X != 0f && Speed.Y == 0f)
                        Speed.Y = -60f;
                    if (Speed.Y < 0f)
                        noGravityTimer = 0.15f;
                }
                else
                {
                    prevLiftSpeed = liftSpeed;
                    if (liftSpeed.Y < 0f && Speed.Y < 0f)
                        Speed.Y = 0f;
                }
            }
            else if (Holdable.ShouldHaveGravity)
            {
                float step = 350f;
                if (Speed.Y < 0f)
                {
                    step *= 0.5f;
                }
                Speed.X = Calc.Approach(Speed.X, 0f, step * Engine.DeltaTime);
                if (noGravityTimer > 0f)
                {
                    noGravityTimer -= Engine.DeltaTime;
                }
                else
                {
                    step = 800f;
                    if (Math.Abs(Speed.Y) <= 30f)
                        step *= 0.5f;
                    Speed.Y = Calc.Approach(Speed.Y, 200f, step * Engine.DeltaTime);
                }
            }
            previousPosition = ExactPosition;
            MoveH(Speed.X * Engine.DeltaTime, onCollideH, null);
            MoveV(Speed.Y * Engine.DeltaTime, onCollideV, null);

            Level level = SceneAs<Level>();
            if (Center.X > level.Bounds.Right)
            {
                MoveH(32f * Engine.DeltaTime, null, null);
                if (Left - 8f > level.Bounds.Right)
                    RemoveSelf();
            }
            else if (Left < level.Bounds.Left)
            {
                Left = level.Bounds.Left;
                Speed.X *= -0.4f;
            }
            else if (Top < level.Bounds.Top - 4)
            {
                Top = level.Bounds.Top + 4;
                Speed.Y = 0f;
            }
            else if (Bottom > level.Bounds.Bottom && SaveData.Instance.Assists.Invincible)
            {
                Bottom = level.Bounds.Bottom;
                Speed.Y = -300f;
                Audio.Play("event:/game/general/assist_screenbottom", Position);
            }
            else if (Top > level.Bounds.Bottom)
            {
                RemoveSelf();
            }
            if (X < level.Bounds.Left + 10)
            {
                MoveH(32f * Engine.DeltaTime, null, null);
            }
        }
        Holdable.CheckAgainstColliders();
        if (hitSeeker != null && swatTimer <= 0f && !hitSeeker.Check(Holdable))
            hitSeeker = null;
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        Draw.Circle(Center, Radius, Color.Red, 24);
    }

    public void Swat(HoldableCollider hc, int dir)
    {
        if (Holdable.IsHeld && hitSeeker == null)
        {
            swatTimer = 0.1f;
            hitSeeker = hc;
            Holdable.Holder.Swat(dir);
        }
    }

    public bool Dangerous(HoldableCollider holdableCollider)
    {
        return !Holdable.IsHeld && Speed != Vector2.Zero && hitSeeker != holdableCollider;
    }

    public void HitSeeker(Seeker seeker)
    {
        if (!Holdable.IsHeld)
            Speed = (Center - seeker.Center).SafeNormalize(120f);
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", Position);
    }

    public void HitSpinner(Entity spinner)
    {
        if (
            !Holdable.IsHeld && Speed.Length() < 0.01f &&
            LiftSpeed.Length() < 0.01f &&
            (previousPosition - ExactPosition).Length() < 0.01f &&
            OnGround(1)
            )
        {
            int dir = Math.Sign(X - spinner.X);
            dir = dir == 0 ? 1 : dir;
            Speed.X = dir * 120f;
            Speed.Y = -30f;
        }
    }

    public bool HitSpring(Spring spring)
    {
        if (!Holdable.IsHeld)
        {
            if (spring.Orientation == Spring.Orientations.Floor && Speed.Y >= 0f)
            {
                Speed.X *= 0.5f;
                Speed.Y = -160f;
                noGravityTimer = 0.15f;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallLeft && Speed.X <= 0f)
            {
                MoveTowardsY(spring.CenterY + 5f, 4f, null);
                Speed.X = 220f;
                Speed.Y = -80f;
                noGravityTimer = 0.1f;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallRight && Speed.X >= 0f)
            {
                MoveTowardsY(spring.CenterY + 5f, 4f, null);
                Speed.X = -220f;
                Speed.Y = -80f;
                noGravityTimer = 0.1f;
                return true;
            }
        }
        return false;
    }

    private void OnCollideH(CollisionData data)
    {
        if (data.Hit is DashSwitch dashSwitch)
        {
            dashSwitch.OnDashCollide(null, Vector2.UnitX * Math.Sign(Speed.X));
        }
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", Position);
        if (Math.Abs(Speed.X) > 100f)
            ImpactParticles(data.Direction);
        Speed.X *= -0.4f;
    }

    private void OnCollideV(CollisionData data)
    {
        if (data.Hit is DashSwitch dashSwitch)
        {
            dashSwitch.OnDashCollide(null, Vector2.UnitY * (float)Math.Sign(Speed.Y));
        }
        if (Speed.Y > 0f)
        {
            if (hardVerticalHitSoundCooldown <= 0f)
            {
                Audio.Play(
                    "event:/game/05_mirror_temple/crystaltheo_hit_ground",
                    Position,
                    "crystal_velocity",
                    Calc.ClampedMap(Speed.Y, 0f, 200f, 0f, 1f)
                    );
                hardVerticalHitSoundCooldown = 0.5f;
            }
            else
            {
                Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", Position, "crystal_velocity", 0f);
            }
        }
        if (Speed.Y > 160f)
            ImpactParticles(data.Direction);
        if (Speed.Y > 140f && data.Hit is not SwapBlock && data.Hit is not DashSwitch)
        {
            Speed.Y *= -0.6f;
            return;
        }
        Speed.Y = 0f;
    }

    private void ImpactParticles(Vector2 dir)
    {
        float direction;
        Vector2 position;
        Vector2 positionRange;
        if (dir.X > 0f)
        {
            direction = MathHelper.Pi;
            position = new Vector2(Right, Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        }
        else if (dir.X < 0f)
        {
            direction = 0f;
            position = new Vector2(Left, Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        }
        else if (dir.Y > 0f)
        {
            direction = -MathHelper.PiOver2;
            position = new Vector2(X, Bottom);
            positionRange = Vector2.UnitX * 6f;
        }
        else
        {
            direction = MathHelper.PiOver2;
            position = new Vector2(X, Top);
            positionRange = Vector2.UnitX * 6f;
        }
        SceneAs<Level>().Particles.Emit(P_Impact, 12, position, positionRange, direction);
    }

    public override bool IsRiding(Solid solid)
    {
        return Speed.Y == 0f && base.IsRiding(solid);
    }

    private void OnPickup()
    {
        Speed = Vector2.Zero;
        AddTag(Tags.Persistent);
    }

    private void OnRelease(Vector2 force)
    {
        RemoveTag(Tags.Persistent);
        if (force.X != 0f && force.Y == 0f)
            force.Y = -0.4f;
        Speed = force * 200f;
        if (Speed != Vector2.Zero)
            noGravityTimer = 0.1f;
    }
}
