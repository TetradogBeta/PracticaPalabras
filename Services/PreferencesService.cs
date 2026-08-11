using System.Text.Json;

namespace PracticaPalabras;

public static class PreferencesService
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PracticaPalabras",
        "preferences.json");

    private static readonly Dictionary<string, object?> Store = Load();

    public static string Get(string key, string defaultValue) =>
        Store.TryGetValue(key, out object? v) ? v?.ToString() ?? defaultValue : defaultValue;

    public static int Get(string key, int defaultValue) =>
        Store.TryGetValue(key, out object? v) && int.TryParse(v?.ToString(), out int i) ? i : defaultValue;

    public static bool Get(string key, bool defaultValue)
    {
        if (Store.TryGetValue(key, out object? v) && bool.TryParse(v?.ToString(), out bool b))
        {
            return b;
        }
        return defaultValue;
    }

    public static DateTime Get(string key, DateTime defaultValue) =>
        Store.TryGetValue(key, out object? v) && DateTime.TryParse(v?.ToString(), out DateTime d)
            ? d.ToUniversalTime()
            : defaultValue;

    public static void Set(string key, string value)
    {
        Store[key] = value;
        Save();
    }

    public static void Set(string key, int value)
    {
        Store[key] = value;
        Save();
    }

    public static void Set(string key, bool value)
    {
        Store[key] = value;
        Save();
    }

    public static void Set(string key, DateTime value)
    {
        Store[key] = value.ToUniversalTime();
        Save();
    }

    private static Dictionary<string, object?> Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<Dictionary<string, object?>>(json)
                    ?? new Dictionary<string, object?>();
            }
        }
        catch
        {
        }
        return new Dictionary<string, object?>();
    }

    private static void Save()
    {
        try
        {
            string? dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string json = JsonSerializer.Serialize(Store, new JsonSerializerOptions { WriteIndented = false });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
        }
    }
}