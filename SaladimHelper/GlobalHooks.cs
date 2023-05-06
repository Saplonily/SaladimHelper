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
    public static ILHook Player_orig_Update_ILHook;
    public static void Load()
    {
        //Player_orig_Update_ILHook = new ILHook(typeof(Player).GetMethod("orig_Update"), Player_orig_Update);
        //On.Celeste.LavaRect.Quad_refInt32_Vector2_Color_Vector2_Color_Vector2_Color_Vector2_Color +=
        //    LavaRect_Quad_refInt32_Vector2_Color_Vector2_Color_Vector2_Color_Vector2_Color;
        //On.Celeste.LavaRect.ctor += LavaRect_ctor;
    }

    private static void LavaRect_ctor(On.Celeste.LavaRect.orig_ctor orig, LavaRect self, float width, float height, int step)
    {
        orig(self, width, height, 1);
    }

    private static void LavaRect_Quad_refInt32_Vector2_Color_Vector2_Color_Vector2_Color_Vector2_Color(
        On.Celeste.LavaRect.orig_Quad_refInt32_Vector2_Color_Vector2_Color_Vector2_Color_Vector2_Color orig, 
        LavaRect self, ref int vert, Vector2 va, Color ca, Vector2 vb, Color cb, Vector2 vc, Color cc, Vector2 vd, Color cd)
    {
        orig(self, ref vert, va, ca, vb, cb, vc, cc, vd, cd);
        Draw.SpriteBatch.Begin();
        Draw.Line(va, vc, Color.White);
        Draw.Line(vb, vd, Color.Red);
        Draw.Line(va, vb, Color.Blue);
        Draw.Line(vc, vd, Color.Green);
        Draw.SpriteBatch.End();
    }

    private static void Player_orig_Update(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, DoInsMatch))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate((Player p) =>
            {
                Level l = p.SceneAs<Level>();
                Vector2 cameraSize = l.Camera.Viewport.Bounds.GetSize();
                Vector2 leftTop = l.Session.LevelData.Position;
                Vector2 ctCenter = p.CameraTarget + cameraSize / 2f;
                Vector2 ctCenterNonOffset = ctCenter - leftTop;
                Vector2 aligned = ctCenterNonOffset.AlignTo(cameraSize / 4f);
                l.Camera.Position = aligned + leftTop - cameraSize / 2f;
            });
        }

        static bool DoInsMatch(Instruction ins)
            => ins.MatchCallvirt<Camera>("set_Position") &&
            ins.Previous.MatchCall<Vector2>("op_Addition") &&
            ins.Previous.Previous.MatchCall<Vector2>("op_Multiply");
    }

    public static void Unload()
    {
        //Player_orig_Update_ILHook.Dispose(); 
    }

    public static Vector2 AlignTo(this Vector2 vec, Vector2 alignVec)
    {
        float xx = (float)Math.Floor(vec.X / alignVec.X) * alignVec.X;
        float yy = (float)Math.Floor(vec.Y / alignVec.Y) * alignVec.Y;
        return new(xx, yy);
    }

    public static Vector2 GetSize(this Rectangle rect)
    {
        return new Vector2(rect.Width, rect.Height);
    }
}
