using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MetodeGabarro
{
    public class Idioma//al final hacer que se pueda serializar
    {
        public const char CARACTERAUX = '&';//y un numero que indica que letra compuesta es

        
        public static char CaracterAdivina = '_';
        static System.Globalization.CultureInfo RegionEsp= new System.Globalization.CultureInfo("es-es");

        public static bool SpeakWord
        {
            get
            {

                return Properties.Settings.Default.SpeakAllWord;
            }
            set
            {
                Properties.Settings.Default.SpeakAllWord = value;
                Properties.Settings.Default.Save();
            }
        }

        public System.Globalization.CultureInfo Region { get; set; }
        public string[] LetrasCompuestas { get; set; }
        public char[] CaracteresLeft { get; set; }
        public char[] CaracteresRight { get; set; }
        public SortedList<string, string> PronunciacionEspañola { get; private set; }

        private SpeechSynthesizer Reader { get; set; }

        public Idioma(System.Globalization.CultureInfo region,string[] letrasCompuestas, char[] caracteresLeft, char[] caracteresRight)
        {
            InstalledVoice voice;

            PronunciacionEspañola = new SortedList<string, string>();
            CaracteresLeft = caracteresLeft;
            CaracteresRight=caracteresRight;
            LetrasCompuestas = letrasCompuestas;
            Region = region;

            
            Reader = new SpeechSynthesizer();
            voice = Reader.GetInstalledVoices(Region).FirstOrDefault();
            if (voice != null && voice.Enabled)
                Reader.SelectVoice(voice.VoiceInfo.Name);
            else
            {
                voice = Reader.GetInstalledVoices(RegionEsp).FirstOrDefault();
                if (voice != null)
                    Reader.SelectVoice(voice.VoiceInfo.Name);
            }
        }
        public string StringAdivina(string palabra)
        {
            string caracterAdivinarStr = CaracterAdivina + "";
            StringBuilder str = new StringBuilder(palabra);
            string letraCompuestaActual;
            string strUpperAux;

            for (int i = 0; i < LetrasCompuestas.Length; i++)
            {
                letraCompuestaActual = LetrasCompuestas[i].ToLowerInvariant();
                //pruebo una mayus
                for (int j = 0; j < letraCompuestaActual.Length; j++)
                {
                    strUpperAux = letraCompuestaActual.Substring(0, i);
                    str.Replace(strUpperAux + char.ToUpperInvariant(letraCompuestaActual[i]) + (letraCompuestaActual.Length > i + 1 ? letraCompuestaActual.Substring(i + 1) : ""), caracterAdivinarStr);
                }
                //pruebo una minus
                for (int j = 0; j < letraCompuestaActual.Length; j++)
                {
                    strUpperAux = letraCompuestaActual.Substring(0, i);
                    str.Replace(strUpperAux.ToUpperInvariant() + (letraCompuestaActual[i]) + (letraCompuestaActual.Length > i + 1 ? letraCompuestaActual.Substring(i + 1) : "").ToUpperInvariant(), caracterAdivinarStr);
                }
                //pruebo toda mayus
                str.Replace(letraCompuestaActual.ToUpperInvariant(), caracterAdivinarStr);
            }
            for (int i = 0; i < CaracteresRight.Length; i++)
                for (int j = 0; j < str.Length; i++)
                {
                    if (str[j] == CaracteresRight[i] && str.Length > j + 1 && str[j + 1] == char.ToUpperInvariant(str[j + 1]))
                        str.Replace("" + CaracteresRight[i] + str[j + 1], caracterAdivinarStr);
                }
            for (int i = 0; i < CaracteresLeft.Length; i++)
                for (int j = 0; j < str.Length; i++)
                {
                    if (str[j] == CaracteresLeft[i] && j > 0 && str[j - 1] == char.ToUpperInvariant(str[j - 1]))
                        str.Replace("" + str[j - 1] + CaracteresLeft[i], caracterAdivinarStr);
                }
            return str.ToString();

        }
        public async Task Deletrea(string palabra,TextBox controlTexto)
        {

        }
    }
}
