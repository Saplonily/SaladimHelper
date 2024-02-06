using Celeste.Mod.Entities;

// modified from CherryHelper (https://github.com/aridai-shi/CherryHelper)

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/FallTeleportMirror")]
public class FallTeleportMirror : Entity
{
    private struct Debris
    {
        public Vector2 Direction;
        public float Percent;
        public float Duration;
        public bool Enabled;
    }

    private class Bg : Entity
    {
        private MirrorSurface surface;
        private Vector2[] offsets;
        private List<MTexture> textures;

        public Bg(Vector2 position) : base(position)
        {
            Depth = 9500;
            textures = GFX.Game.GetAtlasSubtextures("objects/temple/portal/reflection");
            Vector2 vector = new(10f, 4f);
            offsets = new Vector2[textures.Count];
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] = vector + new Vector2(Calc.Random.Range(-4, 4), Calc.Random.Range(-4, 4));
            }
            Add(surface = new MirrorSurface(null));
            surface.OnRender = () =>
            {
                for (int j = 0; j < textures.Count; j++)
                {
                    surface.ReflectionOffset = offsets[j];
                    textures[j].DrawCentered(Position, surface.ReflectionColor);
                }
            };
        }

        public override void Render()
        {
            MTexture mtexture = GFX.Game["objects/temple/portal/surface"];
            mtexture.DrawCentered(Position);
        }
    }

    public static ParticleType P_CurtainDrop;
    public float DistortionFade = 1f;
    public bool seamlessNextChapter;
    public string nextChapterSSID;
    public AreaMode chapterAreaMode;
    public string frameSpritePath;
    private bool canTrigger;
    private VirtualRenderTarget buffer;
    private float bufferAlpha;
    private float bufferTimer;
    private Debris[] debris = new Debris[50];
    private Color debrisColorFrom = Calc.HexToColor("f442d4");
    private Color debrisColorTo = Calc.HexToColor("000000");
    private MTexture debrisTexture = GFX.Game["particles/blob"];
    private string toLevel;
    private bool endLevel;
    private bool stopMusic;
    private string fadeParameter;

    public FallTeleportMirror(Vector2 position) : base(position)
    {
        Depth = 2000;
        Collider = new Hitbox(100f, 54f, -50f, -22f);
        Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
        canTrigger = true;
    }

    public FallTeleportMirror(EntityData data, Vector2 offset) : this(data.Position + offset)
    {
        toLevel = data.Attr("toLevel", "");
        endLevel = data.Bool("endLevel", false);
        nextChapterSSID = data.Attr("mapSID", "");
        seamlessNextChapter = data.Bool("seamlessNextChapter", true);
        frameSpritePath = data.Attr("frameSprite", "");
        stopMusic = data.Bool("stopMusic", true);
        fadeParameter = data.Attr("fadeParameter", "fade");
        chapterAreaMode = data.Attr("side", "") switch
        {
            "A" => AreaMode.Normal,
            "B" => AreaMode.BSide,
            "C" => AreaMode.CSide,
            _ => AreaMode.Normal
        };
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        scene.Add(new Bg(Position));
    }

    public IEnumerator ActivateRoutine()
    {
        Level level = Scene as Level;
        LightingRenderer light = level.Lighting;
        float debrisStart = 0f;
        Add(new BeforeRenderHook(BeforeRender));
        Add(new DisplacementRenderHook(RenderDisplacement));
        while (true)
        {
            bufferAlpha = Calc.Approach(bufferAlpha, 1f, Engine.DeltaTime);
            bufferTimer += 4f * Engine.DeltaTime;
            light.Alpha = Calc.Approach(light.Alpha, 0.2f, Engine.DeltaTime * 0.25f);
            if (debrisStart < debris.Length)
            {
                int index = (int)debrisStart;
                debris[index].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
                debris[index].Enabled = true;
                debris[index].Duration = 0.5f + Calc.Random.NextFloat(0.7f);
            }
            debrisStart += Engine.DeltaTime * 10f;
            int num3;
            for (int i = 0; i < debris.Length; i = num3 + 1)
            {
                bool enabled = debris[i].Enabled;
                if (enabled)
                {
                    Debris[] array = debris;
                    int num = i;
                    array[num].Percent = array[num].Percent % 1f;
                    Debris[] array2 = debris;
                    int num2 = i;
                    array2[num2].Percent = array2[num2].Percent + Engine.DeltaTime / debris[i].Duration;
                }
                num3 = i;
            }
            yield return null;
        }
    }

    private void BeforeRender()
    {
        buffer ??= VirtualContent.CreateRenderTarget("temple-portal", 120, 64, false, true, 0);
        Vector2 vector = new Vector2(buffer.Width, buffer.Height) / 2f;
        MTexture mtexture = GFX.Game["objects/temple/portal/portal"];
        Engine.Graphics.GraphicsDevice.SetRenderTarget(buffer);
        Engine.Graphics.GraphicsDevice.Clear(Color.Black);
        Draw.SpriteBatch.Begin();
        int num = 0;
        while (num < 10f)
        {
            float num2 = bufferTimer % 1f * 0.1f + num / 10f;
            Color color = Color.Lerp(Color.Black, Color.Purple, num2);
            float num3 = MathHelper.TwoPi * num2;
            mtexture.DrawCentered(vector, color, num2, num3);
            num++;
        }
        Draw.SpriteBatch.End();
    }

    private void RenderDisplacement()
    {
        Draw.Rect(X - 60f, Y - 32f, 120f, 64f, new Color(0.5f, 0.5f, 0.25f * DistortionFade * bufferAlpha, 1f));
    }

    public override void Render()
    {
        base.Render();
        if (buffer != null)
            Draw.SpriteBatch.Draw(buffer, Position + new Vector2(-60f, -32f), Color.White * bufferAlpha);
        if (frameSpritePath == "" || GFX.Game[frameSpritePath].get_AtlasPath() == null)
            GFX.Game["objects/temple/portal/portalframe"].DrawCentered(Position);
        else
            GFX.Game[frameSpritePath].DrawCentered(Position);
        Level level = Scene as Level;

        for (int i = 0; i < debris.Length; i++)
        {
            Debris d = debris[i];
            if (d.Enabled)
            {
                float num = Ease.SineOut(d.Percent);
                Vector2 vector = Position + d.Direction * (1f - num) * (190f - level.Zoom * 30f);
                Color color = Color.Lerp(debrisColorFrom, debrisColorTo, num);
                float num2 = Calc.LerpClamp(1f, 0.2f, num);
                debrisTexture.DrawCentered(vector, color, num2, i * 0.05f);
            }
        }
    }

    private void OnPlayer(Player player)
    {
        if (canTrigger)
        {
            canTrigger = false;
            Scene.Add(new CS_FallTeleport(
                player, this,
                toLevel, endLevel,
                seamlessNextChapter, nextChapterSSID,
                chapterAreaMode, stopMusic,
                fadeParameter)
                );
        }
    }

    public override void Removed(Scene scene)
    {
        Dispose();
        base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
        Dispose();
        base.SceneEnd(scene);
    }

    private void Dispose()
    {
        buffer?.Dispose();
        buffer = null;
    }
}

