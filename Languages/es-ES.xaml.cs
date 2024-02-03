namespace PracticaPalabrasMAUI;

public partial class es_ES : ResourceDictionary
{
	public es_ES()
	{
		InitializeComponent();
	}
}

public static class ExtensionEsp
{
	public static void ConfigEsp(this Speak speak)
	{
        if (speak.LangCode != Language.LangCodeEsp)
        {
            speak.DicComplexPronuntiation.Clear();
            speak.DicComplexPronuntiation.Add("ó", "o con tilde");
            speak.DicComplexPronuntiation.Add("é", "e con tilde ");
            speak.DicComplexPronuntiation.Add("á", "a con tilde");
            speak.DicComplexPronuntiation.Add("í", "i con tilde");
            speak.DicComplexPronuntiation.Add("ú", "u con tilde");
            speak.DicComplexPronuntiation.Add("ü", "u con dieresis");
            speak.DicComplexPronuntiation.Add("ï", "i con dieresis");

            speak.LangCode = Language.LangCodeEsp;
        }
    }
}