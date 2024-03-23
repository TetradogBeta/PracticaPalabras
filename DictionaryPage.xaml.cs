using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Text;


namespace PracticaPalabrasMAUI;

public partial class DictionaryPage : ContentPage
{

    private static string FileDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Language.LangCode);
    private static string FilePath => Path.Combine(FileDir, "dictionary.txt");



    private string text;

    public DictionaryPage()
    {
        InitializeComponent();
        BindingContext = this;
      
        App.CurrentApp.langChanged += LoadText;

        LoadText();

    

    }
 
    public string Text { 
        get => text; 
        set { 
            text = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasDuplicateds));
            Save(TextToSave);
        }
    }

    string TextToSave { get {

             StringBuilder sb = new StringBuilder();
            foreach (Word word in Clear(Parse(Text)))
            {
                sb.AppendLine(word.ToSaveString);
                
            }
            return sb.ToString();
     } }

    public bool HasDuplicateds { 
        get { 

            IEnumerable<Word> dirty = Parse(Text);
            return Clear(dirty).Count != dirty.Count();
        }
    }


    void LoadText(object sender=null, EventArgs e=null)
    {
        Text = Load();
    }

    static IEnumerable<Word> AllWordsDirty => Parse(Load());

    public static IList<Word> AllWords => Clear(AllWordsDirty);




    static IEnumerable<Word> Parse(string text)
    {
        IEnumerable<Word> res;

        if(text == null)
        {
            text = string.Empty;
        }

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
    static IList<Word> Clear(IEnumerable<Word> words)
    {
        SortedList<string, Word> dic = new SortedList<string, Word>();


        foreach (Word word in words)
        {
            if (!dic.ContainsKey(word.Content))
            {
                dic.Add(word.Content, word);
            }
        }
        return dic.Values;
    }
    static void Save(string valor=null)
    {

        if (!Directory.Exists(FileDir))
        {
            Directory.CreateDirectory(FileDir);
        }
        
        File.WriteAllText(FilePath, valor);
    }

    static string Load()
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