using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Reflection;

namespace Celeste.Mod.SaladimHelper.Triggers;

public partial class TransitionEasingTrigger
{
    private static ILHook levelTransitionRoutineILHook;

    public static void Load()
    {
        var method = typeof(Level).GetMethod("orig_TransitionRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
        method = method.GetStateMachineTarget();
        levelTransitionRoutineILHook = new(method, OnLevelTransitionRoutineILHook);
        IL.Celeste.Player.TransitionTo += Player_TransitionTo;
    }

    public static void Unload()
    {
        levelTransitionRoutineILHook.Dispose();
        IL.Celeste.Player.TransitionTo -= Player_TransitionTo;
    }

    private static void Player_TransitionTo(ILContext ilc)
    {
        ILCursor cur = new(ilc);
        cur.GotoNext(MoveType.After, ins => ins.MatchLdcR4(60f));
        cur.EmitDelegate(ModTransitionPlayerSpeed);
        cur.GotoNext(MoveType.After, ins => ins.MatchLdcR4(60f));
        cur.EmitDelegate(ModTransitionPlayerSpeed);
    }

    private static void OnLevelTransitionRoutineILHook(ILContext ilc)
    {
        ILCursor cur = new(ilc);
        cur.GotoNext(
            MoveType.After,
            ins => ins.OpCode == OpCodes.Ldfld,
            ins => ins.MatchCallvirt<Player>(nameof(Player.CleanUpTriggers))
        );
        cur.EmitLdloc1();
        cur.EmitDelegate(ModBeginTransition);

        cur.GotoNext(MoveType.After, ins => ins.MatchLdfld<Level>(nameof(Level.NextTransitionDuration)));
        cur.EmitDelegate(ModTransitionCameraDuration);

        cur.GotoNext(MoveType.After, ins => ins.MatchLdsfld("Monocle.Ease", nameof(Ease.CubeOut)));
        cur.EmitDelegate(ModTransitionEaser);

        cur.GotoPrev(MoveType.After, ins => ins.MatchLdcR4(0.9f));
        cur.EmitDelegate(ModTransitionCameraThreshold);

        cur.GotoNext(MoveType.After, ins => ins.MatchStfld<Level>(nameof(Level.transition)));
        cur.EmitDelegate(ModEndTransition);
    }

    private static void ModBeginTransition(Level level)
    {
        var player = level.Tracker.GetEntity<Player>();
        if (player is null) goto None;

        var trigger = player.CollideFirst<TransitionEasingTrigger>();
        if (trigger is null) goto None;

        SetParam(new(trigger.easing, trigger.threshold, trigger.speed, trigger.duration));
        return;
    None:
        SetParam(null);
    }

    private static void ModEndTransition() 
        => SetParam(null);

    private static float ModTransitionCameraDuration(float duration)
        => GetParam() is CustomTransitionParameters p ? p.CameraDuration : duration;

    private static Ease.Easer ModTransitionEaser(Ease.Easer easer)
        => GetParam() is CustomTransitionParameters p ? p.Easer : easer;

    private static float ModTransitionCameraThreshold(float cameraThreshold)
        => GetParam() is CustomTransitionParameters p ? p.CameraThreshold : cameraThreshold;

    private static float ModTransitionPlayerSpeed(float speed)
        => GetParam() is CustomTransitionParameters p ? p.PlayerSpeed : speed;

    // uh, ugly
    private static CustomTransitionParameters? GetParam()
        => ModuleSession.CustomTransition;

    private static void SetParam(CustomTransitionParameters? value)
        => ModuleSession.CustomTransition = value;
}
