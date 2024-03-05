using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Text;


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
    public string Text { get => text; set { text = value; OnPropertyChanged(); Save(); } }

    private static string TextToSave { get {

             StringBuilder sb = new StringBuilder();
            foreach (Word word in AllWords)
            {
                sb.AppendLine(word.ToSaveString);
                
            }
            return sb.ToString();
     } }


    public static IList<Word> AllWords
    {
        get
        {
            IEnumerable<Word> res;
            SortedList<string, Word> dic = new SortedList<string, Word>();

            if (text.Contains(Environment.NewLine[0]))
            {
                res = text.Split(Environment.NewLine[0]).Where(l => l.Trim().Length > 0).Select(p => Word.FromLine(p));
            }
            else
            {
                res = text.Length > 0 ? new Word[] { Word.FromLine(text) } : Array.Empty<Word>();
            }
     
            foreach (Word word in res)
            {
                if (!dic.ContainsKey(word.Content))
                {
                    dic.Add(word.Content, word);
                }
            }
            return dic.Values;
        }
    }
 
    public static void Save(string valor=null)
    {
        File.WriteAllText(FilePath, valor ?? TextToSave);
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