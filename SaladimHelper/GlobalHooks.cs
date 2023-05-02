using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class GlobalHooks
{
    public static void Load()
    {
        //On.Celeste.BounceBlock.Render += BounceBlock_Render;
    }

    public static void Unload()
    {
        //On.Celeste.BounceBlock.Render -= BounceBlock_Render;
    }

    //public static void BounceBlock_Render(On.Celeste.BounceBlock.orig_Render orig, BounceBlock self)
    //{
    //    orig(self);
    //    DynData<BounceBlock> dynData = new(self);
    //    PixelFont font = Dialog.Languages["english"].Font;

    //    font.Draw(
    //        64f,
    //        $"{dynData["state"]}",
    //        self.Position,
    //        Vector2.One / 2,
    //        Vector2.One / 4,
    //        Color.White
    //        );
    //}
}
