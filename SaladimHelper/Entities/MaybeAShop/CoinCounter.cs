using Mono.Cecil.Cil;

using MonoMod.RuntimeDetour;

namespace Celeste.Mod.SaladimHelper.Entities;

[NeedModuleInit]
public class CoinCounter : StrawberriesCounter
{
    public string Sfx;

    public CoinCounter(bool centeredX, int amount, string sfx)
        : base(centeredX, amount, 0, false)
    {
        Sfx = sfx;
    }

    public static ILHook ilhook_Amount_set;
    public const string SfxIncrementStrawBerry = "event:/ui/game/increment_strawberry";

    public static void Load()
    {
        ilhook_Amount_set = new ILHook(typeof(StrawberriesCounter).GetProperty("Amount").GetSetMethod(), IL_Amount_set);
        IL.Celeste.StrawberriesCounter.Render += StrawberriesCounter_Render;
    }

    private static void StrawberriesCounter_Render(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(ins => ins.MatchLdstr("collectables/strawberry")))
        {
            cur.Index++;
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate((string preStr, StrawberriesCounter b) => b is CoinCounter cc ? "SaladimHelper/coin" : preStr);
        }
    }

    public static void Unload()
    {
        ilhook_Amount_set.Dispose();
        IL.Celeste.StrawberriesCounter.Render -= StrawberriesCounter_Render;
    }

    public static void IL_Amount_set(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(ins => ins.MatchLdstr(SfxIncrementStrawBerry)))
        {
            cur.Index++;
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate((string preStr, StrawberriesCounter b) =>
            {
                if (b is CoinCounter cc)
                {
                    // TODO
                    return preStr;
                }
                else
                {
                    return preStr;
                }
            });
        }
    }
}
