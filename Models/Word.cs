using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PracticaPalabras;

public class Word : INotifyPropertyChanged
{
    private string content = string.Empty;
    private string clue = string.Empty;

    private readonly Speak speak = new();

    public string Clue
    {
        get => clue;
        set
        {
            clue = value;
            OnPropertyChanged();
        }
    }

    public string Content
    {
        get => content;
        set
        {
            content = value.Trim();
            OnPropertyChanged();
            OnPropertyChanged(nameof(HiddenContent));
        }
    }

    public string ToSaveString => $"{Content};{Clue}";

    public string HiddenContent
    {
        get
        {
            Language.ConfigSpeak(speak);
            return speak.ToHidden(Content);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public override string ToString() => Content.ToLower();

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public static Word FromLine(string line)
    {
        string[] fields = line.Contains(';') ? line.Split(';') : new[] { line };
        return new Word
        {
            Content = fields[0],
            Clue = fields.Length > 1 ? fields[1] : string.Empty
        };
    }
}