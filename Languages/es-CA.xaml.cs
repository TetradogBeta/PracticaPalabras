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
            speak.DicComplexPronuntiation.Add("ò", "o amb accent ubert");
            speak.DicComplexPronuntiation.Add("ó", "o amb accent tancat");
            speak.DicComplexPronuntiation.Add("è", "e amb accent ubert");
            speak.DicComplexPronuntiation.Add("é", "e amb accent tancat");
            speak.DicComplexPronuntiation.Add("à", "a amb accent");
            speak.DicComplexPronuntiation.Add("í", "i amb accent");
            speak.DicComplexPronuntiation.Add("ú", "u amb accent");
            speak.DicComplexPronuntiation.Add("ü", "u amb dieresis");
            speak.DicComplexPronuntiation.Add("ï", "i amb dieresis");
            speak.DicComplexPronuntiation.Add("l·l", "l geminada");
            speak.DicComplexPronuntiation.Add("ny", "eña");
            speak.DicComplexPronuntiation.Add("-", "guió");
            speak.DicComplexPronuntiation.Add("\"", "cometes");
            speak.DicComplexPronuntiation.Add("'", "apostrof");
            speak.DicComplexPronuntiation.Add(" ", "espai");
            speak.DicComplexPronuntiation.Add("ç", "sé trancada");
            speak.LangCode = Language.LangCodeCat;
        }
    }
}