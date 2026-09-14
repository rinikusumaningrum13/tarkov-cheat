namespace TarkovCheat.Core.Features;

using System.Numerics;
using TarkovCheat.Core.Memory;

/// <summary>Shared lightweight entity snapshot.</summary>
public record struct EntityData(
    int Index, int Health, int Team, bool IsAlive,
    Vector3 Position, Vector3 HeadPosition, Vector2 ScreenPosition);

/// <summary>Aim assist with FOV filtering, smoothing and bone targeting.</summary>
public sealed class AimAssist
{
    public bool Enabled { get; set; }
    public float Fov { get; set; } = 5.0f;
    public float Smooth { get; set; } = 3.5f;
    public int TargetBone { get; set; } = Offsets.Bones.Head;
    public bool RecoilCompensation { get; set; } = true;
    public bool VisibleOnly { get; set; } = true;

    private readonly Random _rng = new();

    public Vector2 CalculateAngle(Vector3 source, Vector3 target)
    {
        Vector3 delta = target - source;
        float hyp = MathF.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
        float pitch = -MathF.Atan2(delta.Z, hyp) * (180.0f / MathF.PI);
        float yaw = MathF.Atan2(delta.Y, delta.X) * (180.0f / MathF.PI);
        return new Vector2(pitch, yaw);
    }

    public bool IsInFov(Vector2 screenCenter, Vector2 targetScreen, float fovRadius)
    {
        float dx = targetScreen.X - screenCenter.X;
        float dy = targetScreen.Y - screenCenter.Y;
        return MathF.Sqrt(dx * dx + dy * dy) <= fovRadius;
    }

    public Vector2 SmoothAngle(Vector2 current, Vector2 target, float smoothFactor)
    {
        if (smoothFactor <= 1.0f) return target;
        Vector2 delta = NormalizeAngles(target - current);
        float jitter = 0.85f + (float)_rng.NextDouble() * 0.30f;
        return current + delta / (smoothFactor * jitter);
    }

    public int SelectBestTarget(
        IReadOnlyList<EntityData> entities, Vector2 screenCenter,
        int localTeam, float fovRadius)
    {
        int bestIdx = -1;
        float bestDist = float.MaxValue;

        for (int i = 0; i < entities.Count; i++)
        {
            EntityData e = entities[i];
            if (!e.IsAlive || e.Health <= 0 || e.Team == localTeam)
                continue;

            float dx = e.ScreenPosition.X - screenCenter.X;
            float dy = e.ScreenPosition.Y - screenCenter.Y;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist < fovRadius && dist < bestDist)
            {
                bestDist = dist;
                bestIdx = i;
            }
        }
        return bestIdx;
    }

    public Vector2 CompensateRecoil(Vector2 angle, Vector2 punchAngle)
    {
        if (!RecoilCompensation) return angle;
        return new Vector2(
            angle.X - punchAngle.X * 2.0f,
            angle.Y - punchAngle.Y * 2.0f);
    }

    public static float GetFovDistance(Vector2 screenCenter, Vector2 target)
    {
        float dx = target.X - screenCenter.X;
        float dy = target.Y - screenCenter.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    private static Vector2 NormalizeAngles(Vector2 angles)
    {
        float pitch = angles.X;
        float yaw = angles.Y;
        while (yaw > 180.0f) yaw -= 360.0f;
        while (yaw < -180.0f) yaw += 360.0f;
        pitch = Math.Clamp(pitch, -89.0f, 89.0f);
        return new Vector2(pitch, yaw);
    }
}
