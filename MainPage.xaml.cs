using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PracticaPalabrasMAUI;

public partial class MainPage : ContentPage
{
    private string text;
    private int max;
    private int current;

    const int INDEX = 60;
    static int TotalRepeat => 5;

    SortedList<string,int> DicRepeat {  get; set; }

    SortedList<string,Word> Dic {  get; set; }

    Random Random { get; set; }
    public MainPage()
    {

        Random = new Random();
        DicRepeat = new SortedList<string,int>();
        Dic = new SortedList<string,Word>();
        
        text = string.Empty;
        InitializeComponent();
        max = Preferences.Get(nameof(Max), 0);
        BindingContext = this;
        
        
   
    }

    public IList<Word> Words { get; set; }


    public int Max { 
        get => max; 
        set { 
            max = value;
            Preferences.Set(nameof(Max), max);
            OnPropertyChanged();
        }
    }
    public int Current { 
        get => current; 
        set { 
            current = value; 
            OnPropertyChanged();
            if (current > max)
            {
                Max = current;
            }
        }
    }


    public string Text { get => text; set { text = value; OnPropertyChanged(); } }

    public Word Actual
    {
        get
        {
            return Resources["actual"] as Word;
        }
        set
        {
            Word actual = Actual;
            if (actual != null)
            {
                actual.Content = value.Content;
                actual.Clue = value.Clue;
            }
        }
    }
    private async Task UpdateWord()
    {

        Word newWorld;
        string uri;
        string repeat;
     

        if (Words.Count > 0)
        {
           
            if (DicRepeat.Count > 0 && Random.Next(100) < INDEX)
            {
                repeat = DicRepeat.Keys[Random.Next(DicRepeat.Count)];
                Actual = Dic[repeat];

            }
            else
            {
                do
                {
                    newWorld = Words[Random.Next(Words.Count)];
                } while (Words.Count > 1 && newWorld.Content == Actual.Content);
                Actual = newWorld;
            }
        }
        else
        {
            uri = $"{nameof(DictionaryPage)}";
            await Shell.Current.GoToAsync(uri);
        }
    }

    private void UpdateRepetedsDic()
    {

        foreach (string word in DicRepeat.Keys.ToArray())
        {
            
            if (!Dic.ContainsKey(word))
            {
                DicRepeat.Remove(word);
            }
        }

    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        txtWord.Text = "";
        Words = DictionaryPage.AllWords.ToList();
        Dic.Clear();
        foreach(Word word in Words)
        {
            Dic.Add(word.Content, word);
        }
        UpdateRepetedsDic();
        await UpdateWord();

    }

    private async void CheckWord(object sender = null, EventArgs e = null)
    {
        Word word = Actual;
        string uri;
        if (!Equals(word, null))
        {

            if (word.ToString() == Text.Trim().ToLower())
            {
                if (DicRepeat.ContainsKey(word.Content))
                {
                    if (DicRepeat[word.Content] > 0)
                    {
                        DicRepeat[word.Content]--;
                    }
                    else
                    {
                        DicRepeat.Remove(word.Content);
                    }
                }
                Text = "";
                Current++;
                await UpdateWord();
            }
            else
            {
                Current = 0;
                if (!DicRepeat.ContainsKey(word.Content))
                {
                    DicRepeat.Add(word.Content, TotalRepeat);
                }
                else
                {
                    DicRepeat[word.Content] =TotalRepeat;
                }
                uri = $"{nameof(VisualitzationWordPage)}?{nameof(VisualitzationWordPage.Word)}={word}";
                await Shell.Current.GoToAsync(uri);
            }





        }
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        Editor entry = (Editor)sender;
        if (entry != null && (entry.Text.EndsWith(Environment.NewLine)))
        {
            CheckWord();
        }
    }
}


public class Word : INotifyPropertyChanged
{
    private string content = string.Empty;
    private string clue = string.Empty;

    public string Clue { 
        get => clue; 
        set {
            clue = value; OnPropertyChanged(); 
        } 
    }
    public string Content { 
        get => content; 
        set { 
            content = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(HiddenContent));
        } 
    }
    public string HiddenContent
    {
        get
        {

            StringBuilder result = new();

            for (int i = 0; i < Content.Length; i++)
            {
                if (char.IsUpper(Content[i]))
                {
                    result.Append('_');
                }
                else
                {
                    result.Append(Content[i]);
                }
            }


            return result.ToString();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;


 
    public override string ToString()
    {
        return Content.ToLower();
    }
    
    
    private void OnPropertyChanged([CallerMemberName] string name=null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    public static Word FromLine(string line)
    {

        string[] fields =line.Contains(';')? line.Split(';'):new string[] { line };
        return new Word() { Content = fields[0], Clue = fields.Length > 1 ? fields[1]:string.Empty };
    }
}

