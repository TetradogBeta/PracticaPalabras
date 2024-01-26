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
    public bool Can { get; set; }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        Can = false;
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey(nameof(Word)))
        {
            Can = true;
            Word = query[nameof(Word)].ToString().ToUpper();

            Task.Run(async() =>
            {
                string word = Word;
                await Task.Delay(1000 * 5);
                if (Can)
                {
                    Word = "";
                    for (int i = word.Length - 1; i >= 0 && Can; i--)
                    {
                        await TextToSpeech.Default.SpeakAsync(word[i] + "");
                        if (Can)
                        {
                            Word = word[i] + Word;
                            await Task.Delay(1000);
                        }
                    }
                    if (Can)
                    {
                        await TextToSpeech.Default.SpeakAsync(word);
                        await Navigation.PopAsync();
                    }
                }

            });
        }


    }
}