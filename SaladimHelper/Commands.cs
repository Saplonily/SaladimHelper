﻿namespace Celeste.Mod.SaladimHelper;

public static class Commands
{
    [Command("sal_set_coin", "Set the count of coins.")]
    public static void SetCoin(int count)
    {
        ModuleSession.CollectedCoinsCount = count;
    }

    [Command("sal_freeze", "Advance the timer until `level.TimeActive += Engine.DeltaTime` does nothing.")]
    public static void Freeze()
    {
        if (Engine.Scene is not Level level) return;
        while (true)
        {
            var p = level.TimeActive;
            level.TimeActive += Engine.DeltaTime;
            if (p == level.TimeActive)
                break;
        }
        long ticks = TimeSpan.FromSeconds((double)Engine.RawDeltaTime).Ticks;
        while (true)
        {
            var p = level.RawTimeActive;
            level.RawTimeActive += Engine.DeltaTime;
            level.Session.Time += ticks;
            if (p == level.RawTimeActive)
                break;
        }
    }
}