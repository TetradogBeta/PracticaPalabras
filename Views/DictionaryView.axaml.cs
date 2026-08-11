using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PracticaPalabras.Views;

public partial class DictionaryView : UserControl, INotifyPropertyChanged
{
    private static string FileDir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        Language.LangCode);

    private static string FilePath => Path.Combine(FileDir, "dictionary.txt");

    private string text = string.Empty;

    public DictionaryView()
    {
        InitializeComponent();
        DataContext = this;

        App.CurrentApp!.LangChanged += (_, _) => Text = Load();
        Text = Load();
    }

    public string Text
    {
        get => text;
        set
        {
            if (text == value)
            {
                return;
            }
            text = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasDuplicateds));
            Save(TextToSave);
        }
    }

    private string TextToSave
    {
        get
        {
            var sb = new StringBuilder();
            foreach (Word word in Clear(Parse(text)))
            {
                sb.AppendLine(word.ToSaveString);
            }
            return sb.ToString();
        }
    }

    public bool HasDuplicateds
    {
        get
        {
            IEnumerable<Word> dirty = Parse(text);
            return Clear(dirty).Count != dirty.Count();
        }
    }

    private static IEnumerable<Word> AllWordsDirty => Parse(Load());

    public static IList<Word> AllWords => Clear(AllWordsDirty);

    private static IEnumerable<Word> Parse(string? text)
    {
        if (text == null)
        {
            text = string.Empty;
        }

        if (text.Contains(Environment.NewLine[0]))
        {
            return text.Split(Environment.NewLine[0])
                .Where(l => l.Trim().Length > 0)
                .Select(p => Word.FromLine(p));
        }
        return text.Length > 0
            ? new[] { Word.FromLine(text) }
            : Array.Empty<Word>();
    }

    private static IList<Word> Clear(IEnumerable<Word> words)
    {
        var dic = new SortedList<string, Word>();
        var list = new List<Word>();
        foreach (Word word in words)
        {
            if (!dic.ContainsKey(word.Content))
            {
                dic.Add(word.Content, word);
                list.Add(word);
            }
        }
        return ConfigurationView.Instance.SortDictionary ? dic.Values : list;
    }

    private static void Save(string? value)
    {
        if (!Directory.Exists(FileDir))
        {
            Directory.CreateDirectory(FileDir);
        }
        File.WriteAllText(FilePath, value ?? string.Empty);
    }

    private static string Load()
    {
        if (File.Exists(FilePath))
        {
            return File.ReadAllText(FilePath);
        }
        return string.Empty;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}