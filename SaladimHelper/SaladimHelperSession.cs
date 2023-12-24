using Celeste.Mod.SaladimHelper.Entities;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSession : EverestModuleSession
{
    public HashSet<EntityID> CollectedCoins = new();
    public HashSet<int> ShopBoughtItems = new();

    [YamlIgnore] public int CollectedCoinsAmount = 114514;

    public Vector2? MomentumRefillSpeedKept = null;

    [YamlIgnore] public CoinDisplayer CurrentCoinDisplayer = null;
    public bool EnabledFrostFreeze = false;

    public List<FilterEntry> FilterEntries = new();

    public FilterEntry GetFilterEntry(string effectPath, float index)
    {
        var first = FilterEntries.FirstOrDefault(e => e.EffectPath == effectPath && e.Index == index);
        if (first is not null) return first;
        FilterEntry entry = new(effectPath, index);
        FilterEntries.Add(entry);
        FilterEntries.Sort((a, b) => a.Index.CompareTo(b.Index));
        return entry;
    }
}