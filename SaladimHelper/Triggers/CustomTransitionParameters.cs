namespace Celeste.Mod.SaladimHelper.Triggers;

internal readonly struct CustomTransitionParameters
{
    public readonly Ease.Easer Easer;
    public readonly float CameraThreshold;
    public readonly float PlayerSpeed;
    public readonly float CameraDuration;

    public CustomTransitionParameters(Ease.Easer easer, float cameraThreshold, float playerSpeed, float cameraDuration)
    {
        Easer = easer;
        CameraThreshold = cameraThreshold;
        PlayerSpeed = playerSpeed;
        CameraDuration = cameraDuration;
    }
}
