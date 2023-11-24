using Microsoft.Xna.Framework.Graphics;

using YamlDotNet.Serialization;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public sealed class FilterEntry
{
    public string EffectPath { get; private set; }
    public float Index { get; private set; }

    [YamlIgnore]
    public Effect Effect { get; private set; }
    public float Strength { get; set; }

    // for yaml deserialization
    public FilterEntry() { }

    public FilterEntry(string effectPath, float index)
    {
        EffectPath = effectPath;
        Index = index;

        Strength = 0.0f;
        // load our Effect
        Effect = LoadEffect($"Effects/{effectPath}.xnb");
        if (Effect is null)
            EffectPath = null;
    }

    public static Effect LoadEffect(string path)
    {
        if (Everest.Content.TryGet(path, out var modAsset))
        {
            // FIXME it's a magic number
            int headLength = 168;
            using var s = modAsset.Stream;
            s.Seek(headLength, SeekOrigin.Begin);
            byte[] code = new byte[s.Length - headLength];
            s.Read(code, 0, (int)s.Length - headLength);
            return new(Engine.Graphics.GraphicsDevice, code);
        }
        return null;
    }

    public static Ease.Easer GetEaserWithName(string name)
        => name switch
        {
            "Linear" => Ease.Linear,
            "SineIn" => Ease.SineIn,
            "SineOut" => Ease.SineOut,
            "SineInOut" => Ease.SineInOut,
            "QuadIn" => Ease.QuadIn,
            "QuadOut" => Ease.QuadOut,
            "QuadInOut" => Ease.QuadInOut,
            "CubeIn" => Ease.CubeIn,
            "CubeOut" => Ease.CubeOut,
            "CubeInOut" => Ease.CubeInOut,
            "QuintIn" => Ease.QuintIn,
            "QuintOut " => Ease.QuintOut,
            "QuintInOut" => Ease.QuintInOut,
            "ExpoIn" => Ease.ExpoIn,
            "ExpoOut" => Ease.ExpoOut,
            "ExpoInOut" => Ease.ExpoInOut,
            "BackIn" => Ease.BackIn,
            "BackOut" => Ease.BackOut,
            "BackInOut" => Ease.BackInOut,
            "BigBackIn" => Ease.BigBackIn,
            "BigBackOut" => Ease.BigBackOut,
            "BigBackInOut" => Ease.BigBackInOut,
            "ElasticIn" => Ease.ElasticIn,
            "ElasticOut" => Ease.ElasticOut,
            "ElasticInOut" => Ease.ElasticInOut,
            "BounceIn " => Ease.BounceIn,
            "BounceOut" => Ease.BounceOut,
            "BounceInOut" => Ease.BounceInOut,
            _ => Ease.Linear
        };

    public static void Load()
    {
        IL.Celeste.Level.Render += Level_Render;
    }

    private static void Level_Render(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchCall("Celeste.Glitch", "Apply")))
        {
            Logger.Log(LogLevel.Info, ModuleName, $"Applying Filter applying hook at {cur.Index}.");
            cur.EmitDelegate(ApplyInvertColor);
        }
    }

    private static void ApplyInvertColor()
    {
        foreach (var entry in ModuleSession.FilterEntries)
        {
            if (entry.EffectPath is null) continue;
            if (entry.Strength <= 0.0f) continue;

            entry.Effect ??= LoadEffect($"Effects/{entry.EffectPath}.xnb");

            var device = Engine.Instance.GraphicsDevice;
            var batch = Draw.SpriteBatch;

            entry.Effect.Parameters["Strength"].SetValue(entry.Strength / 100.0f);
            device.SetRenderTarget(GameplayBuffers.TempA);
            device.Clear(Color.Transparent);
            batch.Begin(
                SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone, entry.Effect
                );
            batch.Draw(GameplayBuffers.Level, Vector2.Zero, Color.White);
            batch.End();
            device.SetRenderTarget(GameplayBuffers.Level);
            device.Clear(Color.Transparent);
            batch.Begin(
                SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone, null
                );
            batch.Draw(GameplayBuffers.TempA, Vector2.Zero, Color.White);
            batch.End();
        }
    }

    public static void Unload()
    {
        IL.Celeste.Level.Render -= Level_Render;
    }
}
