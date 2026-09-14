namespace TarkovCheat.Core.Memory;

using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>Process memory read/write wrapper (simulation-safe).</summary>
public sealed class MemoryManager : IDisposable
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(int access, bool inherit, int pid);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(
        nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(
        nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpWritten);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(nint hObject);

    private const int ProcessAllAccess = 0x1F0FFF;

    private nint _handle;
    private readonly int _pid;
    private bool _disposed;
    private readonly Dictionary<string, nint> _moduleCache = new();

    public bool IsAttached => _handle != nint.Zero;
    public int ProcessId => _pid;

    public MemoryManager(int processId)
    {
        _pid = processId;
        _handle = OpenProcess(ProcessAllAccess, false, _pid);
        if (_handle == nint.Zero)
            throw new InvalidOperationException(
                $"Failed to open process {_pid}. Win32 error: {Marshal.GetLastWin32Error()}");
    }

    public T Read<T>(nint address) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>();
        byte[] buf = new byte[size];
        ReadProcessMemory(_handle, address, buf, size, out _);
        return MemoryMarshal.Read<T>(buf);
    }

    public byte[] ReadBytes(nint address, int count)
    {
        byte[] buf = new byte[count];
        ReadProcessMemory(_handle, address, buf, count, out int read);
        if (read != count)
            Array.Resize(ref buf, read);
        return buf;
    }

    public string ReadString(nint address, int maxLen = 256)
    {
        byte[] raw = ReadBytes(address, maxLen);
        int end = Array.IndexOf(raw, (byte)0);
        return System.Text.Encoding.UTF8.GetString(raw, 0, end < 0 ? raw.Length : end);
    }

    public bool Write<T>(nint address, T value) where T : unmanaged
    {
        int size = Marshal.SizeOf<T>();
        byte[] buf = new byte[size];
        MemoryMarshal.Write(buf.AsSpan(), in value);
        return WriteProcessMemory(_handle, address, buf, size, out _);
    }

    public bool WriteBytes(nint address, byte[] data)
    {
        return WriteProcessMemory(_handle, address, data, data.Length, out _);
    }

    public nint GetModuleBase(string moduleName)
    {
        if (_moduleCache.TryGetValue(moduleName, out nint cached))
            return cached;
        try
        {
            var proc = Process.GetProcessById(_pid);
            foreach (ProcessModule mod in proc.Modules)
            {
                if (string.Equals(mod.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    _moduleCache[moduleName] = mod.BaseAddress;
                    return mod.BaseAddress;
                }
            }
        }
        catch (Exception) { /* process may have exited */ }
        return nint.Zero;
    }

    public int GetModuleSize(string moduleName)
    {
        try
        {
            var proc = Process.GetProcessById(_pid);
            foreach (ProcessModule mod in proc.Modules)
            {
                if (string.Equals(mod.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
                    return mod.ModuleMemorySize;
            }
        }
        catch (Exception) { }
        return 0;
    }

    public void Dispose()
    {
        if (!_disposed && _handle != nint.Zero)
        {
            CloseHandle(_handle);
            _handle = nint.Zero;
            _disposed = true;
        }
    }
}
