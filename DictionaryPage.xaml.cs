using Microsoft.Maui.Controls;

namespace PracticaPalabrasMAUI;

public partial class DictionaryPage : ContentPage
{
    
    private static string text = Load();
    private static string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dictionary.txt");

    

    public DictionaryPage()
    {
        InitializeComponent();
        BindingContext = this;
       
    }
    public string Text { get => text; set { text = value;OnPropertyChanged();Save(); } }

    public static IEnumerable<Word> AllWords => text.Contains('\r')?text.Split('\r').Select(p=>Word.FromLine(p)) :text.Length>0?new Word[] { Word.FromLine(text) } :Array.Empty<Word>();
 
    public static void Save(string valor=null)
    {
        File.WriteAllText(FilePath, valor ?? text);
    }

    public static string Load()
    {
        if (File.Exists(FilePath))
        {
            return File.ReadAllText(FilePath);
        }
        else
        {
            return string.Empty; // o proporciona un valor predeterminado
        }
    }
}