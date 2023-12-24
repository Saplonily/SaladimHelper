using System.Reflection;

namespace Celeste.Mod.SaladimHelper;

public static class StateMachineExt
{
    public static int AddState(this StateMachine machine,
        Func<int> onUpdate,
        Func<IEnumerator> coroutine = null,
        Action begin = null,
        Action end = null
        )
    {
        var begins = (Action[])StateMachine_begins.GetValue(machine);
        var updates = (Func<int>[])StateMachine_updates.GetValue(machine);
        var ends = (Action[])StateMachine_ends.GetValue(machine);
        var coroutines = (Func<IEnumerator>[])StateMachine_coroutines.GetValue(machine);
        int statesCounts = begins.Length;

        Array.Resize(ref begins, begins.Length + 1);
        Array.Resize(ref updates, updates.Length + 1);
        Array.Resize(ref ends, ends.Length + 1);
        Array.Resize(ref coroutines, coroutines.Length + 1);

        StateMachine_begins.SetValue(machine, begins);
        StateMachine_updates.SetValue(machine, updates);
        StateMachine_ends.SetValue(machine, ends);
        StateMachine_coroutines.SetValue(machine, coroutines);
        machine.SetCallbacks(statesCounts, onUpdate, coroutine, begin, end);
        return statesCounts;
    }

    public static readonly FieldInfo StateMachine_begins
        = typeof(StateMachine).GetField("begins", BindingFlags.Instance | BindingFlags.NonPublic);
    public static readonly FieldInfo StateMachine_updates
        = typeof(StateMachine).GetField("updates", BindingFlags.Instance | BindingFlags.NonPublic);
    public static readonly FieldInfo StateMachine_ends
        = typeof(StateMachine).GetField("ends", BindingFlags.Instance | BindingFlags.NonPublic);
    public static readonly FieldInfo StateMachine_coroutines
        = typeof(StateMachine).GetField("coroutines", BindingFlags.Instance | BindingFlags.NonPublic);
}