using Microsoft.Maui.Controls;

namespace PracticaPalabrasMAUI;

public partial class DictionaryPage : ContentPage
{
    public static string text = string.Empty;

    public DictionaryPage()
    {
        InitializeComponent();
        BindingContext = this;
       
    }

    public string Text { get => text; set { text = value;OnPropertyChanged(); } }

    public static IEnumerable<Word> AllWords => text.Contains('\r')?text.Split('\r').Select(p=>Word.FromLine(p)) :text.Length>0?new Word[] { Word.FromLine(text) } :Array.Empty<Word>();

}