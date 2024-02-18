namespace Celeste.Mod.SaladimHelper;

[MonoMod.ModInterop.ModExportName("SaladimHelper.Mapper")]
public static class Mapper
{
    public static Ease.Easer GetEaser(string name) => name switch
    {
        "Linear" => Ease.Linear,
        "SineIn" => Ease.SineIn,
        "SineOut" => Ease.SineOut,
        "SineInOut" => Ease.SineInOut,
        "QuadIn" => Ease.QuadIn,
        "QuadOut" => Ease.QuadOut,
        "QuadInOut" => Ease.QuadInOut,
        "CubeIn" => Ease.CubeIn,
        "CubeOut" => Ease.CubeOut,
        "CubeInOut" => Ease.CubeInOut,
        "QuintIn" => Ease.QuintIn,
        "QuintOut" => Ease.QuintOut,
        "QuintInOut" => Ease.QuintInOut,
        "ExpoIn" => Ease.ExpoIn,
        "ExpoOut" => Ease.ExpoOut,
        "ExpoInOut" => Ease.ExpoInOut,
        "BackIn" => Ease.BackIn,
        "BackOut" => Ease.BackOut,
        "BackInOut" => Ease.BackInOut,
        "BigBackIn" => Ease.BigBackIn,
        "BigBackOut" => Ease.BigBackOut,
        "BigBackInOut" => Ease.BigBackInOut,
        "ElasticIn" => Ease.ElasticIn,
        "ElasticOut" => Ease.ElasticOut,
        "ElasticInOut" => Ease.ElasticInOut,
        "BounceIn" => Ease.BounceIn,
        "BounceOut" => Ease.BounceOut,
        "BounceInOut" => Ease.BounceInOut,
        _ => Ease.Linear
    };

    public static Tween.TweenMode GetTweenMode(string name) => name switch
    {
        "Persist" => Tween.TweenMode.Persist,
        "Oneshot" => Tween.TweenMode.Oneshot,
        "Looping" => Tween.TweenMode.Looping,
        "YoyoOneshot" => Tween.TweenMode.YoyoOneshot,
        "YoyoLooping" => Tween.TweenMode.YoyoLooping,
        _ => Tween.TweenMode.Oneshot
    };
}
