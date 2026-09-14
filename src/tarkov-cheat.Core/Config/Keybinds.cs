namespace TarkovCheat.Core.Config;

using System.Runtime.InteropServices;

/// <summary>Global hotkey registry with toggle-press detection.</summary>
public sealed class Keybinds
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static class Keys
    {
        public const int Insert  = 0x2D;
        public const int Delete  = 0x2E;
        public const int Home    = 0x24;
        public const int End     = 0x23;
        public const int F1      = 0x70;
        public const int F2      = 0x71;
        public const int F3      = 0x72;
        public const int F4      = 0x73;
        public const int F5      = 0x74;
        public const int F6      = 0x75;
        public const int Mouse4  = 0x05;
        public const int Mouse5  = 0x06;
    }

    private readonly Dictionary<string, int> _binds = new()
    {
        ["ToggleMenu"]   = Keys.Insert,
        ["AimAssist"]    = Keys.F1,
        ["Esp"]          = Keys.F2,
        ["TriggerBot"]   = Keys.F3,
        ["Bhop"]         = Keys.F4,
        ["AimKey"]       = Keys.Mouse5,
        ["ReloadConfig"] = Keys.Home,
        ["Panic"]        = Keys.End,
    };

    private readonly HashSet<int> _prevDown = new();

    public int GetBind(string action)
        => _binds.TryGetValue(action, out int vk) ? vk : 0;

    public void SetBind(string action, int vk) => _binds[action] = vk;

    public bool IsKeyDown(string action)
    {
        if (!_binds.TryGetValue(action, out int vk)) return false;
        return (GetAsyncKeyState(vk) & 0x8000) != 0;
    }

    public bool IsKeyPressed(string action)
    {
        if (!_binds.TryGetValue(action, out int vk)) return false;
        bool down = (GetAsyncKeyState(vk) & 0x8000) != 0;
        bool wasDown = _prevDown.Contains(vk);

        if (down && !wasDown)
        {
            _prevDown.Add(vk);
            return true;
        }
        if (!down && wasDown)
            _prevDown.Remove(vk);

        return false;
    }

    public IReadOnlyDictionary<string, int> AllBinds => _binds;
}
