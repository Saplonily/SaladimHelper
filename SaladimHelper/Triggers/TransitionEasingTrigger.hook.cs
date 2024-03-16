using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Reflection;

namespace Celeste.Mod.SaladimHelper.Triggers;

[NeedModuleInit]
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
        while (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdcR4(60f)))
        {
            cur.EmitDelegate(ModTransitionSpeed);
        }
    }

    private static void OnLevelTransitionRoutineILHook(ILContext ilc)
    {
        ILCursor cur = new(ilc);
        if (cur.TryGotoNext(MoveType.After, ins => ins.OpCode == OpCodes.Ldfld, ins => ins.MatchCallvirt<Player>("CleanUpTriggers")))
        {
            cur.Emit(OpCodes.Ldloc_1);
            cur.EmitDelegate(ModBeginTransition);
        }
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdfld<Level>("NextTransitionDuration")))
            cur.EmitDelegate(ModTransitionDuration);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchLdsfld("Monocle.Ease", "CubeOut")))
            cur.EmitDelegate(ModTransitionEaser);
        if (cur.TryGotoPrev(MoveType.After, ins => ins.MatchLdcR4(0.9f)))
            cur.EmitDelegate(ModTransitionThreshold);
        if (cur.TryGotoNext(MoveType.After, ins => ins.MatchStfld<Level>("transition")))
            cur.EmitDelegate(ModEndTransition);
    }

    private static void ModBeginTransition(Level level)
    {
        var player = level.Tracker.GetEntity<Player>();
        if (player is null) return;
        var trigger = player.CollideFirst<TransitionEasingTrigger>();
        if (trigger is null) return;
        ModuleSession.CustomTransition = (trigger.easing, trigger.threshold, trigger.speed, trigger.duration);
    }

    private static void ModEndTransition()
    {
        ModuleSession.CustomTransition = null;
    }

    private static float ModTransitionDuration(float duration)
    {
        var custom = ModuleSession.CustomTransition;
        return custom is null ? duration : custom.Value.duration;
    }

    private static Ease.Easer ModTransitionEaser(Ease.Easer easer)
    {
        var custom = ModuleSession.CustomTransition;
        return custom is null ? easer : custom.Value.easer;
    }

    private static float ModTransitionThreshold(float threshold)
    {
        var custom = ModuleSession.CustomTransition;
        return custom is null ? threshold : custom.Value.threshold;
    }

    private static float ModTransitionSpeed(float speed)
    {
        var custom = ModuleSession.CustomTransition;
        return custom is null ? speed : custom.Value.speed;
    }
}
