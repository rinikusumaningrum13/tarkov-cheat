namespace TarkovCheat.Core.Features;

using System.Runtime.InteropServices;

/// <summary>TriggerBot — fires when crosshair is over enemy.</summary>
public sealed class TriggerBot
{
    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, nuint dwExtraInfo);

    private const uint MouseLeftDown = 0x0002;
    private const uint MouseLeftUp   = 0x0004;

    public bool Enabled { get; set; }
    public int DelayMs { get; set; } = 50;
    public int DelayJitter { get; set; } = 15;
    public int BurstCount { get; set; } = 1;
    public int BurstDelayMs { get; set; } = 40;
    public bool TeamCheck { get; set; } = true;

    private readonly Random _rng = new();
    private DateTime _lastShot = DateTime.MinValue;
    private bool _triggerHeld;

    public bool ShouldFire(int crosshairEntityId, int localTeam, Func<int, int> getTeam)
    {
        if (!Enabled || crosshairEntityId <= 0)
            return false;
        if (TeamCheck && getTeam(crosshairEntityId) == localTeam)
            return false;

        TimeSpan elapsed = DateTime.UtcNow - _lastShot;
        int delay = DelayMs + _rng.Next(-DelayJitter, DelayJitter + 1);
        return elapsed.TotalMilliseconds >= Math.Max(delay, 1);
    }

    public async Task FireAsync(CancellationToken ct = default)
    {
        for (int i = 0; i < BurstCount && !ct.IsCancellationRequested; i++)
        {
            mouse_event(MouseLeftDown, 0, 0, 0, 0);
            _triggerHeld = true;

            await Task.Delay(10 + _rng.Next(8), ct);

            mouse_event(MouseLeftUp, 0, 0, 0, 0);
            _triggerHeld = false;
            _lastShot = DateTime.UtcNow;

            if (i < BurstCount - 1)
                await Task.Delay(BurstDelayMs + _rng.Next(10), ct);
        }
    }

    public void Reset()
    {
        if (_triggerHeld)
        {
            mouse_event(MouseLeftUp, 0, 0, 0, 0);
            _triggerHeld = false;
        }
        _lastShot = DateTime.MinValue;
    }

    public int GetEffectiveDelay()
    {
        return Math.Max(1, DelayMs + _rng.Next(-DelayJitter, DelayJitter + 1));
    }
}
