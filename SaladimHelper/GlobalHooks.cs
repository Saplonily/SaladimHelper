using Celeste.Mod.SaladimHelper.Entities;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.SaladimHelper;

[NeedModuleInit]
public static class GlobalHooks
{
    //public static ILHook Player_orig_Update_ILHook;

    public static void Load()
    {
        //Player_orig_Update_ILHook = new ILHook(typeof(Player).GetMethod("orig_Update"), Player_orig_Update);
        //On.Celeste.Level.Update += Level_Update;
    }

    public static void Unload()
    {
        //Player_orig_Update_ILHook.Dispose(); 
        //On.Celeste.Level.Update -= Level_Update;
    }


    //private static void Level_Update(On.Celeste.Level.orig_Update orig, Level self)
    //{
    //    orig(self);
    //    if (Input.Grab.Pressed)
    //    {
    //        Input.Grab.ConsumeBuffer();
    //        self.Camera.Zoom *= 0.5f;
    //        self.Camera.Viewport.Width *= 2;
    //        self.Camera.Viewport.Height *= 2;
    //        //Logger.Log(LogLevel.Info, "1111", "how!");
    //    }
    //    //Logger.Log(LogLevel.Info, "1111", $"vp: {self.Camera.Viewport}");
    //}

    //private static void LavaRect_ctor(On.Celeste.LavaRect.orig_ctor orig, LavaRect self, float width, float height, int step)
    //{
    //    orig(self, width, height, 1);
    //}

    //private static void Player_orig_Update(ILContext il)
    //{
    //    ILCursor cur = new(il);
    //    if (cur.TryGotoNext(MoveType.After, DoInsMatch))
    //    {
    //        cur.Emit(OpCodes.Ldarg_0);
    //        cur.EmitDelegate((Player p) =>
    //        {
    //            Level l = p.SceneAs<Level>();
    //            Vector2 cameraSize = l.Camera.Viewport.Bounds.GetSize();
    //            Vector2 leftTop = l.Session.LevelData.Position;
    //            Vector2 ctCenter = p.CameraTarget + cameraSize / 2f;
    //            Vector2 ctCenterNonOffset = ctCenter - leftTop;
    //            Vector2 aligned = ctCenterNonOffset.AlignTo(cameraSize / 4f);
    //            l.Camera.Position = aligned + leftTop - cameraSize / 2f;
    //        });
    //    }

    //    static bool DoInsMatch(Instruction ins)
    //        => ins.MatchCallvirt<Camera>("set_Position") &&
    //        ins.Previous.MatchCall<Vector2>("op_Addition") &&
    //        ins.Previous.Previous.MatchCall<Vector2>("op_Multiply");
    //}

    //public static Vector2 AlignTo(this Vector2 vec, Vector2 alignVec)
    //{
    //    float xx = (float)Math.Floor(vec.X / alignVec.X) * alignVec.X;
    //    float yy = (float)Math.Floor(vec.Y / alignVec.Y) * alignVec.Y;
    //    return new(xx, yy);
    //}

    //public static Vector2 GetSize(this Rectangle rect)
    //{
    //    return new Vector2(rect.Width, rect.Height);
    //}
}
