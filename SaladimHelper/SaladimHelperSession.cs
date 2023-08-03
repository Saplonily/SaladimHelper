using Celeste.Mod.SaladimHelper.Entities;
using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSession : EverestModuleSession
{
    public HashSet<EntityID> CollectedCoins = new();
    public Dictionary<string, int> GroupToCoinsMap = new();

    [YamlIgnore]
    public CoinDisplayer CurrentCoinDisplayer = null;
}