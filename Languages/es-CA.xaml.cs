namespace PracticaPalabrasMAUI;

public partial class es_CA : ResourceDictionary
{
	public es_CA()
	{
		InitializeComponent();
	}
}

public static class ExtensionCat
{
    public static void ConfigCat(this Speak speak)
    {
        if (speak.LangCode != Language.LangCodeCat)
        {
            speak.DicComplexPronuntiation.Clear();
            speak.DicComplexPronuntiation.Add("�", "o amb accent ubert");
            speak.DicComplexPronuntiation.Add("�", "o amb accent tancat");
            speak.DicComplexPronuntiation.Add("�", "e amb accent ubert");
            speak.DicComplexPronuntiation.Add("�", "e amb accent tancat");
            speak.DicComplexPronuntiation.Add("�", "a amb accent");
            speak.DicComplexPronuntiation.Add("�", "i amb accent");
            speak.DicComplexPronuntiation.Add("�", "u amb accent");
            speak.DicComplexPronuntiation.Add("�", "u amb dieresis");
            speak.DicComplexPronuntiation.Add("�", "i amb dieresis");
            speak.DicComplexPronuntiation.Add("l�l", "l geminada");
            speak.DicComplexPronuntiation.Add("ny", "e�a");
            speak.DicComplexPronuntiation.Add("-", "gui�");
            speak.DicComplexPronuntiation.Add("\"", "cometes");
            speak.DicComplexPronuntiation.Add("'", "apostrof");
            speak.DicComplexPronuntiation.Add(" ", "espai");
            speak.DicComplexPronuntiation.Add("�", "s� trancada");
            speak.LangCode = Language.LangCodeCat;
        }
    }
}