using Mono.Cecil.Cil;

using MonoMod.RuntimeDetour;

namespace Celeste.Mod.SaladimHelper.Entities;

[NeedModuleInit]
public class CoinCounter : StrawberriesCounter
{
    public const string SfxIncrementStrawBerry = "event:/ui/game/increment_strawberry";

    public string Sfx;

    public CoinCounter(bool centeredX, int amount, string sfx)
        : base(centeredX, amount, 0, false) 
        => Sfx = sfx;


    public static void Load()
    {
        IL.Celeste.StrawberriesCounter.Render += StrawberriesCounter_Render;
    }

    public static void Unload()
    {
        IL.Celeste.StrawberriesCounter.Render -= StrawberriesCounter_Render;
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
}
