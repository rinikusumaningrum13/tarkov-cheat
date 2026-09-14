namespace TarkovCheat.Core.Config;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>Root settings model — serialisable to JSON profile files.</summary>
public sealed class CheatSettings
{
    public AimSettings Aim { get; set; } = new();
    public EspSettings Esp { get; set; } = new();
    public TriggerSettings Trigger { get; set; } = new();
    public MiscSettings Misc { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    public string ToJson() => JsonSerializer.Serialize(this, JsonOpts);

    public static CheatSettings FromJson(string json)
        => JsonSerializer.Deserialize<CheatSettings>(json, JsonOpts) ?? new();

    public CheatSettings Clone() => FromJson(ToJson());
}

public sealed class AimSettings
{
    public bool Enabled { get; set; }
    public float Fov { get; set; } = 5.0f;
    public float Smooth { get; set; } = 3.5f;
    public string Bone { get; set; } = "Head";
    public bool Rcs { get; set; } = true;
    public bool VisibleOnly { get; set; } = true;
}

public sealed class EspSettings
{
    public bool Enabled { get; set; } = true;
    public bool Box { get; set; } = true;
    public bool CornerBox { get; set; }
    public bool Health { get; set; } = true;
    public bool Skeleton { get; set; }
    public bool Name { get; set; } = true;
    public bool Distance { get; set; } = true;
    public bool Snaplines { get; set; }
    public bool Armor { get; set; }
}

public sealed class TriggerSettings
{
    public bool Enabled { get; set; }
    public int DelayMs { get; set; } = 50;
    public int Jitter { get; set; } = 15;
    public int BurstCount { get; set; } = 1;
    public bool TeamCheck { get; set; } = true;
}

public sealed class MiscSettings
{
    public bool Bhop { get; set; }
    public bool NoFlash { get; set; }
    public bool Radar { get; set; }
    public float MaxFlashAlpha { get; set; } = 0.0f;
}
