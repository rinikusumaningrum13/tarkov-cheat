namespace TarkovCheat.Core.Features;

using System.Numerics;
using TarkovCheat.Core.Overlay;

/// <summary>Bounding-box result for a projected entity.</summary>
public record struct EspBox(float X, float Y, float Width, float Height);

/// <summary>ESP renderer — world-to-screen projection, box, health, skeleton.</summary>
public sealed class EspRenderer
{
    public bool Enabled { get; set; } = true;
    public bool DrawBox { get; set; } = true;
    public bool DrawHealth { get; set; } = true;
    public bool DrawSkeleton { get; set; }
    public bool DrawName { get; set; } = true;
    public bool DrawDistance { get; set; } = true;
    public bool DrawSnaplines { get; set; }
    public bool DrawArmor { get; set; }

    public Vector2? WorldToScreen(Vector3 world, float[] vm, int screenW, int screenH)
    {
        float w = vm[12] * world.X + vm[13] * world.Y + vm[14] * world.Z + vm[15];
        if (w < 0.001f) return null;

        float x = vm[0] * world.X + vm[1] * world.Y + vm[2] * world.Z + vm[3];
        float y = vm[4] * world.X + vm[5] * world.Y + vm[6] * world.Z + vm[7];

        float ndcX = x / w;
        float ndcY = y / w;

        float screenX = screenW * 0.5f + ndcX * screenW * 0.5f;
        float screenY = screenH * 0.5f - ndcY * screenH * 0.5f;

        return new Vector2(screenX, screenY);
    }

    public EspBox? CalculateBox(
        Vector3 headPos, Vector3 feetPos, float[] viewMatrix, int sw, int sh)
    {
        Vector2? headScreen = WorldToScreen(headPos, viewMatrix, sw, sh);
        Vector2? feetScreen = WorldToScreen(feetPos, viewMatrix, sw, sh);
        if (headScreen is null || feetScreen is null) return null;

        float height = MathF.Abs(feetScreen.Value.Y - headScreen.Value.Y);
        float width = height * 0.45f;
        float bx = headScreen.Value.X - width * 0.5f;
        float by = headScreen.Value.Y;

        return new EspBox(bx, by, width, height);
    }

    public void RenderEntity(
        DrawingHelper gfx, EntityData entity,
        float[] viewMatrix, int sw, int sh, Vector3 localPos)
    {
        if (!Enabled) return;
        EspBox? box = CalculateBox(entity.HeadPosition, entity.Position, viewMatrix, sw, sh);
        if (box is null) return;
        EspBox b = box.Value;

        if (DrawBox)
        {
            gfx.DrawRect(b.X - 1, b.Y - 1, b.Width + 2, b.Height + 2, 0xFF000000);
            uint teamColor = entity.Team == 2 ? 0xFFFF4444u : 0xFF4444FFu;
            gfx.DrawRect(b.X, b.Y, b.Width, b.Height, teamColor);
        }

        if (DrawHealth)
        {
            float pct = Math.Clamp(entity.Health / 100.0f, 0f, 1f);
            gfx.DrawHealthBar(b.X, b.Y, b.Height, pct);
        }

        if (DrawName)
            gfx.DrawText($"[{entity.Index}]", b.X, b.Y - 14, 0xFFCCCCCC);

        if (DrawDistance)
        {
            float dist = Vector3.Distance(localPos, entity.Position);
            Vector2? feet = WorldToScreen(entity.Position, viewMatrix, sw, sh);
            if (feet is not null)
                gfx.DrawText($"{dist:F0}m", feet.Value.X, feet.Value.Y + 4, 0xFFFFFFFF);
        }

        if (DrawSnaplines)
        {
            float cx = sw * 0.5f;
            float cy = (float)sh;
            gfx.DrawLine(cx, cy, b.X + b.Width * 0.5f, b.Y + b.Height, 0xFFFF0000);
        }
    }

    public void RenderFovCircle(DrawingHelper gfx, int sw, int sh, float fovPx)
    {
        float cx = sw * 0.5f;
        float cy = sh * 0.5f;
        gfx.DrawCircle(cx, cy, fovPx, 0x60FFFFFF);
    }
}
