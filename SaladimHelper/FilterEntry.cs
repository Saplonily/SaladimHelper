using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public sealed class FilterEntry
{
    public string EffectPath { get; private set; }
    public float Index { get; private set; }

    public Effect Effect { get; private set; }
    public float Strength { get; set; }

    public FilterEntry(string effectPath, float index)
    {
        EffectPath = effectPath;
        Index = index;

        Strength = 0.0f;
        // load our Effect
        Effect effect = null;
        if (Everest.Content.TryGet($"Effects/{effectPath}.xnb", out var modAsset))
        {
            // FIXME it's a magic number
            int headLength = 168;
            using var s = modAsset.Stream;
            s.Seek(headLength, SeekOrigin.Begin);
            byte[] code = new byte[s.Length - headLength];
            s.Read(code, 0, (int)s.Length - headLength);
            effect = new(Engine.Graphics.GraphicsDevice, code);
        }
        Effect = effect;
    }


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
            if (entry.Effect is null) continue;
            if (entry.Strength <= 0.0f) continue;
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
