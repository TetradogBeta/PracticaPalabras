using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PracticaPalabrasMAUI;

public partial class MainPage : ContentPage
{
    private string text;
    private int max;
    private int current;

    const int NODATA = 2000;

    const int INDEX = 30;
    static int TotalRepeat => 3;


           
 
    public MainPage()
    {

        Random = new Random();
        DicRepeat = new SortedList<string,int>();
        Dic = new SortedList<string,Word>();
        DicWrongWords = new SortedList<string, int>();
        text = string.Empty;
        InitializeComponent();
        max = Preferences.Get(nameof(Max), 0);
        BindingContext = this;
        App.CurrentApp.langChanged +=async (s, e) =>await Load();
        Loaded +=async (s, e) => await UpdateWord();


    }
 

    Random Random { get; set; }
    IList<Word> Words { get; set; }

    SortedList<string,int> DicRepeat {  get; set; }
    SortedList<string, int> DicWrongWords { get; set; }

    SortedList<string,Word> Dic {  get; set; }
    public int Max { 
        get => max; 
        set { 
            max = value;
            Preferences.Set(nameof(Max), max);
            Record = DateTime.UtcNow;
            OnPropertyChanged();
        }
    }
    public bool HasRecord => Record.Year != NODATA;
    public DateTime Record { get => Preferences.Get(nameof(Record),new DateTime(NODATA,1,1).ToUniversalTime()); set =>Preferences.Set(nameof(Record), value); }
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
            txtWord.Focus();
        }
        else
        {
            uri = $"{nameof(DictionaryPage)}";
            await Shell.Current.GoToAsync(uri);
        }
    }

    private void UpdateDics()
    {

        foreach (string word in DicRepeat.Keys.ToArray())
        {
            
            if (!Dic.ContainsKey(word))
            {
                DicRepeat.Remove(word);
              
            }
        }

        foreach(string word in DicWrongWords.Keys.ToArray())
        {
            if(!Dic.ContainsKey(word))
            {
                DicWrongWords.Remove(word);

            }else if (ConfigurationPage.Instance.ImagineRate>0 && DicWrongWords[word] > ConfigurationPage.Instance.ImagineRate)
            {
                DicWrongWords[word]= ConfigurationPage.Instance.ImagineRate - 1;
            }
        }

    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
   
        base.OnNavigatedTo(args);
        await Load();
        
    }


    private async Task Load()
    {
        txtWord.Text = "";
   
        Dic.Clear();
        foreach (Word word in DictionaryPage.AllWords)
        {
            if (!Dic.ContainsKey(word.Content))
            {
                Dic.Add(word.Content, word);
            }
        }
        Words = Dic.Values;
        UpdateDics();
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
                Current++;
                await Next();

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
                if (!DicWrongWords.ContainsKey(word.Content))
                {
                    DicWrongWords.Add(word.Content, 1);
                }
                else
                {
                    DicWrongWords[word.Content]= DicWrongWords[word.Content] + 1;
                }
                if (DicWrongWords[word.Content] == ConfigurationPage.Instance.ImagineRate)
                {
                    uri = $"{nameof(VisualitzationWordPage)}?{nameof(VisualitzationWordPage.Word)}={word}";
                    await Shell.Current.GoToAsync(uri);
                    DicWrongWords[word.Content] = 0;
                }
                else
                {
                    await Next();
                }
            }





        }
    }
    private async Task Next()
    {
        Text = "";

        await UpdateWord();
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        Editor entry = (Editor)sender;
        if (entry != null && entry.Text.EndsWith(Environment.NewLine[0]))//cojo solo el primer caracter ya que en Windows ponen dos pero el Editor solo usa el primero...
        {
            CheckWord();
        }
    }
}

