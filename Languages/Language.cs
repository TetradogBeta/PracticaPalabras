using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaPalabrasMAUI
{
    public class Language
    {
        private static string langCode;

        public static string LangCodeEsp => "es-ES";
        public static string LangCodeCat => "es-CA";
        public static string LangCodeEng => "en-UK";
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
}
