namespace Celeste.Mod.SaladimHelper.Entities;

public class CoinDisplayer : Entity
{
    public string Group;

    public CoinDisplayer()
    {
        Tag |= Tags.HUD | Tags.Persistent | Tags.TransitionUpdate | Tags.PauseUpdate;

    }

    public override void Update()
    {
        base.Update();
        if (Input.CrouchDash.Pressed)
        {
            Input.CrouchDash.ConsumeBuffer();
            Logger.Log(LogLevel.Info, ModuleName, "---------");
            foreach (var item in Scene)
            {
                if (item.TagCheck(Tags.HUD))
                {
                    Logger.Log(LogLevel.Info, ModuleName, $"{item.GetType().FullName}");
                }
            }
            Logger.Log(LogLevel.Info, ModuleName, "---------");
        }
    }

    public override void Render()
    {
        base.Render();
        ActiveFont.Draw($"> {ModuleSession.GroupToCoinsMap[Group]}", Position + Vector2.UnitY * 120f, Color.White);
    }
}

// oh we have to do some fucking cleaning works

[Tracked(false)]
internal class TotalCollectableDisplay : Entity
{
    public const float NumberUpdateDelay = 0.4f;
    public const float ComboUpdateDelay = 0.3f;
    public const float AfterUpdateDelay = 2f;
    public const float LerpInSpeed = 1.2f;
    public const float LerpOutSpeed = 2f;
    public static readonly Color FlashColor = Calc.HexToColor("FF5E76");

    private MTexture bg;
    private float strawberriesUpdateTimer;
    private float strawberriesWaitTimer;
    private float baseYPos;

    public float XPosPercent;
    public CollectableCounter Counter;
    public Func<int> GetValue;

    public TotalCollectableDisplay(float Ypos, Func<int> del, int playSound, MTexture graphic)
    {
        Y = Ypos;
        baseYPos = Ypos;
        Depth = -100;
        GetValue = del;
        Tag = Tags.HUD | Tags.PauseUpdate | Tags.Persistent | Tags.TransitionUpdate;
        bg = GFX.Gui["strawberryCountBG"];

        Add(Counter = new CollectableCounter(false, GetValue(), playSound, graphic));
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        bool valueNeedUpdate = GetValue() > Counter.Amount;

        if (valueNeedUpdate && strawberriesUpdateTimer <= 0f)
            strawberriesUpdateTimer = 0.4f;

        if (valueNeedUpdate || strawberriesUpdateTimer > 0f || strawberriesWaitTimer > 0f || (level.Paused && level.PauseMainMenuOpen))
            XPosPercent = Calc.Approach(XPosPercent, 1f, 1.2f * Engine.RawDeltaTime);
        else
            XPosPercent = Calc.Approach(XPosPercent, 0f, 2f * Engine.RawDeltaTime);

        if (strawberriesWaitTimer > 0f)
            strawberriesWaitTimer -= Engine.RawDeltaTime;

        if (strawberriesUpdateTimer > 0f && XPosPercent == 1f)
        {
            strawberriesUpdateTimer -= Engine.RawDeltaTime;
            if (strawberriesUpdateTimer <= 0f)
            {
                if (valueNeedUpdate)
                {
                    Counter.Amount++;
                }
                strawberriesWaitTimer = 2f;
                if (valueNeedUpdate)
                {
                    strawberriesUpdateTimer = 0.3f;
                }
            }
        }
        if (Visible)
        {
            float ypos = baseYPos;
            if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
                ypos += 58f;
            else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
                ypos += 78f;

            Y = Calc.Approach(Y, ypos, Engine.DeltaTime * 800f);
        }
        Visible = XPosPercent > 0f;
    }

    public override void Render()
    {
        Vector2 finalPosition = Vector2.Lerp(new Vector2(-bg.Width, Y), new Vector2(32f, Y), Ease.CubeOut(XPosPercent)).Round();
        bg.DrawJustified(finalPosition + new Vector2(-96f, 12f), new(0f, 0.5f));
        Counter.Position = finalPosition + new Vector2(0f, 0f - Y);
        base.Render();
    }
}

public class CollectableCounter : Component
{
    public static readonly Color FlashColor = Calc.HexToColor("FF5E76");
    private const int IconWidth = 60;

    public bool Golden;
    public Vector2 Position;
    public bool CenteredX;
    public bool CanWiggle;
    public float Scale;
    public float Stroke;
    public float Rotation;
    public Color Color;
    public Color OutOfColor;
    public bool OverworldSfx;
    private int amount;
    private int outOf;
    private Wiggler wiggler;
    private float flashTimer;
    private string sAmount;
    private string sOutOf;
    private MTexture x;
    private bool showOutOf;
    private MTexture graphicID;
    private int playSound;

