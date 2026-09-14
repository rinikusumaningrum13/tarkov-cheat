namespace TarkovCheat.Core.Overlay;

/// <summary>Buffered drawing primitive collector.</summary>
public sealed class DrawingHelper
{
    private readonly List<DrawCommand> _commands = new();

    public void DrawLine(float x1, float y1, float x2, float y2, uint color, float thickness = 1f)
    {
        _commands.Add(new DrawCommand(DrawType.Line, x1, y1, x2 - x1, y2 - y1, color, thickness, null));
    }

    public void DrawRect(float x, float y, float w, float h, uint color, float thickness = 1f)
    {
        _commands.Add(new DrawCommand(DrawType.Rect, x, y, w, h, color, thickness, null));
    }

    public void DrawFilledRect(float x, float y, float w, float h, uint color)
    {
        _commands.Add(new DrawCommand(DrawType.FilledRect, x, y, w, h, color, 0, null));
    }

    public void DrawCircle(float cx, float cy, float radius, uint color, float thickness = 1f)
    {
        _commands.Add(new DrawCommand(DrawType.Circle, cx, cy, radius, 0, color, thickness, null));
    }

    public void DrawText(string text, float x, float y, uint color)
    {
        _commands.Add(new DrawCommand(DrawType.Text, x, y, 0, 0, color, 0, text));
    }

    public void DrawCornerBox(float x, float y, float w, float h, float cornerLen, uint color)
    {
        float cl = MathF.Min(cornerLen, MathF.Min(w, h) * 0.4f);
        DrawLine(x, y, x + cl, y, color);
        DrawLine(x, y, x, y + cl, color);
        DrawLine(x + w, y, x + w - cl, y, color);
        DrawLine(x + w, y, x + w, y + cl, color);
        DrawLine(x, y + h, x + cl, y + h, color);
        DrawLine(x, y + h, x, y + h - cl, color);
        DrawLine(x + w, y + h, x + w - cl, y + h, color);
        DrawLine(x + w, y + h, x + w, y + h - cl, color);
    }

    public void DrawHealthBar(float x, float y, float h, float percent, float barWidth = 3f)
    {
        DrawFilledRect(x - barWidth - 2, y, barWidth + 2, h, 0x80000000);
        float fillH = h * Math.Clamp(percent, 0f, 1f);
        uint color = percent > 0.5f ? 0xFF00FF00 : percent > 0.25f ? 0xFFFFFF00 : 0xFFFF0000;
        DrawFilledRect(x - barWidth - 1, y + h - fillH, barWidth, fillH, color);
    }

    public IReadOnlyList<DrawCommand> Flush()
    {
        var snapshot = _commands.ToList();
        _commands.Clear();
        return snapshot;
    }

    public int PendingCount => _commands.Count;

    public static uint Rgba(byte r, byte g, byte b, byte a = 255)
        => (uint)(a << 24 | r << 16 | g << 8 | b);
}

public enum DrawType { Line, Rect, FilledRect, Circle, Text }

public record struct DrawCommand(
    DrawType Type, float X, float Y, float W, float H,
    uint Color, float Thickness, string? Text);
