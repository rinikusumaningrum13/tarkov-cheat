namespace TarkovCheat.Loader;

using System.Diagnostics;

/// <summary>Utility for locating and monitoring the game process.</summary>
public static class ProcessFinder
{
    public static int FindProcess(string name)
    {
        var procs = Process.GetProcessesByName(name);
        return procs.Length > 0 ? procs[0].Id : -1;
    }

    public static int WaitForProcess(string name, TimeSpan timeout)
    {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            int pid = FindProcess(name);
            if (pid > 0) return pid;
            Thread.Sleep(500);
        }
        return -1;
    }

    public static bool IsRunning(int pid)
    {
        try
        {
            var proc = Process.GetProcessById(pid);
            return !proc.HasExited;
        }
        catch (ArgumentException) { return false; }
    }

    public static nint GetMainWindowHandle(string name)
    {
        var procs = Process.GetProcessesByName(name);
        return procs.Length > 0 ? procs[0].MainWindowHandle : nint.Zero;
    }

    public static string? GetMainModulePath(string name)
    {
        var procs = Process.GetProcessesByName(name);
        if (procs.Length == 0) return null;
        try { return procs[0].MainModule?.FileName; }
        catch (Exception) { return null; }
    }

    public static int[] FindAllInstances(string name)
    {
        return Process.GetProcessesByName(name).Select(p => p.Id).ToArray();
    }
}
