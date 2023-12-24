namespace Celeste.Mod.SaladimHelper.Entities;

public class CoinDisplayer : TotalCustomDisplay
{
    public bool AlwaysDisplay;

    public CoinDisplayer(bool silence)
        : base(MakeCoinCounter(silence), CommonModule.DeathTrackerLoaded ? 277f : 202f)
    {
    }

    private static CoinCounter MakeCoinCounter(bool silence)
        => new CoinCounter(false, ModuleSession.CollectedCoinsAmount - (silence ? 0 : 1), "" /* TODO */);

    public override int GetValue()
        => ModuleSession.CollectedCoinsAmount;

    public override bool GetNeedLerpIn()
        => base.GetNeedLerpIn() || AlwaysDisplay;

    public static CoinDisplayer Display(Scene scene, bool silence = false, bool setAmountAgain = false)
    {
        var cur = ModuleSession.CurrentCoinDisplayer;

        // 如果当前没有一个 coin displayer 才去new
        // 注意 new 完 Add Scene 后这一帧是没有这个实体的
        // 所以得同时检查是否 ToAdd 有这个实体
        // 不然会出现同时吃金币出现俩 displayer 的 bug
        if (cur is null || (cur?.Scene != scene && !scene.Entities.ToAdd.Contains(cur)))
        {
            cur = new(silence);
            ModuleSession.CurrentCoinDisplayer = cur;
            scene.Add(cur);
        }
        if (setAmountAgain)
            cur.counter.Amount = ModuleSession.CollectedCoinsAmount;
        return cur;
    }
}
