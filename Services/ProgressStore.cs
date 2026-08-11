using System.Text.Json;

namespace PracticaPalabras;

public class ProgressStore
{
    public Dictionary<string, int> DicRepeat { get; set; } = new();
    public Dictionary<string, int> DicWrongWords { get; set; } = new();

    private static string KeyFor(string langCode) => $"Progress_{langCode}";

    public static ProgressStore Load(string langCode)
    {
        string json = PreferencesService.Get(KeyFor(langCode), string.Empty);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new ProgressStore();
        }
        try
        {
            return JsonSerializer.Deserialize<ProgressStore>(json) ?? new ProgressStore();
        }
        catch
        {
            return new ProgressStore();
        }
    }

    public void Save(string langCode)
    {
        string json = JsonSerializer.Serialize(this);
        PreferencesService.Set(KeyFor(langCode), json);
    }

    public static void Clear(string langCode)
    {
        PreferencesService.Set(KeyFor(langCode), string.Empty);
    }
}
