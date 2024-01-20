using System;
using Celeste.Mod.Backdrops;

namespace Celeste.Mod.SaladimHelper.Backdrops;

// silly copying, but idk if there's a better way to do that

[CustomBackdrop("SaladimHelper/GravityWindSnowFG")]
public class GravityWindSnowFG : Backdrop
{
    public Vector2 CameraOffset = Vector2.Zero;
    public float Alpha = 1f;
    private Vector2[] positions;
    private SineWave[] sines;
    private Vector2 scale = Vector2.One;
    private float rotation = 0f;
    private float loopWidth = 640f;
    private float loopHeight = 360f;
    private float visibleFade = 1f;
    private float ySpeedMul = 1f;
    private float speedMul = 1f;

    public GravityWindSnowFG(BinaryPacker.Element data)
    {
        Color = Color.White;
        positions = new Vector2[240];
        for (int i = 0; i < positions.Length; i++)
            positions[i] = Calc.Random.Range(new Vector2(0f, 0f), new Vector2(loopWidth, loopHeight));

        sines = new SineWave[16];
        for (int j = 0; j < sines.Length; j++)
        {
            sines[j] = new SineWave(Calc.Random.Range(0.8f, 1.2f), 0f);
            sines[j].Randomize();
        }
        speedMul = data.AttrFloat("speedMul", 1f);
    }

    public override void Update(Scene scene)
    {
        base.Update(scene);
        Level level = scene as Level;

        visibleFade = Calc.Approach(visibleFade, IsVisible(scene as Level) ? 1f : 0f, Engine.DeltaTime * 2f);

        SineWave[] sineWaves = sines;
        for (int i = 0; i < sineWaves.Length; i++)
            sineWaves[i].Update();

        bool noYWind = level.Wind.Y == 0f;
        if (noYWind)
        {
            scale.X = Math.Max(1f, Math.Abs(level.Wind.X) / 100f);
            rotation = Calc.Approach(rotation, 0f, Engine.DeltaTime * 8f);
        }
        else
        {
            scale.X = Math.Max(1f, Math.Abs(level.Wind.Y) / 40f);
            rotation = Calc.Approach(rotation, -1.5707964f, Engine.DeltaTime * 8f);
        }
        scale.Y = 1f / Math.Max(1f, scale.X * 0.25f);

        if (ThirdPartyHelpers.ChronoHelperInstalled)
            ySpeedMul = Calc.Approach(ySpeedMul, QueryChronoGravityDirection() * speedMul, 5f * Engine.DeltaTime);
        for (int j = 0; j < positions.Length; j++)
        {
            float offset = sines[j % sines.Length].Value;
            Vector2 move = noYWind ?
                new Vector2(level.Wind.X + offset * 10f, 20f * ySpeedMul) :
                new Vector2(0f, (level.Wind.Y * 3f + offset * 10f) * ySpeedMul);
            positions[j] += move * Engine.DeltaTime;
        }
    }

    public float QueryChronoGravityDirection()
        => ChronoHelper.ChronoHelper.Session.gravityModeUp ? -1f : 1f;

    public override void Render(Scene scene)
    {
        if (Alpha <= 0f) return;

        Color color = Color * visibleFade * Alpha;
        Level level = scene as Level;
        bool noYWind = level.Wind.Y == 0f;
        int total = (int)(noYWind ? positions.Length : (positions.Length * 0.6f));
        int cur = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            position.Y -= level.Camera.Y + CameraOffset.Y;
            position.Y %= loopHeight;
            if (position.Y < 0f)
                position.Y += loopHeight;

            position.X -= level.Camera.X + CameraOffset.X;
            position.X %= loopWidth;
            if (position.X < 0f)
                position.X += loopWidth;
            if (cur < total)
                GFX.Game["particles/snow"].DrawCentered(position, color, scale, rotation);
            total++;
        }
    }
}
