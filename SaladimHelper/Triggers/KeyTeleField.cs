using Celeste.Mod.Entities;
using MonoMod.Utils;
using System.Reflection;

namespace Celeste.Mod.SaladimHelper.Triggers;

[CustomEntity("SaladimHelper/KeyTeleField"), Tracked]
public class KeyTeleField : Trigger
{
    public const float TeleportCorrectValue = 4f;

    public Vector2 Vector { get; set; }
    public bool CrossRoom { get; set; }
    public string TargetRoomId { get; set; }
    public string AudioToPlay { get; set; }
    public bool KillPlayerOnCollideWall { get; set; }
    public bool Absolute { get; set; }

    public KeyTeleField(EntityData data, Vector2 offset) : base(data, offset)
    {
        float f1 = data.Float("vector_x", 0);
        float f2 = data.Float("vector_y", 0);
        Vector = new(f1, f2);
        Absolute = data.Bool("absolute", false);
        CrossRoom = data.Bool("cross_room", false);
        TargetRoomId = data.Attr("target_room_id", null);
        AudioToPlay = data.Attr("audio_to_play", null);
        KillPlayerOnCollideWall = !data.Bool("kill_player_on_collide_wall", true);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        var key = SaladimHelperModule.Settings.DoTeleport;
        if (key.Pressed)
        {
            key.ConsumePress();
            if (CrossRoom)
                TeleportHelper.DoCrossRoomTeleport(player, TargetRoomId, Vector, Absolute, AudioToPlay);
            else
                TeleportHelper.DoNormalTeleport(player, Vector, Absolute, AudioToPlay);
            if (TeleportHelper.MakeCorrectCheck(player) is false)
            {
                player.Die(Vector2.Zero);
            }
        }
    }
}

public static class TeleportHelper
{
    public static MethodInfo PlayerDashCorrectCheck = typeof(Player).GetMethod("DashCorrectCheck", BindingFlags.NonPublic);

    public static void DoCrossRoomTeleport(Player player, string targetRoomId, Vector2 vector, bool absolute, string audioToPlay)
    {
        Level level = Engine.Scene as Level;
        LevelData levelData = level.Session.LevelData;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.1f, true);

        tween.OnUpdate = t => Glitch.Value = 0.5f * t.Eased;

        tween.OnComplete = _ =>
            level.OnEndOfFrame += () =>
            {
                Vector2 prePlayerPosition = player.Position;
                player.Position -= levelData.Position;
                level.Camera.Position -= levelData.Position;
                level.Session.Level = targetRoomId;
                player.CleanUpTriggers();

                var dashes = player.Dashes;
                var facing = player.Facing;
                var leader = player.Get<Leader>();
                foreach (var follower in leader.Followers)
                {
                    if (follower.Entity is null) continue;

                    follower.Entity.AddTag(Tags.Global);
                    level.Session.DoNotLoad.Add(follower.ParentEntityID);
                }

                level.Remove(player);
                level.UnloadLevel();
                level.Add(player);
                level.LoadLevel(Player.IntroTypes.Transition, false);

                if (!string.IsNullOrEmpty(audioToPlay))
                    Audio.Play(audioToPlay);

                levelData = level.Session.LevelData;

                if (absolute)
                    player.Position = vector + levelData.Position;
                else
                    player.Position += levelData.Position + vector;

                player.Facing = facing;
                level.Camera.Position += levelData.Position;
                level.Session.RespawnPoint = new Vector2?(level.Session.LevelData.Spawns.ClosestTo(player.Position));
                player.Dashes = dashes;

                Vector2 vector2 = player.Position - prePlayerPosition;
                foreach (Follower follower in leader.Followers)
                {
                    if (follower.Entity != null)
                    {
                        follower.Entity.Position += vector2;
                        follower.Entity.RemoveTag(Tags.Global);
                        level.Session.DoNotLoad.Remove(follower.ParentEntityID);
                    }
                }
                for (int i = 0; i < leader.PastPoints.Count; i++)
                {
                    List<Vector2> pastPoints = leader.PastPoints;
                    int num = i;
                    pastPoints[num] += vector2;
                }
                leader.TransferFollowers();
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.05f, true);
                tween.OnUpdate = t =>
                {
                    Glitch.Value = 0.5f * (1f - t.Eased);
                };
                player.Add(tween);
            };

        player.Add(tween);
    }

    public static void DoNormalTeleport(Player player, Vector2 vector, bool absolute, string audioToPlay)
    {
        player.Position = absolute ? player.SceneAs<Level>().Session.LevelData.Position + vector : player.Position + vector;
        if (!string.IsNullOrEmpty(audioToPlay))
        {
            Audio.Play(audioToPlay);
        }
    }

    // true: 校正完成且玩家没死
    // false: 校正完成但玩家还是死了
    // null: 没校正也没死
    public static bool? MakeCorrectCheck(Player player)
    {
        const float correctValue = KeyTeleField.TeleportCorrectValue;
        Solid solid = player.CollideFirst<Solid>();

        if (solid is not null)
        {
            player.Y -= correctValue;
            if (!player.CollideCheck<Solid>())
                return true;

            player.Y += correctValue * 2;
            if (!player.CollideCheck<Solid>())
                return true;

            player.Y -= correctValue;
            player.X += correctValue;
            if (!player.CollideCheck<Solid>())
                return true;

            player.X -= correctValue * 2;
            if (!player.CollideCheck<Solid>())
                return true;

            player.X += correctValue;

            return false;
        }
        else
        {
            return null;
        }
    }
}