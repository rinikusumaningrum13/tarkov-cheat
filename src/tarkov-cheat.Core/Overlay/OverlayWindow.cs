namespace TarkovCheat.Core.Overlay;

using System.Runtime.InteropServices;

/// <summary>Transparent overlay window — attaches on top of the game window.</summary>
public sealed class OverlayWindow : IDisposable
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(nint hWnd, out Rect lpRect);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(nint hWnd, out Rect lpRect);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;
        public readonly int Width => Right - Left;
        public readonly int Height => Bottom - Top;
    }

    private static readonly nint HwndTopmost = new(-1);
    private const uint SwpNoActivate = 0x0010;

    private nint _targetHwnd;
    private bool _disposed;

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool IsVisible { get; private set; }
    public bool StreamProof { get; set; }

    public bool AttachToWindow(string windowTitle)
    {
        _targetHwnd = FindWindow(null, windowTitle);
        if (_targetHwnd == nint.Zero) return false;
        UpdateBounds();
        IsVisible = true;
        return true;
    }

    public void UpdateBounds()
    {
        if (_targetHwnd == nint.Zero) return;
        if (GetWindowRect(_targetHwnd, out Rect r))
        {
            X = r.Left;
            Y = r.Top;
            Width = r.Width;
            Height = r.Height;
        }
    }

    public bool IsTargetFocused()
    {
        return GetForegroundWindow() == _targetHwnd;
    }

    public void BeginFrame()
    {
        UpdateBounds();
    }

    public void EndFrame()
    {
        // Flush drawing commands to the overlay render surface.
    }

    public void SetTopmost(nint overlayHwnd)
    {
        SetWindowPos(overlayHwnd, HwndTopmost, X, Y, Width, Height, SwpNoActivate);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            IsVisible = false;
            _targetHwnd = nint.Zero;
            _disposed = true;
        }
    }
}
