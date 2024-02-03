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
            speak.DicComplexPronuntiation.Add("�", "o con tilde");
            speak.DicComplexPronuntiation.Add("�", "e con tilde ");
            speak.DicComplexPronuntiation.Add("�", "a con tilde");
            speak.DicComplexPronuntiation.Add("�", "i con tilde");
            speak.DicComplexPronuntiation.Add("�", "u con tilde");
            speak.DicComplexPronuntiation.Add("�", "u con dieresis");
            speak.DicComplexPronuntiation.Add("�", "i con dieresis");

            speak.LangCode = Language.LangCodeEsp;
        }
    }
}