using System.Reflection;

namespace Celeste.Mod.SaladimHelper.Entities;

[NeedModuleInit]
public partial class ReelCamera
{
    public static void Load()
    {
        On.Celeste.Player.Update += Player_Update;
        On.Celeste.Level.TransitionTo += Level_TransitionTo;
        On.Celeste.Level.Begin += Level_Begin;
    }

    public static void Unload()
    {
        On.Celeste.Player.Update -= Player_Update;
        On.Celeste.Level.TransitionTo -= Level_TransitionTo;
        On.Celeste.Level.Begin -= Level_Begin;
    }

    private static void Level_Begin(On.Celeste.Level.orig_Begin orig, Level self)
    {
        orig(self);
        ResetReel();
    }

    private static void Level_TransitionTo(On.Celeste.Level.orig_TransitionTo orig, Level self, LevelData next, Vector2 direction)
    {
        orig(self, next, direction);
        ResetReel();
    }

    protected static void ResetReel()
    {
        ActivedReelCamera = null;
        PlayerDoingReel = null;
    }

    protected static void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);
        Level level = self.SceneAs<Level>();
        var reelCamera = ActivedReelCamera;
        if (reelCamera != null)
        {
            var c = level.Camera;
            level.CameraOffset = reelCamera.CameraPosition - self.Position;
            level.Camera.Position = reelCamera.CameraPosition - new Vector2(c.Right - c.Left, c.Bottom - c.Top) / 2;
        }
    }
}
