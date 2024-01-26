namespace PracticaPalabrasMAUI;

public partial class VisualitzationWordPage : ContentPage, IQueryAttributable
{
    private string word;

    public VisualitzationWordPage()
    {
        word= string.Empty;
        InitializeComponent();
        BindingContext = this;
    }

    public string Word { get => word; set { word = value;OnPropertyChanged(); } }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey(nameof(Word)))
        {
            Word = query[nameof(Word)].ToString().ToUpper();

            Task.Run(async() =>
            {
                string word = Word;
                await Task.Delay(1000 * 5);
                Word = "";
                for(int i=word.Length-1;i>=0;i--)
                {
                    await TextToSpeech.Default.SpeakAsync(word[i]+"");
                    Word = word[i] + Word;
                    await Task.Delay(1000);
                }
                await TextToSpeech.Default.SpeakAsync(word);
                await Navigation.PopAsync();

            });
        }


    }
}