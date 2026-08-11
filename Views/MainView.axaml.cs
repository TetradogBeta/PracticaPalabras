using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace PracticaPalabras.Views;

public partial class MainView : UserControl, INotifyPropertyChanged
{
    private const int NodataYear = 2000;
    private const int IndexThreshold = 30;
    private const int TotalRepeat = 3;

    private readonly Random random = new();
    private readonly SortedList<string, int> dicRepeat = new();
    private readonly SortedList<string, int> dicWrongWords = new();
    private readonly SortedList<string, Word> dic = new();

    private int max;
    private int current;
    private string text = string.Empty;
    private DateTime record;

    public MainView()
    {
        InitializeComponent();
        DataContext = this;

        max = PreferencesService.Get(nameof(Max), 0);
        record = PreferencesService.Get(nameof(Record), new DateTime(NodataYear, 1, 1).ToUniversalTime());

        Loaded += async (_, _) => await UpdateWord();
    }

    public IList<Word> Words { get; set; } = new List<Word>();

    public int Max
    {
        get => max;
        set
        {
            max = value;
            PreferencesService.Set(nameof(Max), max);
            Record = DateTime.UtcNow;
            OnPropertyChanged();
        }
    }

    public DateTime Record
    {
        get => record;
        set
        {
            record = value;
            PreferencesService.Set(nameof(Record), value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasRecord));
        }
    }

    public bool HasRecord => record.Year != NodataYear;

    public int Current
    {
        get => current;
        set
        {
            current = value;
            OnPropertyChanged();
            if (current > max)
            {
                Max = current;
            }
        }
    }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            OnPropertyChanged();
        }
    }

    public Word? Actual
    {
        get => Resources["actual"] as Word;
        set
        {
            if (Actual is { } currentActual && value != null)
            {
                currentActual.Content = value.Content;
                currentActual.Clue = value.Clue;
            }
        }
    }

    private Task UpdateWord()
    {
        if (Words.Count > 0)
        {
            Word newWord;
            if (dicRepeat.Count > 0 && random.Next(100) < IndexThreshold)
            {
                string repeat = dicRepeat.Keys[random.Next(dicRepeat.Count)];
                Actual = dic[repeat];
            }
            else
            {
                do
                {
                    newWord = Words[random.Next(Words.Count)];
                } while (Words.Count > 1 && newWord.Content == Actual?.Content);
                Actual = newWord;
            }
            TxtWord.Focus();
        }
        else
        {
            ShellWindow.Instance?.NavigateTo<DictionaryView>();
        }
        return Task.CompletedTask;
    }

    private void UpdateDics()
    {
        foreach (string word in dicRepeat.Keys.ToArray())
        {
            if (!dic.ContainsKey(word))
            {
                dicRepeat.Remove(word);
            }
        }

        foreach (string word in dicWrongWords.Keys.ToArray())
        {
            if (!dic.ContainsKey(word))
            {
                dicWrongWords.Remove(word);
            }
            else if (ConfigurationView.Instance.ImagineRate > 0 && dicWrongWords[word] > ConfigurationView.Instance.ImagineRate)
            {
                dicWrongWords[word] = ConfigurationView.Instance.ImagineRate - 1;
            }
        }
    }

    private void SaveProgressIfEnabled()
    {
        if (ConfigurationView.Instance.PersistProgress)
        {
            new ProgressStore
            {
                DicRepeat = dicRepeat.ToDictionary(kv => kv.Key, kv => kv.Value),
                DicWrongWords = dicWrongWords.ToDictionary(kv => kv.Key, kv => kv.Value)
            }.Save(Language.LangCode);
        }
    }

    private void LoadProgressIfEnabled()
    {
        if (!ConfigurationView.Instance.PersistProgress)
        {
            return;
        }
        var store = ProgressStore.Load(Language.LangCode);
        foreach (var kv in store.DicRepeat)
        {
            dicRepeat[kv.Key] = kv.Value;
        }
        foreach (var kv in store.DicWrongWords)
        {
            dicWrongWords[kv.Key] = kv.Value;
        }
    }

    public static void ForceClearProgressForLanguage(string langCode)
    {
        ProgressStore.Clear(langCode);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _ = Load();
    }

    private async Task Load()
    {
        TxtWord.Text = string.Empty;

        dic.Clear();
        dicRepeat.Clear();
        dicWrongWords.Clear();
        foreach (Word word in DictionaryView.AllWords)
        {
            if (!dic.ContainsKey(word.Content))
            {
                dic.Add(word.Content, word);
            }
        }
        Words = dic.Values;
        LoadProgressIfEnabled();
        UpdateDics();
        await UpdateWord();
    }

    private async void CheckWord()
    {
        Word? word = Actual;
        if (word == null)
        {
            return;
        }

        if (word.ToString() == Text.Trim().ToLower())
        {
            if (dicRepeat.ContainsKey(word.Content))
            {
                if (dicRepeat[word.Content] > 0)
                {
                    dicRepeat[word.Content]--;
                }
                else
                {
                    dicRepeat.Remove(word.Content);
                }
            }
            Current++;
            SaveProgressIfEnabled();
            await Next();
        }
        else
        {
            Current = 0;
            if (!dicRepeat.ContainsKey(word.Content))
            {
                dicRepeat.Add(word.Content, TotalRepeat);
            }
            else
            {
                dicRepeat[word.Content] = TotalRepeat;
            }
            if (!dicWrongWords.ContainsKey(word.Content))
            {
                dicWrongWords.Add(word.Content, 1);
            }
            else
            {
                dicWrongWords[word.Content] = dicWrongWords[word.Content] + 1;
            }

            if (dicWrongWords[word.Content] == ConfigurationView.Instance.ImagineRate)
            {
                var view = new VisualitzationWordView();
                view.SetWord(word.Content.ToUpper());
                ShellWindow.Instance?.NavigateTo(view);
                dicWrongWords[word.Content] = 0;
            }
            else
            {
                await Next();
            }
            SaveProgressIfEnabled();
        }
    }

    private async Task Next()
    {
        Text = string.Empty;
        await UpdateWord();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CheckWord();
            e.Handled = true;
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}