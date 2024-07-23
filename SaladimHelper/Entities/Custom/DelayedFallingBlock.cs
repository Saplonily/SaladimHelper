using System.Reflection;
using Celeste.Mod.Entities;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/DelayedFallingBlock"), NeedModuleInit]
public class DelayedFallingBlock : FallingBlock
{
    public float preDelay = 0.2f;
    public float playerWaitDelay = 0.4f;
    public bool noSfx = false;

    public DelayedFallingBlock(Vector2 position, char tile,
        int width, int height,
        bool behind, bool climbFall,
        float preDelay, float playerWaitDelay,
        bool noSfx, bool autoFall)
        : base(position, tile, width, height, false, behind, climbFall)
    {
        this.preDelay = preDelay;
        this.playerWaitDelay = playerWaitDelay;
        this.noSfx = noSfx;
        if (autoFall)
            Triggered = true;
    }

    public DelayedFallingBlock(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Char("tiletype", '3'),
              data.Width, data.Height,
              data.Bool("behind"), data.Bool("climbFall"),
              data.Float("preDelay", 0.2f), data.Float("playerWaitDelay", 0.4f),
              data.Bool("noSfx", false), data.Bool("autoFall", false))
    {
    }

    private static ILHook fallingBlockCoroutineHook;

    private static void OnFallingBlockCoroutine(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(ins => ins.MatchLdcR4(0.2f)))
        {
            cur.Remove();
            cur.Emit(OpCodes.Ldloc_1);
            cur.EmitDelegate<Func<FallingBlock, float>>(f => f is DelayedFallingBlock d ? d.preDelay : 0.2f);
        }
        cur.Index = 0;
        if (cur.TryGotoNext(ins => ins.MatchLdcR4(0.4f)))
        {
            cur.Remove();
            cur.Emit(OpCodes.Ldloc_1);
            cur.EmitDelegate<Func<FallingBlock, float>>(f => f is DelayedFallingBlock d ? d.playerWaitDelay : 0.4f);
        }
    }

    private static void FallingBlock_ShakeSfx(On.Celeste.FallingBlock.orig_ShakeSfx orig, FallingBlock self)
    {
        if (self is DelayedFallingBlock d && d.noSfx)
            return;
        orig(self);
    }

    public static void Load()
    {
        fallingBlockCoroutineHook = new ILHook(
            typeof(FallingBlock).GetMethod("Sequence", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(),
            OnFallingBlockCoroutine
            );
        On.Celeste.FallingBlock.ShakeSfx += FallingBlock_ShakeSfx;
    }

    public static void Unload()
    {
        fallingBlockCoroutineHook.Dispose();
        On.Celeste.FallingBlock.ShakeSfx -= FallingBlock_ShakeSfx;
    }
}