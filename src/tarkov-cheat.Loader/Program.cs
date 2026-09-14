namespace TarkovCheat.Loader;

using Microsoft.Extensions.Logging;
using TarkovCheat.Core.Memory;
using TarkovCheat.Core.Features;
using TarkovCheat.Core.Overlay;
using TarkovCheat.Core.Config;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(b =>
            b.SetMinimumLevel(LogLevel.Information).AddConsole());
        var log = loggerFactory.CreateLogger("Loader");

        log.LogInformation("waiting for EscapeFromTarkov.exe ...");
        int pid = ProcessFinder.WaitForProcess("EscapeFromTarkov", TimeSpan.FromSeconds(120));
        if (pid <= 0)
        {
            log.LogError("target process not found — exiting");
            return;
        }
        log.LogInformation("attached to pid {Pid}", pid);

        using var mem = new MemoryManager(pid);
        if (!mem.IsAttached)
        {
            log.LogError("failed to open process handle");
            return;
        }

        var profiles = new ProfileManager("tarkov-cheat");
        var settings = profiles.GetActive();
        var keys = new Keybinds();

        var aim = new AimAssist
        {
            Enabled = settings.Aim.Enabled,
            Fov = settings.Aim.Fov,
            Smooth = settings.Aim.Smooth,
            RecoilCompensation = settings.Aim.Rcs,
        };
        var esp = new EspRenderer
        {
            Enabled = settings.Esp.Enabled,
            DrawBox = settings.Esp.Box,
            DrawHealth = settings.Esp.Health,
            DrawSkeleton = settings.Esp.Skeleton,
        };
        var trigger = new TriggerBot
        {
            Enabled = settings.Trigger.Enabled,
            DelayMs = settings.Trigger.DelayMs,
            TeamCheck = settings.Trigger.TeamCheck,
        };
        var misc = new MiscFeatures
        {
            BhopEnabled = settings.Misc.Bhop,
            NoFlashEnabled = settings.Misc.NoFlash,
            RadarEnabled = settings.Misc.Radar,
        };

        using var overlay = new OverlayWindow();
        log.LogInformation("attaching overlay to 'EscapeFromTarkov'");
        if (!overlay.AttachToWindow("EscapeFromTarkov"))
            log.LogWarning("overlay attach failed — game window not found");

        log.LogInformation("running — press END to exit");

        while (true)
        {
            if (keys.IsKeyPressed("Panic"))
            {
                log.LogWarning("panic key — shutting down");
                break;
            }

            if (keys.IsKeyPressed("AimAssist"))
            {
                aim.Enabled = !aim.Enabled;
                log.LogInformation("aim assist: {State}", aim.Enabled ? "ON" : "OFF");
            }
            if (keys.IsKeyPressed("Esp"))
            {
                esp.Enabled = !esp.Enabled;
                log.LogInformation("esp: {State}", esp.Enabled ? "ON" : "OFF");
            }
            if (keys.IsKeyPressed("TriggerBot"))
            {
                trigger.Enabled = !trigger.Enabled;
                log.LogInformation("trigger: {State}", trigger.Enabled ? "ON" : "OFF");
            }
            if (keys.IsKeyPressed("Bhop"))
            {
                misc.BhopEnabled = !misc.BhopEnabled;
                log.LogInformation("bhop: {State}", misc.BhopEnabled ? "ON" : "OFF");
            }

            if (keys.IsKeyPressed("ReloadConfig"))
            {
                settings = profiles.GetActive();
                log.LogInformation("config reloaded from disk");
            }

            if (!ProcessFinder.IsRunning(pid))
            {
                log.LogWarning("target process exited");
                break;
            }

            await Task.Delay(1);
        }

        profiles.SaveActive(settings);
        log.LogInformation("clean exit");
    }
}
