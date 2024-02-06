using Celeste.Mod.Entities;

// modified and refactored from ChineseNewYear2024Helper by optimize-2
// https://github.com/optimize-2/ChineseNewYear2024Helper

namespace Celeste.Mod.SaladimHelper.Entities;

[CustomEntity("SaladimHelper/BitsMagicLanternController"), Tracked, NeedModuleInit]
public class BitsMagicLanternController : Entity
{
    public float DarkDuration;
    public float MaxDarkDuration;
    private MTexture lightTex;
    private bool gfxOnly;

    public BitsMagicLanternController(Vector2 position, float maxDarkDuration, bool gfxOnly)
        : base(position)
    {
        this.gfxOnly = gfxOnly;
        MaxDarkDuration = maxDarkDuration;
        lightTex = GFX.Game["SaladimHelper/entities/bitsMagicLantern/light"];
        Depth = Depths.FakeWalls;
        var tl = new TransitionListener();
        tl.OnOutBegin = RemoveSelf;
        Add(tl);
    }

    public BitsMagicLanternController(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Float("maxDarkDuration", 10f), data.Bool("gfxOnly"))
    {
    }

    public override void Update()
    {
        base.Update();
        if (gfxOnly)
            return;
        Player player = Scene.Tracker.GetEntity<Player>();

        if (player == null)
            return;

        bool inDark = true;
        foreach (BitsMagicLantern lantern in Scene.Tracker.GetEntities<BitsMagicLantern>().Cast<BitsMagicLantern>())
        {
            if (Vector2.DistanceSquared(lantern.Center, player.Center) <= lantern.Radius * lantern.Radius)
                inDark = false;
        }

        if (inDark)
            DarkDuration += Engine.DeltaTime;
        else
            DarkDuration = 0f;

        if (inDark && DarkDuration >= MaxDarkDuration && !player.Dead)
            player.Die(Vector2.Zero);
    }

    public override void Render()
    {
        base.Render();
        foreach (BitsMagicLantern lantern in Scene.Tracker.GetEntities<BitsMagicLantern>().Cast<BitsMagicLantern>())
        {
            float scale = lantern.Radius / 100f;
            lightTex.Draw(
                lantern.Center - new Vector2(lightTex.Width, lightTex.Height) * scale / 2f,
                Vector2.Zero,
                Color.White * 0.4f,
                new Vector2(scale)
                );
        }
    }

    public static void Load()
    {
        On.Celeste.Player.Render += Player_OnRender;
    }

    public static void Unload()
    {
        On.Celeste.Player.Render -= Player_OnRender;
    }

    private static void Player_OnRender(On.Celeste.Player.orig_Render orig, Player self)
    {
        var controller = self.Scene.Tracker.GetEntity<BitsMagicLanternController>();

        if (controller != null && controller.DarkDuration > 0)
        {
            float stamina = self.Stamina;
            self.Stamina = controller.DarkDuration + 2 > controller.MaxDarkDuration ? 0 : Player.ClimbMaxStamina;
            orig(self);
            self.Stamina = stamina;
        }
        else
        {
            orig(self);
        }
    }
}