using Celeste.Mod.SaladimHelper.Entities;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSession : EverestModuleSession
{
    public bool EnabledFrostFreeze = false;
    public List<FilterEntry> FilterEntries = new();

    public (Vector2? speed, float mul) MomentumRefillSpeedKept = (null, 1.0f);
    public HashSet<EntityID> CollectedCoins = new();
    public HashSet<int> ShopBoughtItems = new();
#if DEBUG
    [YamlIgnore] public int CollectedCoinsAmount = 200;
#else

    [YamlIgnore] public int CollectedCoinsAmount = 0;
#endif
    [YamlIgnore] public CoinDisplayer CurrentCoinDisplayer = null;

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