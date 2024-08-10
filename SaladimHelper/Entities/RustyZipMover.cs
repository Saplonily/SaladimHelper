using System.Reflection;
using Celeste.Mod.Entities;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.SaladimHelper.Entities;

[SaladimModule, CustomEntity("SaladimHelper/RustyZipMover")]
public sealed class RustyZipMover : ZipMover
{
    public static ILHook hook;
    public bool Rusty = true;
    public float AreaRadio => Hitbox.Size.X * Hitbox.Size.Y / 1400f;

    public static ParticleType P_Motion;

    public RustyZipMover(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.NodesOffset(offset)[0], data.Bool("is_moon") ? Themes.Moon : Themes.Normal)
    { }

    public RustyZipMover(Vector2 position, int width, int height, Vector2 target, Themes theme)
        : base(position, width, height, target, theme)
    {
        OnDashCollide = (p, dir) =>
        {
            if (Rusty)
            {
                Rusty = false;
                SceneAs<Level>().ParticlesFG.Emit(P_Motion, (int)(200 * AreaRadio), BottomCenter, new Vector2(Width, 2) / 2f);
                return DashCollisionResults.Rebound;
            }
            return DashCollisionResults.NormalCollision;
        };
    }

    public override void Update()
    {
        base.Update();
    }

    //public override void Render()
    //{
    //    base.Render();

    //}

    private static void SequenceILHooked(ILContext il)
    {
        ILCursor cur = new(il);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchCallvirt("Celeste.Solid", "HasPlayerRider")))
        {
            cur.Emit(OpCodes.Ldloc_1);
            cur.EmitDelegate<Func<ZipMover, bool>>(z => z is not RustyZipMover rzm || !rzm.Rusty);
            cur.Emit(OpCodes.And);
        }
    }

    public static void Load()
    {
        hook = new(typeof(ZipMover).GetMethod("Sequence", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), SequenceILHooked);
    }

    public static void Unload()
    {
        hook.Dispose();
    }
}