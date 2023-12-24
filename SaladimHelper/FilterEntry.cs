using Microsoft.Xna.Framework.Graphics;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public sealed class FilterEntry
{
    public string EffectPath { get; private set; }
    public float Index { get; private set; }

    [YamlIgnore]
    public Effect Effect { get; internal set; }
    public float Strength { get; set; }

    // for yaml deserialization
    public FilterEntry() { }

    public FilterEntry(string effectPath, float index)
    {
        EffectPath = effectPath;
        Index = index;

        Strength = 0.0f;
        // load our Effect
        Effect = FilterModule.LoadEffect($"Effects/{effectPath}.xnb");
        if (Effect is null)
            EffectPath = null;
    }
}