public class CS_FallTeleport : CutsceneEntity
{
    private class Fader : Entity
    {
        public float Target;
        public bool Ended;
        public float fade;

        public Fader()
        {
            Depth = -1000000;
        }

        public override void Update()
        {
            fade = Calc.Approach(fade, Target, Engine.DeltaTime * 0.5f);
            if (Target <= 0f && fade <= 0f && Ended)
            {
                RemoveSelf();
            }
            base.Update();
        }

        public override void Render()
        {
            Camera camera = (Scene as Level).Camera;
            if (fade > 0f)
                Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * fade);
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null && !player.OnGround(2))
                player.Render();
        }
    }

    public string toLevel;
    public string nextChapterSSID = "";
    private Player player;
    private FallTeleportMirror portal;
    private bool EndLevel;
    private bool seamlessNextChapter;
    private AreaMode chapterAreaMode;
    private Fader fader;
    private SoundSource sfx;
    private bool stopMusic;
    private string fadeParameter;

    public CS_FallTeleport(
        Player player,
        FallTeleportMirror portal,
        string toLevel,
        bool EndLevel,
        bool seamlessNextChapter,
        string nextChapterSSID,
        AreaMode chapterAreaMode,
        bool stopMusic,
        string fadeParameter
        )
    {
        this.player = player;
        this.portal = portal;
        this.toLevel = toLevel;
        this.EndLevel = EndLevel;
        this.nextChapterSSID = nextChapterSSID;
        this.seamlessNextChapter = seamlessNextChapter;
        this.chapterAreaMode = chapterAreaMode;
        this.stopMusic = stopMusic;
        this.fadeParameter = fadeParameter;
    }

    public override void OnBegin(Level level)
    {
        Add(new Coroutine(Cutscene(level), true));
        level.Add(fader = new Fader());
        level.InCutscene = false;
        level.CancelCutscene();
    }

    private IEnumerator Cutscene(Level level)
    {
        player.StateMachine.State = Player.StDummy;
        player.StateMachine.Locked = true;
        player.Dashes = 1;
        if (stopMusic)
            Audio.SetMusic(null);
        Add(sfx = new SoundSource());
        sfx.Position = portal.Center;
        Add(new Coroutine(CenterCamera()));
        player.DummyAutoAnimate = false;
        player.Sprite.Play("lookUp");
        yield return 1f;
        player.DummyAutoAnimate = true;

        Add(new Coroutine(portal.ActivateRoutine()));

        sfx.Play("event:/new_content/game/10_farewell/glitch_short", null, 0f);
        Add(new Coroutine(FadeOutMusicRoutine()));
        Add(new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 3f, 12f)));
        yield return 0.25f;
        player.ForceStrongWindHair.X = -1f;
        yield return 0.5f;
        player.Facing = Facings.Right;
        player.DummyAutoAnimate = false;
        sfx.Play("event:/new_content/game/10_farewell/glitch_long", null, 0f);
        player.DummyGravity = false;
        player.Sprite.Play("fallFast", false, false);
        player.Sprite.Rate = 1f;
        Vector2 target = portal.Center + new Vector2(0f, 8f);
        Vector2 from = player.Position;
        for (float i = 0f; i < 1f; i += Engine.DeltaTime * 2f)
        {
            player.Position = from + (target - from) * Ease.SineInOut(i);
            yield return null;
        }
        player.ForceStrongWindHair.X = 0f;
        fader.Target = 1f;
        yield return 2f;
        player.Sprite.Play("fallFast", false, false);
        yield return 1f;
        while ((portal.DistortionFade -= Engine.DeltaTime * 2f) > 0f)
            yield return null;
        player.Sprite.Visible = false;
        player.Hair.Visible = false;
        sfx.Play("event:/char/badeline/disappear", null, 0f);
        yield return 2f;
        if (seamlessNextChapter && nextChapterSSID != "")
        {
            if (EndLevel)
                level.RegisterAreaComplete();
            LevelEnter.Go(new Session(AreaData.Get(nextChapterSSID).ToKey(chapterAreaMode), null, null), false);
        }
        else
        {
            if (EndLevel)
                level.CompleteArea(false, true, true);
            else
            {
                level.OnEndOfFrame += () =>
                {
                    if (fader != null && !WasSkipped)
                    {
                        fader.Tag = Tags.Global;
                        fader.fade = 0f;
                        fader.Target = 0f;
                        fader.Ended = true;
                    }
                    Leader.StoreStrawberries(player.Leader);
                    level.Remove(player);
                    level.UnloadLevel();
                    level.Session.Keys.Clear();
                    level.Session.Level = toLevel;
                    level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
                    level.LoadLevel(Player.IntroTypes.WakeUp, false);
                    Audio.SetMusicParam("fade", 1f);
                    Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
                    level.Camera.Y -= 8f;
                    fader?.RemoveTag(Tags.Global);
                };
            }
        }
        yield break;
    }

    private IEnumerator CenterCamera()
    {
        Camera camera = Level.Camera;
        Vector2 target = portal.Center - new Vector2(160f, 90f);
        while ((camera.Position - target).Length() > 1f)
        {
            camera.Position += (target - camera.Position) * (1f - (float)Math.Pow(0.01, (double)Engine.DeltaTime));
            yield return null;
        }
        yield break;
    }

    private IEnumerator FadeOutMusicRoutine()
    {
        for (float p = 1f; p > 0f; p -= Engine.DeltaTime)
        {
            Audio.SetMusicParam(fadeParameter, p);
            yield return null;
        }
        Audio.SetMusicParam(fadeParameter, 0f);
        yield break;
    }

    public override void OnEnd(Level level)
    {
        level.OnEndOfFrame += () =>
        {
            if (fader != null && !WasSkipped)
            {
                fader.Tag = Tags.Global;
                fader.fade = 0f;
                fader.Target = 0f;
                fader.Ended = true;
            }
            Leader.StoreStrawberries(player.Leader);
            level.Remove(player);
            level.UnloadLevel();
            level.Session.Keys.Clear();
            level.Session.Level = toLevel;
            level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Top)));
            level.LoadLevel(Player.IntroTypes.WakeUp, false);
            Audio.SetMusicParam("fade", 1f);
            Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
            level.Camera.Y -= 8f;
            if (fader != null)
            {
                fader.RemoveTag(Tags.Global);
            }
        };
    }
}