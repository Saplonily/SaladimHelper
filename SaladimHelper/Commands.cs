namespace Celeste.Mod.SaladimHelper;

public static class Commands
{
    [Command("sal_set_coin", "set the coin amount for gddshop")]
    public static void SetCoin(int amount)
    {
        ModuleSession.CollectedCoinsAmount = amount;
    }
}