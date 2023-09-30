using Celeste.Mod.Entities;

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity($"{ModuleName}/MaybeAShop")]
public partial class MaybeAShop : Entity
{
    public readonly EntityID ID;
    private int[] costs;
    private Vector2[] nodes;
    private string[] itemTexs;
    private List<Entity>[] shopEntities;
    private TalkComponent tc;
    private int lineMax;

    public MaybeAShop(EntityData data, Vector2 offset)
        : this(data.Position + offset,
              new(data.Width, data.Height),
              new EntityID(data.Level.Name, data.ID),
              data.Int("line_max", 4),
              data.Attr("tex_sequence").Split(','),
              ParseSequence(data.Attr("cost_sequence"), data.Nodes.Length),
              data.NodesOffset(offset))
    {

    }

    public MaybeAShop(Vector2 position, Vector2 size, EntityID id, int lineMax, string[] itemTexs, int[] costs, Vector2[] nodes)
    {
        Position = position;
        shopEntities = new List<Entity>[nodes.Length];
        ID = id;
        this.costs = costs;
        this.itemTexs = itemTexs;
        this.nodes = nodes;
        this.lineMax = lineMax;
        if (itemTexs.Length != costs.Length)
            throw new Exception("itemTexs.Length != costs.Length");
        if (costs.Length != nodes.Length)
            throw new Exception("costs.Length != nodes.Length");
        if (itemTexs.Length != nodes.Length)
            throw new Exception("itemTexs.Length != nodes.Length");

        Rectangle box = new Rectangle(0, 0, (int)size.X, (int)size.Y);
        Collider = new Hitbox(box.Width, box.Height, box.X, box.Y);
        tc = new(box, new Vector2(size.X / 2, 0), OnTalk);
        tc.PlayerMustBeFacing = false;
        Add(tc);
    }

    public override void Update()
    {
        base.Update();


        var displayer = ModuleSession.CurrentCoinDisplayer;
        if (displayer is not null && displayer.Scene != Scene)
            displayer = null;
        var player = Scene.Tracker.GetEntity<Player>();
        if (player is not null && player.CollideCheck(this))
        {
            displayer ??= CoinDisplayer.Display(Scene, true);
            displayer.AlwaysDisplay = true;
        }
        else if (displayer is not null)
        {
            displayer.AlwaysDisplay = false;
        }
    }

    public static int[] ParseSequence(string sequence, int expectedLength)
    {
        try
        {
            var result = sequence.Split(',').Select(int.Parse).ToArray();
            if (result.Length != expectedLength)
                throw new ArgumentException($"Expected {expectedLength} parts but got {result.Length}.");
            return result;
        }
        catch (Exception e)
        {
            throw new Exception("Sequence parsing failed.", e);
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        // ok let's do colliding
        for (int i = 0; i < nodes.Length; i++)
        {
            shopEntities[i] = new(24);
            var pos = nodes[i];

            var allEntities = Scene.Entities;
            foreach (var e in allEntities)
            {
                if (Collide.CheckPoint(e, pos))
                    shopEntities[i].Add(e);
            }
        }

        for (int i = 0; i < shopEntities.Length; i++)
        {
            if (!ModuleSession.ShopBoughtItems.Contains(i))
                foreach (var entity in shopEntities[i])
                    entity.RemoveSelf();
        }
    }

    public override void Render()
    {
        base.Render();
        foreach (var node in nodes)
        {
            Draw.Point(node, Color.Red);
        }
    }

    private void OnTalk(Player p)
    {
        tc.Enabled = false;
        ShowUI(p);
        Input.Talk.ConsumePress();
    }

    private void ShowUI(Player p)
    {
        MaybeAShopUI ui = new(OnBought, OnFinish, itemTexs, costs, lineMax);
        Scene.Add(ui);
        p.StateMachine.State = Player.StDummy;
        void OnFinish(MaybeAShopUI ui)
        {
            tc.Enabled = true;
            p.StateMachine.State = Player.StNormal;
        }
        void OnBought(MaybeAShopUI ui, int index)
        {
            ModuleSession.ShopBoughtItems.Add(index);
            CoinDisplayer.Display(Scene, setAmountAgain: true);
            foreach (var item in shopEntities[index])
            {
                var from = Center;
                var to = item.Center;
                ShopBoughtLeader leader = new(from, to, item);
                Scene.Add(leader);
            }
        }
    }
}
