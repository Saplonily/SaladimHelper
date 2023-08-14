using Celeste.Mod.SaladimHelper.Entities;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSession : EverestModuleSession
{
    public HashSet<EntityID> CollectedCoins = new();
    public HashSet<int> ShopBoughtItems = new();

    [YamlIgnore] public int CollectedCoinsAmount => CollectedCoins.Count;

    public Vector2? MomentumRefillSpeedKept = null;

    [YamlIgnore] public CoinDisplayer CurrentCoinDisplayer = null;
    public bool EnabledFrostFreeze = false;
}