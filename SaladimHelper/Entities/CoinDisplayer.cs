namespace Celeste.Mod.SaladimHelper.Entities;

public class CoinDisplayer : Entity
{
    public string Group;

    public CoinDisplayer()
    {
        Tag |= Tags.HUD | Tags.Persistent | Tags.TransitionUpdate | Tags.PauseUpdate;

    }

    public override void Update()
    {
        base.Update();
        if (Input.CrouchDash.Pressed)
        {
            Input.CrouchDash.ConsumeBuffer();
            Logger.Log(LogLevel.Info, ModuleName, "---------");
            foreach (var item in Scene)
            {
                if (item.TagCheck(Tags.HUD))
                {
                    Logger.Log(LogLevel.Info, ModuleName, $"{item.GetType().FullName}");
                }
            }
            Logger.Log(LogLevel.Info, ModuleName, "---------");
        }
    }

    public override void Render()
    {
        base.Render();
        ActiveFont.Draw($"> {ModuleSession.GroupToCoinsMap[Group]}", Position + Vector2.UnitY * 120f, Color.White);
    }
}