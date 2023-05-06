using Celeste.Mod.Entities;
using FMOD;
using static Celeste.GaussianBlur;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity($"{ModuleName}/DirtBounceBlock")]
public class DirtBounceBlock : Solid
{
    public enum State
    {
        Invalid,
        Idle,
        Breaking,
        Broken,
        Reforming,
        Appearing
    }

    public static ParticleType P_Motion;
    public static readonly float MaxFallingSpeed = 260f;

    public State curState = State.Idle;
    public float FallingSpeed;
    public Coroutine coroutine = null;
    public Vector2 StartPosition;
    public Image CenterImage;

    // default, 0f(full white), completed, 1f(opacity)
    public float respawnFlashPercent = 1f;

    public float AreaRadio => Hitbox.Size.X * Hitbox.Size.Y / 1400f;

    protected List<Image> images;

    static DirtBounceBlock()
    {
        P_Motion = new(BounceBlock.P_FireBreak)
        {
            Acceleration = Vector2.UnitY * 60f,
            SpeedMax = 40f,
            SpeedMin = 10f,
            SpeedMultiplier = 1f,
            Color = Calc.HexToColor("58421C"),
            Color2 = Calc.HexToColor("866833"),
            Direction = (float)(-Math.PI / 2)
        };
    }

    public DirtBounceBlock(Vector2 position, float width, float height)
        : base(position, width, height, false)
    {
        OnDashCollide = OnDashCollided;
        StartPosition = position;
        images = BuildSprite(GFX.Game[$"{ModuleName}/Entities/more_bounce_block/rock_tiles"]);
        var img = CenterImage = GFX.SpriteBank.Create("sal_bumpBlockCenterDirt");
        img.Position = Center - TopLeft;
        img.CenterOrigin();
        Add(img);
    }

