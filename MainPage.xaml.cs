using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace PracticaPalabrasMAUI;

public partial class MainPage : ContentPage
{
    private string text;
    private Random Random { get; set; }
    public MainPage()
    {
		
        Random= new Random();
        text = String.Empty;
        InitializeComponent();

		BindingContext = this;
        

        UpdateWord();

    }

   
    public static List<Word> Words { get; set; }= new List<Word>
	{
		new Word(){Content="Planeta",Clue="Pista1"},
        new Word(){Content="Tierra",Clue="Pista2"}
    };

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
    private void UpdateWord()
    {
        Actual = Words[Random.Next(Words.Count)];
    }

    private async void Button_Clicked(object sender=null, EventArgs e=null)
    {
		Word word = Actual;
        string uri;
		if(!Equals(word, null)) {
		
			if(word.ToString() == Text.Trim().ToLower())
			{
				Text = "";
                UpdateWord();
            }
            else
            {
                uri = $"{nameof(VisualitzationWordPage)}?{nameof(VisualitzationWordPage.Word)}={word}";
                await Shell.Current.GoToAsync(uri);
            }
		
		
		
		
		
		}
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry= (Entry)sender;
        if(entry != null && entry.Text.Contains(' '))
        {
            Button_Clicked();
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
    [JsonIgnore]
    public string HiddenContent
    {
        get
        {

            string result = "";

            for (int i = 0; i < Content.Length; i++)
            {
                if (Char.IsUpper(Content[i]))
                {
                    result += "_";
                }
                else
                {
                    result += Content[i];
                }
            }


            return result;
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
}

