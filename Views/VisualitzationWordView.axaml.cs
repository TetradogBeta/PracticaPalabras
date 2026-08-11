using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace PracticaPalabras.Views;

public partial class VisualitzationWordView : UserControl, INotifyPropertyChanged, IReadController
{
    private readonly Speak speak = new();
    private readonly ISpeechService speechService = new SpeechService();
    private readonly CancellationTokenSource cts = new();

    private string word = string.Empty;

    public VisualitzationWordView()
    {
        InitializeComponent();
        DataContext = this;

        Language.ConfigSpeak(speak);
        Loaded += OnLoaded;
    }

    public string Word
    {
        get => word;
        set
        {
            if (word != value)
            {
                word = value;
                OnPropertyChanged();
            }
        }
    }

    public bool CanContinue => !cts.IsCancellationRequested;

    public CancellationToken CancellationToken => cts.Token;

    public string GetWord() => word;

    public void SetWord(string value) => Word = value;

    private async void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            await Task.Delay(5000, cts.Token);
            await speak.Read(word, this, speechService);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        if (!cts.IsCancellationRequested)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => ShellWindow.Instance?.NavigateTo<MainView>());
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        cts.Cancel();
        cts.Dispose();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}