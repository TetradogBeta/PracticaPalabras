using Microsoft.Maui.Controls;
using System.Collections.Generic;

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

 


    public static IEnumerable<Word> AllWords
    {
        get
        {
            IEnumerable<Word> res;
            if (text.Contains(Environment.NewLine[0]))
            {
                res = text.Split(Environment.NewLine[0]).Where(l => l.Trim().Length > 0).Select(p => Word.FromLine(p));
            }
            else
            {
                res = text.Length > 0 ? new Word[] { Word.FromLine(text) } : Array.Empty<Word>();
            }
            return res;
        }
    }
 
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