    public DirtBounceBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height)
    { }

    public DashCollisionResults OnDashCollided(Player player, Vector2 dir)
    {
        if (curState is State.Idle)
        {
            coroutine = new Coroutine(MakeFallingCoroutine());
            Add(coroutine);

            SceneAs<Level>().ParticlesFG.Emit(P_Motion, (int)(75 * AreaRadio), Center, new Vector2(Width, Height) / 1.9f, dir.Angle());
            Celeste.Freeze(0.05f);
            Audio.Play("event:/game/general/fallblock_shake", Center);
            return DashCollisionResults.Rebound;
        }
        else
        {
            return DashCollisionResults.NormalCollision;
        }
    }

    public override void OnShake(Vector2 amount)
    {
        base.OnShake(amount);
        SceneAs<Level>().Particles.Emit(P_Motion, (int)(20 * AreaRadio), Center, new Vector2(Width, Height) / 2f);
        foreach (var img in images)
        {
            img.Position += amount;
        }
        CenterImage.Position += amount;
    }

    public IEnumerator MakeFallingCoroutine()
    {
        curState = State.Breaking;
        StartShaking(1f);
        yield return 1f;
        FallingSpeed = 0f;
        while (true)
        {
            FallingSpeed = Calc.Approach(FallingSpeed, MaxFallingSpeed, 500f * Engine.DeltaTime);

            bool collided = MoveVCollideSolids(FallingSpeed * Engine.DeltaTime, true, null);
            SceneAs<Level>().Particles.Emit(P_Motion, 1, Center, new Vector2(Width, Height) / 1.5f);
            if (collided)
            {
                curState = State.Broken;
                SceneAs<Level>().Particles.Emit(P_Motion, (int)(100 * AreaRadio), BottomCenter, new Vector2(Width, 2) / 2f);
                MakeFallingDebris($"{ModuleName}/Entities/more_bounce_block/rock_rubble", d =>
                {
                    Vector2 direction = Calc.AngleToVector((-Vector2.UnitY).Angle() + Calc.Random.Range(-0.1f, 0.1f), 1f);
                    float sp = FallingSpeed;
                    d.Speed = direction * Calc.Random.Range(sp * 0.2f, sp * 0.4f);
                });
                Audio.Play("event:/game/general/wall_break_stone", BottomCenter);
                DisableStaticMovers();
                Visible = Collidable = false;
                Vector2 prePos = Position;
                Alarm.Set(this, 1.5f, () => Add(new Coroutine(MakeRespawnCoroutine(prePos))));
                break;
            }
            yield return null;
        }
        yield break;
    }

    public IEnumerator MakeRespawnCoroutine(Vector2 prePos)
    {
        while (CollideCheck<Player>(StartPosition) || CollideCheck<Solid>(StartPosition))
            yield return null;
        Position = StartPosition;
        Vector2 posOffset = Position - prePos;
        MoveStaticMovers(posOffset);
        Collidable = true;
        curState = State.Reforming;
        MakeRespawnDebris($"{ModuleName}/Entities/more_bounce_block/rock_rubble", 0.4f);
        yield return 0.2f;
        Audio.Play("event:/game/09_core/bounceblock_reappear", Center);
        yield return 0.2f;
        curState = State.Appearing;
        Visible = true;
        EnableStaticMovers();
        respawnFlashPercent = 0f;
        while (true)
        {
            respawnFlashPercent += Engine.DeltaTime / 0.1f;
            if (respawnFlashPercent is < 1f)
                yield return null;
            else
                break;
        }
        MakeReformParticles();
        curState = State.Idle;
        respawnFlashPercent = 1f;
    }

    // let's do happy copying
    protected List<Image> BuildSprite(MTexture source)
    {
        List<Image> list = new();
        int widthInTile = source.Width / 8;
        int heightInTile = source.Height / 8;
        int w = 0;
        while (w < Width)
        {
            int h = 0;
            while (h < Height)
            {
                int resultW;
                if (w == 0)
                    resultW = 0;
                else if (w >= Width - 8f)
                    resultW = widthInTile - 1;
                else
                    resultW = Calc.Random.Next(1, widthInTile - 2);
                int resultH;
                if (h == 0)
                    resultH = 0;
                else if (h >= Height - 8f)
                    resultH = heightInTile - 1;
                else
                    resultH = Calc.Random.Next(1, heightInTile - 2);

                Image image = new(source.GetSubtexture(resultW * 8, resultH * 8, 8, 8))
                {
                    Position = new Vector2(w, h)
                };
                list.Add(image);
                Add(image);
                h += 8;
            }
            w += 8;
        }
        return list;
    }

    public void MakeFallingDebris(string rubbleAtlasName, Action<FallingDebris> xtraInitAction = null)
    {
        int w = 0;
        while (w < Width)
        {
            int h = 0;
            while (h < Height)
            {
                Scene.Add(Engine.Pooler.Create<FallingDebris>().Init(new Vector2(X + w + 4f, Y + h + 4f), rubbleAtlasName, xtraInitAction));
                h += 8;
            }
            w += 8;
        }
    }

    public void MakeRespawnDebris(string rubbleAtlasName, float duration, Action<RespawnDebris> xtraInitAction = null)
    {
        Vector2 center = Center;
        float distance = 16f;
        int w = 0;
        while (w < Width)
        {
            int h = 0;
            while (h < Height)
            {
                Vector2 to = new(X + w + 4f, Y + h + 4f);
                Scene.Add(Engine.Pooler.Create<RespawnDebris>()
                    .Init(to + (to - center).SafeNormalize(distance), to, duration, rubbleAtlasName, xtraInitAction));
                h += 8;
            }
            w += 8;
        }
    }

    public override void Update()
    {
        base.Update();
        if (curState is State.Broken)
        {
            Visible = Collidable = false;
        }
        if (curState is State.Reforming)
        {
            Visible = false;
        }
    }

    public override void Render()
    {
        base.Render();
        if (respawnFlashPercent != 1f)
        {
            Draw.Rect(X, Y, Width, Height, Color.White * (Ease.CubeOut(1f - respawnFlashPercent)));
        }
    }

    public void MakeReformParticles()
    {
        Level level = SceneAs<Level>();
        int w = 0;
        while (w < Width)
        {
            level.Particles.Emit(P_Motion, new Vector2(X + 2f + w + Calc.Random.Range(-1, 1), Y), -(float)Math.PI / 2f);
            level.Particles.Emit(P_Motion, new Vector2(X + 2f + w + Calc.Random.Range(-1, 1), Bottom - 1f), (float)Math.PI / 2f);
            w += 4;
        }
        int h = 0;
        while (h < Height)
        {
            level.Particles.Emit(P_Motion, new Vector2(X, Y + 2f + h + Calc.Random.Range(-1, 1)), (float)Math.PI);
            level.Particles.Emit(P_Motion, new Vector2(Right - 1f, Y + 2f + h + Calc.Random.Range(-1, 1)), 0f);
            h += 4;
        }
    }

    // we love happy copying
    [Pooled]
    public class FallingDebris : Entity
    {
        public Image Sprite;
        public Vector2 Speed;
        public float Percent;
        public float Duration;

        public FallingDebris Init(
            Vector2 position,
            string rubbleAtlasName,
            Action<FallingDebris> xtraInitAction = null
            )
        {
            List<MTexture> atlasSubtextures =
                GFX.Game.GetAtlasSubtextures(rubbleAtlasName);
            MTexture mtexture = Calc.Random.Choose(atlasSubtextures);
            if (Sprite == null)
            {
                Add(Sprite = new Image(mtexture));
                Sprite.CenterOrigin();
            }
            else
            {
                Sprite.Texture = mtexture;
            }
            Position = position;
            Percent = 0f;
            Duration = Calc.Random.Range(2, 3);
            xtraInitAction?.Invoke(this);
            return this;
        }

        public override void Update()
        {
            base.Update();
            Position += Speed * Engine.DeltaTime;
            Speed.X = Calc.Approach(Speed.X, 0f, 180f * Engine.DeltaTime);
            Speed.Y += 200f * Engine.DeltaTime;
            Percent += Engine.DeltaTime / Duration;
            Sprite.Color = Color.White * (1f - Percent);

            if (Percent >= 1f)
                RemoveSelf();
        }

        public override void Render()
        {
            Sprite.DrawOutline(Color.Black, 1);
            base.Render();
        }
    }

    [Pooled]
    public class RespawnDebris : Entity
    {
        public Image Sprite;
        public Vector2 From;
        public Vector2 To;
        public float Percent;
        public float Duration;

        public RespawnDebris Init(Vector2 from, Vector2 to, float duration, string rubbleAtlasName, Action<RespawnDebris> xtraInitAction)
        {
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(rubbleAtlasName);
            MTexture mtexture = Calc.Random.Choose(atlasSubtextures);
            if (Sprite == null)
            {
                Add(Sprite = new Image(mtexture));
                Sprite.CenterOrigin();
            }
            else
            {
                Sprite.Texture = mtexture;
            }
            Position = from;
            Percent = 0f;
            From = from;
            To = to;
            Duration = duration;
            xtraInitAction?.Invoke(this);
            return this;
        }

        public override void Update()
        {
            if (Percent > 1f)
            {
                RemoveSelf();
                return;
            }
            Percent += Engine.DeltaTime / Duration;
            Position = Vector2.Lerp(From, To, Ease.CubeIn(Percent));
            Sprite.Color = Color.White * Percent;
        }

        public override void Render()
        {
            Sprite.DrawOutline(Color.Black, 1);
            base.Render();
        }
    }
}
