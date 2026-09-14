namespace TarkovCheat.Core.Config;

/// <summary>Named JSON config profiles stored under %APPDATA%.</summary>
public sealed class ProfileManager
{
    private readonly string _profileDir;
    private readonly Dictionary<string, CheatSettings> _cache = new(StringComparer.OrdinalIgnoreCase);

    public string ActiveProfile { get; private set; } = "default";

    public ProfileManager(string appDataSlug)
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _profileDir = Path.Combine(appData, appDataSlug, "profiles");
        Directory.CreateDirectory(_profileDir);
    }

    public CheatSettings Load(string name)
    {
        if (_cache.TryGetValue(name, out var cached))
            return cached;

        string path = Path.Combine(_profileDir, name + ".json");
        if (!File.Exists(path))
        {
            var defaults = new CheatSettings();
            Save(name, defaults);
            return defaults;
        }

        string json = File.ReadAllText(path);
        var settings = CheatSettings.FromJson(json);
        _cache[name] = settings;
        return settings;
    }

    public void Save(string name, CheatSettings settings)
    {
        string path = Path.Combine(_profileDir, name + ".json");
        File.WriteAllText(path, settings.ToJson());
        _cache[name] = settings;
    }

    public void SetActive(string name) => ActiveProfile = name;

    public CheatSettings GetActive() => Load(ActiveProfile);

    public void SaveActive(CheatSettings settings) => Save(ActiveProfile, settings);

    public IReadOnlyList<string> ListProfiles()
    {
        if (!Directory.Exists(_profileDir))
            return Array.Empty<string>();

        return Directory.GetFiles(_profileDir, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n is not null)
            .Select(n => n!)
            .OrderBy(n => n)
            .ToList();
    }

    public bool DeleteProfile(string name)
    {
        if (string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
            return false;
        string path = Path.Combine(_profileDir, name + ".json");
        if (!File.Exists(path)) return false;
        File.Delete(path);
        _cache.Remove(name);
        return true;
    }

    public void ImportJson(string name, string json)
    {
        var settings = CheatSettings.FromJson(json);
        Save(name, settings);
    }

    public string ExportJson(string name) => Load(name).ToJson();
}
