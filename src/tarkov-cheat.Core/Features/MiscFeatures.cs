namespace TarkovCheat.Core.Features;

using System.Numerics;
using System.Runtime.InteropServices;
using TarkovCheat.Core.Memory;

/// <summary>Miscellaneous features: bhop, radar hack, no-flash, position prediction.</summary>
public sealed class MiscFeatures
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint Type;
        public KEYBDINPUT Ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public nuint ExtraInfo;
        private ulong _pad1;
    }

    private const int VkSpace = 0x20;
    private const int FlagOnGround = 1 << 0;
    private const uint InputKeyboard = 1;
    private const uint KeyEventKeyUp = 0x0002;

    public bool BhopEnabled { get; set; }
    public bool NoFlashEnabled { get; set; }
    public bool RadarEnabled { get; set; }
    public float MaxFlashAlpha { get; set; } = 0.0f;

    public void RunBhop(MemoryManager mem, nint localPlayer)
    {
        if (!BhopEnabled) return;
        bool spaceHeld = (GetAsyncKeyState(VkSpace) & 0x8000) != 0;
        if (!spaceHeld) return;

        int flags = mem.Read<int>(localPlayer + Offsets.Entity.Flags);
        bool onGround = (flags & FlagOnGround) != 0;
        if (onGround)
            SimulateKeyTap(VkSpace);
    }

    public void RunNoFlash(MemoryManager mem, nint localPlayer)
    {
        if (!NoFlashEnabled) return;
        float current = mem.Read<float>(localPlayer + Offsets.Entity.FlashDuration);
        if (current > MaxFlashAlpha)
            mem.Write(localPlayer + Offsets.Entity.FlashDuration, MaxFlashAlpha);
    }

    public void RunRadar(MemoryManager mem, IReadOnlyList<nint> entityAddresses)
    {
        if (!RadarEnabled) return;
        foreach (nint ent in entityAddresses)
        {
            if (ent == nint.Zero) continue;
            bool spotted = mem.Read<bool>(ent + Offsets.Entity.SpottedMask);
            if (!spotted)
                mem.Write(ent + Offsets.Entity.SpottedMask, true);
        }
    }

    public static Vector3 PredictPosition(Vector3 pos, Vector3 velocity, float latencyMs)
    {
        float t = latencyMs / 1000.0f;
        return new Vector3(
            pos.X + velocity.X * t,
            pos.Y + velocity.Y * t,
            pos.Z + velocity.Z * t);
    }

    public static float GetSpeed(Vector3 velocity)
    {
        return MathF.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
    }

    private static void SimulateKeyTap(int vk)
    {
        INPUT[] inputs =
        {
            new() { Type = InputKeyboard, Ki = new KEYBDINPUT { Vk = (ushort)vk } },
            new() { Type = InputKeyboard, Ki = new KEYBDINPUT { Vk = (ushort)vk, Flags = KeyEventKeyUp } },
        };
        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
    }
}
