using System.Net;
using System.Text;

namespace PracticaPalabrasMAUI;

public partial class VisualitzationWordPage : ContentPage, IQueryAttributable
{
    private string word;
    private Speak speak;
    private bool can;

    public VisualitzationWordPage()
    {
        word= string.Empty;
        InitializeComponent();
        BindingContext = this;
        speak = new Speak();
    }
    public string Word { get => word; set { word = value;OnPropertyChanged(); } }
    public bool Can { get => can; set { can = value; } }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        Can = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

        speak.Config();

        if (query.ContainsKey(nameof(Word)))
        {
            Can = true;
            Word = WebUtility.UrlDecode(query[nameof(Word)].ToString()).ToUpper();

            Task.Run(async() =>
            {
                string word = Word;
                await Task.Delay(1000 * 5);
                await speak.Read(word,this);
                if (Can)
                {
                    await Navigation.PopAsync();
                }

            });
        }


    }
}