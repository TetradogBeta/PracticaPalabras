using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace PracticaPalabras;

public partial class App : Avalonia.Application
{
    private ResourceDictionary? _antLangDic;

    public static App? CurrentApp { get; private set; }

    public event EventHandler? LangChanged;

    public App()
    {
        CurrentApp = this;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ApplySavedTheme();
            UpdateLang();
            desktop.MainWindow = new ShellWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }

    public void ApplySavedTheme()
    {
        int themeIndex = PreferencesService.Get("ThemeIndex", 0);
        RequestedThemeVariant = themeIndex switch
        {
            1 => ThemeVariant.Light,
            2 => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }

    public void UpdateLang()
    {
        string code = Language.LangCode;
        ResourceDictionary dic = LoadLangDict(code);

        if (_antLangDic != null)
        {
            Resources.MergedDictionaries.Remove(_antLangDic);
        }
        Resources.MergedDictionaries.Add(dic);
        _antLangDic = dic;

        LangChanged?.Invoke(this, EventArgs.Empty);
    }

    private static ResourceDictionary LoadLangDict(string code)
    {
        var uri = new Uri($"avares://PracticaPalabras/Languages/{code}.axaml");
        if (AvaloniaXamlLoader.Load(uri) is not ResourceDictionary dic)
        {
            throw new InvalidOperationException($"Failed to load language dictionary: {code}");
        }
        return dic;
    }
}