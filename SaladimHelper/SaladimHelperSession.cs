using Celeste.Mod.SaladimHelper.Entities;
using Celeste.Mod.SaladimHelper.Triggers;
using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public sealed class SaladimHelperSession : EverestModuleSession
{
    public SessionFlags SessionFlags;

    public HashSet<int> ShopBoughtItems = new();
    public int CollectedCoinsCount = 0;

    [YamlIgnore] 
    public (Vector2 speed, float mul)? MomentumRefillSpeedKept = null;

    [YamlIgnore] 
    public CoinDisplayer CurrentCoinDisplayer = null;

    [YamlIgnore] 
    internal CustomTransitionParameters? CustomTransition = null;

    public SaladimHelperSession()
    {
        SessionFlags = new();
    }
}