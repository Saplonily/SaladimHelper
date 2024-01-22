namespace Celeste.Mod.SaladimHelper.Entities;

public class TotalCustomDisplay : Entity
{
    public const float NumberUpdateDelay = 0.4f;
    public const float ComboUpdateDelay = 0.3f;
    public const float AfterUpdateDelay = 1.7f;
    public const float LerpInSpeed = 1.2f;
    public const float LerpOutSpeed = 2f;

    public static Color FlashColor;
    public float DrawLerp;
    private bool updateLerping;
    protected MTexture bg;
    protected StrawberriesCounter counter;
    protected float yoffset;

    public TotalCustomDisplay(StrawberriesCounter counter, float yoffset = 96f)
    {
        Y = yoffset;
        Depth = -101;
        Tag = Tags.HUD | Tags.PauseUpdate | Tags.TransitionUpdate | Tags.Global | Tags.Persistent;
        bg = GFX.Gui["strawberryCountBG"];
        this.counter = counter;
        this.yoffset = yoffset;
        Add(counter);
        Add(new Coroutine(UpdateCoroutine()));
    }

    public override void Update()
    {
        base.Update();
        var level = SceneAs<Level>();
        if (Visible)
        {
            float thisYOffset = yoffset;
            if (!level.TimerHidden)
            {
                thisYOffset += Settings.Instance.SpeedrunClock switch
                {
                    SpeedrunType.Chapter => 58f,
                    SpeedrunType.File => 78f,
                    _ => 0f
                };
            }
            Y = Calc.Approach(Y, thisYOffset, Engine.DeltaTime * 800f);
        }
        DrawLerp = GetNeedLerpIn()
            ? Calc.Approach(DrawLerp, 1f, LerpInSpeed * Engine.RawDeltaTime)
            : Calc.Approach(DrawLerp, 0f, LerpOutSpeed * Engine.RawDeltaTime);
        Visible = DrawLerp > 0f;
    }

    public IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            bool GetNeedUpdate() => GetValue() > counter.Amount;
            if (GetNeedUpdate())
            {
                while (true)
                {
                    updateLerping = true;
                    if (DrawLerp != 1f)
                        yield return null;
                    else
                        break;
                }
                yield return NumberUpdateDelay;
                do
                {
                    counter.Amount += 1;
                    yield return ComboUpdateDelay;
                }
                while (GetNeedUpdate());
                yield return AfterUpdateDelay;
                updateLerping = false;
            }
            yield return null;
        }
    }

    public virtual int GetValue() => 0;
    public virtual bool GetNeedLerpIn()
        => updateLerping || (SceneAs<Level>().Paused && SceneAs<Level>().PauseMainMenuOpen);

    public override void Render()
    {
        Vector2 pos = Vector2.Lerp(new Vector2(-bg.Width, Y), new Vector2(32f, Y), Ease.CubeOut(DrawLerp)).Round();
        bg.DrawJustified(pos + new Vector2(-96f, 12f), new Vector2(0f, 0.5f));
        counter.Position = pos + new Vector2(0f, -Y);
        counter.Render();
    }
}