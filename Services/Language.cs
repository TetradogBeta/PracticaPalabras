using System.Globalization;

namespace PracticaPalabras;

public static class Language
{
    private static string langCode = string.Empty;

    public const string LangCodeEsp = "es-ES";
    public const string LangCodeCat = "es-CA";
    public const string LangCodeEng = "en-UK";

    public static string[] LangCodes => [LangCodeEng, LangCodeEsp, LangCodeCat];

    public static string LangCode
    {
        get => langCode;
        set
        {
            langCode = value;
            PreferencesService.Set(nameof(LangCode), langCode);
        }
    }

    static Language()
    {
        string sysLang = PreferencesService.Get(nameof(LangCode), GetSystemLanguage());
        LangCode = LangCodes.Contains(sysLang) ? sysLang : LangCodeEng;
    }

    public static string GetSystemLanguage()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string twoLetter = currentCulture.TwoLetterISOLanguageName;
        string[] parts = currentCulture.Name.Split('-');
        string region = parts.Length > 1 ? parts[1] : twoLetter.ToUpperInvariant();
        return $"{twoLetter}-{region}";
    }

    public static void ConfigSpeak(Speak speak)
    {
        switch (LangCode)
        {
            case LangCodeCat:
                ConfigCat(speak);
                break;
            case LangCodeEsp:
                ConfigEsp(speak);
                break;
            default:
                speak.DicComplexPronuntiation.Clear();
                break;
        }
    }

    private static void ConfigCat(Speak speak)
    {
        if (speak.LangCode == LangCodeCat)
        {
            return;
        }
        speak.DicComplexPronuntiation.Clear();
        speak.DicComplexPronuntiation.Add("\u00F2", "o amb accent ubert");
        speak.DicComplexPronuntiation.Add("\u00F3", "o amb accent tancat");
        speak.DicComplexPronuntiation.Add("\u00E8", "e amb accent ubert");
        speak.DicComplexPronuntiation.Add("\u00E9", "e amb accent tancat");
        speak.DicComplexPronuntiation.Add("\u00E0", "a amb accent");
        speak.DicComplexPronuntiation.Add("\u00ED", "i amb accent");
        speak.DicComplexPronuntiation.Add("\u00FA", "u amb accent");
        speak.DicComplexPronuntiation.Add("\u00FC", "u amb dieresis");
        speak.DicComplexPronuntiation.Add("\u00EF", "i amb dieresis");
        speak.DicComplexPronuntiation.Add("l\u00B7l", "l geminada");
        speak.DicComplexPronuntiation.Add("ny", "e\u00F1a");
        speak.DicComplexPronuntiation.Add("-", "gui\u00F3");
        speak.DicComplexPronuntiation.Add("\"", "cometes");
        speak.DicComplexPronuntiation.Add("'", "ap\u00F2strof");
        speak.DicComplexPronuntiation.Add(" ", "espai");
        speak.DicComplexPronuntiation.Add("ss", "s trencada");
        speak.LangCode = LangCodeCat;
    }

    private static void ConfigEsp(Speak speak)
    {
        if (speak.LangCode == LangCodeEsp)
        {
            return;
        }
        speak.DicComplexPronuntiation.Clear();
        speak.DicComplexPronuntiation.Add("\u00F3", "o con tilde");
        speak.DicComplexPronuntiation.Add("\u00E9", "e con tilde");
        speak.DicComplexPronuntiation.Add("\u00E1", "a con tilde");
        speak.DicComplexPronuntiation.Add("\u00ED", "i con tilde");
        speak.DicComplexPronuntiation.Add("\u00FA", "u con tilde");
        speak.DicComplexPronuntiation.Add("\u00FC", "u con dieresis");
        speak.DicComplexPronuntiation.Add("\u00EF", "i con dieresis");
        speak.LangCode = LangCodeEsp;
    }
}