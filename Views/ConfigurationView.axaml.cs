using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace PracticaPalabras.Views;

public partial class ConfigurationView : UserControl, INotifyPropertyChanged
{
    public static ConfigurationView Instance { get; set; } = new();

    public new event PropertyChangedEventHandler? PropertyChanged;

    public ConfigurationView()
    {
        InitializeComponent();
        DataContext = this;
        Instance = this;
    }

    public int ThemeIndex
    {
        get => PreferencesService.Get(nameof(ThemeIndex), 0);
        set
        {
            if (ThemeIndex == value)
            {
                return;
            }
            PreferencesService.Set(nameof(ThemeIndex), value);
            ApplyTheme();
            OnPropertyChanged();
        }
    }

    public int ImagineRate
    {
        get => PreferencesService.Get(nameof(ImagineRate), 1);
        set
        {
            int clamped = value < 0 ? 0 : value;
            if (ImagineRate == clamped)
            {
                return;
            }
            PreferencesService.Set(nameof(ImagineRate), clamped);
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsImagineDisabled));
        }
    }

    public bool IsImagineDisabled => ImagineRate == 0;

    public bool SpeakAllWord
    {
        get => Speak.SpeakAllWord;
        set
        {
            if (Speak.SpeakAllWord == value)
            {
                return;
            }
            Speak.SpeakAllWord = value;
            OnPropertyChanged();
        }
    }

    public bool SortDictionary
    {
        get => PreferencesService.Get(nameof(SortDictionary), false);
        set
        {
            if (SortDictionary == value)
            {
                return;
            }
            PreferencesService.Set(nameof(SortDictionary), value);
            OnPropertyChanged();
        }
    }

    public bool PersistProgress
    {
        get => PreferencesService.Get(nameof(PersistProgress), false);
        set
        {
            if (PersistProgress == value)
            {
                return;
            }
            PreferencesService.Set(nameof(PersistProgress), value);
            OnPropertyChanged();
        }
    }

    public void ClearProgress()
    {
        ProgressStore.Clear(Language.LangCode);
        Views.MainView.ForceClearProgressForLanguage(Language.LangCode);
    }

    private void OnClearProgressClicked(object? sender, RoutedEventArgs e)
    {
        ClearProgress();
    }

    private void ApplyTheme()
    {
        if (Avalonia.Application.Current is { } app)
        {
            app.RequestedThemeVariant = ThemeIndex switch
            {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}