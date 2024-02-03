using System.Globalization;

namespace PracticaPalabrasMAUI
{
    public class Language
    {
        private static string langCode;

        public const string LangCodeEsp = "es-ES";
        public const string LangCodeCat = "es-CA";
        public const string LangCodeEng = "en-UK";
        public static string[] LangCodes => new string[] { LangCodeEng, LangCodeEsp, LangCodeCat };
        public static string LangCode { get => langCode; set { langCode = value; Preferences.Set(nameof(LangCode), langCode); } }

        static Language()
        {
            string sysLang = Preferences.Get("LangCode", defaultValue: GetSystemLanguage());
            if (LangCodes.Contains(sysLang))
            {
                LangCode = sysLang;
            }
            else
            {
                LangCode = LangCodeEng;
            }
        }

        public static string GetSystemLanguage()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            return $"{currentCulture.TwoLetterISOLanguageName}-{currentCulture.Name.Split('-')[1]}";
        }




    }



    public static class ExtensionLang
    {
        public static void Config(this Speak speak)
        {
            switch (Language.LangCode)
            {
                case Language.LangCodeCat:
                    speak.ConfigCat();
                    break;
                case Language.LangCodeEsp:
                    speak.ConfigEsp();
                    break;
                //aqui van el resto de configuraciones
            }
        }
    }
}
