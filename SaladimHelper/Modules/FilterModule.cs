using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class FilterModule
{
    public static void Load()
    {
        IL.Celeste.Level.Render += Level_Render;
    }

    public static void Unload()
    {
        IL.Celeste.Level.Render -= Level_Render;
    }

    private static void Level_Render(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchCall("Celeste.Glitch", "Apply")))
        {
            Logger.Log(LogLevel.Info, ModuleName, $"Applying Filter hook at {cur.Index}.");
            cur.EmitDelegate(ApplyFilters);
        }
    }

    private static void ApplyFilters()
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
            {
                batch.Begin(
                    SpriteSortMode.Deferred, BlendState.AlphaBlend,
                    SamplerState.PointClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, entry.Effect
                    );
                batch.Draw(GameplayBuffers.Level, Vector2.Zero, Color.White);
                batch.End();
            }
            device.SetRenderTarget(GameplayBuffers.Level);
            {
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
}
