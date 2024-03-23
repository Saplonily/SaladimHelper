using Mono.Cecil.Cil;

namespace Celeste.Mod.SaladimHelper.Entities;

[NeedModuleInit]
public partial class ExplodeFocusArea
{
    public static void Load()
    {
        IL.Celeste.Player.ExplodeLaunch_Vector2_bool_bool += Player_ExplodeLaunch;
    }

    public static void Unload()
    {
        IL.Celeste.Player.ExplodeLaunch_Vector2_bool_bool -= Player_ExplodeLaunch;
    }

    private static void Player_ExplodeLaunch(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchCall("Monocle.Calc", "SafeNormalize")))
        {
            cur.Emit(OpCodes.Ldarg_0);
            cur.EmitDelegate(ModDirection);
        }
    }

    public static Vector2 ModDirection(Vector2 direction, Player player)
    {
        var area = player.CollideFirst<ExplodeFocusArea>();
        if (area is not null)
        {
            direction = direction.Rotate(area.rotation);
            switch (area.focusType)
            {
            case FocusType.EightWay: direction = direction.EightWayNormal(); break;
            case FocusType.FourWay: direction = direction.FourWayNormal(); break;
            case FocusType.DiagonalFourWay:
                direction = direction.Rotate(MathHelper.PiOver4);
                direction = direction.FourWayNormal();
                direction = direction.Rotate(-MathHelper.PiOver4);
                break;
            case FocusType.HorizontalTwoWay:
                direction.Y = 0;
                direction = direction.SafeNormalize();
                break;
            case FocusType.VerticalTwoWay:
                direction.X = 0;
                direction = direction.SafeNormalize();
                break;
            }
            direction = direction.Rotate(-area.rotation);
        }
        return direction;
    }
}
