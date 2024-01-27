using System.Globalization;

namespace PracticaPalabrasMAUI;

public partial class App : Application
{
	public App()
	{
      
		InitializeComponent();
        SetLang(GetSystemLanguage());

        MainPage = new AppShell();
     
    }
    public string GetSystemLanguage()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;

        // Puedes obtener el código de idioma de la cultura actual
        string languageCode = currentCulture.Name;

        // O puedes obtener el código de idioma en formato "es-ES", "es-CA", etc.
        string languageCodeFormatted = $"{currentCulture.TwoLetterISOLanguageName}-{currentCulture.Name.Split('-')[1]}";

        return languageCodeFormatted;
    }
    public void SetLang(string lang)
    {
        ResourceDictionary dic;

        switch(lang)
        {
            case "es-ES":
                dic =new es_ES();
                break;
            case "es-CA":
                dic = new es_CA();
                break;
            default:
                dic = new en_UK();
                break;
        }
        Current.Resources.MergedDictionaries.Add(dic);
    }
}