    public int Amount
    {
        get
        {
            return amount;
        }
        set
        {
            bool flag = amount != value;
            bool flag2 = amount < value && playSound >= 1;
            if (!flag)
            {
                return;
            }
            amount = value;
            UpdateStrings();
            if (!CanWiggle)
            {
                return;
            }
            bool overworldSfx = OverworldSfx;
            if (flag2)
            {
                if (overworldSfx)
                {
                    Audio.Play(Golden ? "event:/ui/postgame/goldberry_count" : "event:/ui/postgame/strawberry_count");
                }
                else
                {
                    Audio.Play(playSound == 2 ? "event:/ui/postgame/goldberry_count" : "event:/ui/game/increment_strawberry");
                }
            }
            wiggler.Start();
            flashTimer = 0.5f;
        }
    }

    public int OutOf
    {
        get
        {
            return outOf;
        }
        set
        {
            outOf = value;
            UpdateStrings();
        }
    }

    public bool ShowOutOf
    {
        get
        {
            return showOutOf;
        }
        set
        {
            if (showOutOf != value)
            {
                showOutOf = value;
                UpdateStrings();
            }
        }
    }

    public float FullHeight => Math.Max(ActiveFont.LineHeight, GFX.Gui["collectables/strawberry"].Height);

    public Vector2 RenderPosition => (((base.Entity != null) ? base.Entity.Position : Vector2.Zero) + Position).Round();

    public CollectableCounter(bool centeredX, int amount, int sound, MTexture graphic, int outOf = 0, bool showOutOf = false)
        : base(active: true, visible: true)
    {
        Golden = false;
        CanWiggle = true;
        Scale = 1f;
        Stroke = 2f;
        Rotation = 0f;
        Color = Color.White;
        OutOfColor = Color.LightGray;
        this.outOf = -1;
        CenteredX = centeredX;
        this.amount = amount;
        this.outOf = outOf;
        this.showOutOf = showOutOf;
        UpdateStrings();
        wiggler = Wiggler.Create(0.5f, 3f);
        wiggler.StartZero = true;
        wiggler.UseRawDeltaTime = true;
        x = GFX.Gui["x"];
        playSound = sound;
        graphicID = graphic;
    }

    private void UpdateStrings()
    {
        sAmount = amount.ToString();
        if (outOf > -1)
        {
            sOutOf = "/" + outOf;
        }
        else
        {
            sOutOf = "";
        }
    }

    public void Wiggle()
    {
        wiggler.Start();
        flashTimer = 0.5f;
    }

    public override void Update()
    {
        base.Update();
        if (wiggler.Active)
        {
            wiggler.Update();
        }
        if (flashTimer > 0f)
        {
            flashTimer -= Engine.RawDeltaTime;
        }
    }

    public override void Render()
    {
        Vector2 renderPos = RenderPosition;
        Vector2 dir = Calc.AngleToVector(Rotation, 1f);
        Vector2 dirPer = new(0f - dir.Y, dir.X);
        string text = (showOutOf ? sOutOf : "");
        float num = ActiveFont.Measure(sAmount).X;
        float num2 = ActiveFont.Measure(text).X;
        float num3 = 62f + (float)x.Width + 2f + num + num2;
        Color color = Color;
        if (flashTimer > 0f && base.Scene != null && base.Scene.BetweenRawInterval(0.05f))
        {
            color = FlashColor;
        }
        if (CenteredX)
        {
            renderPos -= dir * (num3 / 2f) * Scale;
        }
        float Scale2 = Scale * (78f / (float)graphicID.Height);
        graphicID.DrawCentered(renderPos + dir * 60f * 0.5f * Scale, Color.White, Scale2);
        x.DrawCentered(renderPos + dir * (62f + (float)x.Width * 0.5f) * Scale + dirPer * 2f * Scale, color, Scale);
        ActiveFont.DrawOutline(sAmount, renderPos + dir * (num3 - num2 - num * 0.5f) * Scale + dirPer * (wiggler.Value * 18f) * Scale, new Vector2(0.5f, 0.5f), Vector2.One * Scale, color, Stroke, Color.Black);
        if (text != "")
        {
            ActiveFont.DrawOutline(text, renderPos + dir * (num3 - num2 / 2f) * Scale, new Vector2(0.5f, 0.5f), Vector2.One * Scale, OutOfColor, Stroke, Color.Black);
        }
    }
}