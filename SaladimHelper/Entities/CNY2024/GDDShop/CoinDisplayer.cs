namespace Celeste.Mod.SaladimHelper.Entities;

[Tracked]
public class CoinDisplayer : TotalCustomDisplay
{
    public bool AlwaysDisplay;

    public CoinDisplayer(bool silence)
        : base(MakeCoinCounter(silence), ThirdPartyHelpers.DeathTrackerInstalled ? 277f : 202f)
    {
    }

    private static CoinCounter MakeCoinCounter(bool silence)
        => new CoinCounter(false, ModuleSession.CollectedCoinsCount - (silence ? 0 : 1));

    public override int GetValue()
        => ModuleSession.CollectedCoinsCount;

    public override bool GetNeedLerpIn()
        => base.GetNeedLerpIn() || AlwaysDisplay;

    public static CoinDisplayer Display(Scene scene, bool silent = false, bool setAmountAgain = false)
    {
        var cur = scene.Tracker.GetEntity<CoinDisplayer>();

        if (cur is null)
        {
            // tracker won't update on Scene.Add call
            cur = scene.Entities.ToAdd.OfType<CoinDisplayer>().SingleOrDefault();
            if (cur is null)
            {
                cur = new(silent);
                scene.Add(cur);
            }
        }
        if (setAmountAgain)
            cur.counter.Amount = ModuleSession.CollectedCoinsCount;
        return cur;
    }
}
