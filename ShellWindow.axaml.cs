using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PracticaPalabras.Views;

namespace PracticaPalabras;

public partial class ShellWindow : Window
{
    public static ShellWindow? Instance { get; private set; }

    public ShellWindow()
    {
        InitializeComponent();

        Instance = this;
        TxtVersion.Text = $"v{GetAppVersion()}";

        App.CurrentApp!.LangChanged += (_, _) => RefreshLangDependentUI();
        RefreshLangDependentUI();

        NavigateTo<MainView>();
    }

    private static string GetAppVersion()
    {
        try
        {
            var info = typeof(App).Assembly.GetName();
            return info.Version?.ToString(3) ?? "0.0.0";
        }
        catch
        {
            return "0.0.0";
        }
    }

    public void NavigateTo<T>() where T : UserControl, new()
    {
        ContentArea.Content = new T();
        SplitView.IsPaneOpen = false;
    }

    public void NavigateTo(UserControl view)
    {
        ContentArea.Content = view;
        SplitView.IsPaneOpen = false;
    }

    private void OnTogglePane(object? sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
    }

    private void OnNavigatePractice(object? sender, RoutedEventArgs e) => NavigateTo<MainView>();
    private void OnNavigateDictionary(object? sender, RoutedEventArgs e) => NavigateTo<DictionaryView>();
    private void OnNavigateConfiguration(object? sender, RoutedEventArgs e) => NavigateTo<ConfigurationView>();

    private void RefreshLangDependentUI()
    {
        LstLangs.Children.Clear();
        foreach (string code in Language.LangCodes)
        {
            LstLangs.Children.Add(BuildLangChip(code, code == Language.LangCode));
        }
    }

    private static Border BuildLangChip(string code, bool isActive)
    {
        var label = new TextBlock
        {
            Text = code,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            FontSize = 12,
            Foreground = ResolveForeground(isActive)
        };

        var chip = new Border { Child = label };
        chip.Classes.Add("lang-chip");
        if (isActive)
        {
            chip.Classes.Add("lang-chip-active");
        }

        chip.Tapped += (_, _) =>
        {
            Language.LangCode = code;
            App.CurrentApp?.UpdateLang();
        };

        return chip;
    }

    private static Avalonia.Media.IBrush? ResolveForeground(bool isActive)
    {
        var resources = Avalonia.Application.Current?.Resources;
        if (resources == null)
        {
            return null;
        }
        string key = isActive ? "PrimaryBrush" : "OnSurfaceVariantBrush";
        return resources.TryGetResource(key, Avalonia.Styling.ThemeVariant.Default, out var brush)
            ? brush as Avalonia.Media.IBrush
            : null;
    }
}