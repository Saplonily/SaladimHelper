using Celeste.Mod.SaladimHelper.Entities;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSession : EverestModuleSession
{
    public SessionFlags SessionFlags;

    public HashSet<int> ShopBoughtItems = new();
    public int CollectedCoinsAmount = 0;
    [YamlIgnore] public (Vector2 speed, float mul)? MomentumRefillSpeedKept = null;
    [YamlIgnore] public CoinDisplayer CurrentCoinDisplayer = null;
    [YamlIgnore] public (Ease.Easer easer, float threshold, float speed, float duration)? CustomTransition = null;

    public SaladimHelperSession()
    {
        SessionFlags = new();
    }
}