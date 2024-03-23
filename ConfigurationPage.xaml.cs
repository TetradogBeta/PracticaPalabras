namespace PracticaPalabrasMAUI;

public partial class ConfigurationPage : ContentPage
{
	public static ConfigurationPage Instance { get; set; } = new ConfigurationPage();
	public ConfigurationPage()
	{
		InitializeComponent();
		Instance = this;
		BindingContext = this;
	}


	public int ImagineRate
	{
		get => Preferences.Get(nameof(ImagineRate), 1);
		set
		{
			if (value < 0)
			{
				value = 0;
			}

            Preferences.Set(nameof(ImagineRate), value);
            OnPropertyChanged();
			OnPropertyChanged(nameof(IsImagineDisabled));
        }
	}
	public bool IsImagineDisabled => ImagineRate == 0;


	public bool SpeakAllWord
	{
		get => Speak.SpeakAllWord;
		set
		{
			Speak.SpeakAllWord = value;
            OnPropertyChanged();
		}
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		SpeakAllWord = !SpeakAllWord;
    }
}