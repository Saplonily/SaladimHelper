using Celeste.Mod.SaladimHelper.Entities;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

public class SaladimHelperSession : EverestModuleSession
{
    public SessionFlags SessionFlags;

    public List<FilterEntry> FilterEntries = new();

    public HashSet<int> ShopBoughtItems = new();
    public int CollectedCoinsAmount = 0;
    [YamlIgnore] public (Vector2 speed, float mul)? MomentumRefillSpeedKept = null;
    [YamlIgnore] public CoinDisplayer CurrentCoinDisplayer = null;
    [YamlIgnore] public (Ease.Easer easer, float threshold, float speed, float duration)? CustomTransition = null;

    public SaladimHelperSession()
    {
        SessionFlags = new();
    }

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