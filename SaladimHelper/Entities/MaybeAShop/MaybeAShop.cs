using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity($"{ModuleName}/MaybeAShop")]
public class MaybeAShop : Entity
{
    private TalkComponent tc;

    public MaybeAShop(EntityData data, Vector2 offset)
        : this(data.Position + offset, new(data.Width, data.Height), data.Attr("config_path"))
    {

    }

    public MaybeAShop(Vector2 position, Vector2 size, string configPath)
    {
        Position = position;
        //Logger.Log(LogLevel.Info, ModuleName, $"config_path: {configPath}");
        tc = new(new Rectangle(0, 0, (int)size.X, (int)size.Y), new Vector2(size.X / 2, 0), OnTalk);
        Add(tc);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
    }

    private void OnTalk(Player p)
    {
        tc.Enabled = false;
        ShowUI(p);
        Input.Talk.ConsumePress();
    }

    private void ShowUI(Player p)
    {
        MaybeAShopUI ui = new(this, u =>
        {
            tc.Enabled = true;
            p.StateMachine.State = Player.StRedDash;
        });
        Scene.Add(ui);
        p.StateMachine.State = Player.StDummy;
    }
}
