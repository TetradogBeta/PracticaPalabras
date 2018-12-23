using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MetodeGabarro
{
    public delegate void ActDelegate();
    public class Idioma//al final hacer que se pueda serializar
    {

        public static char CaracterAdivina = '_';
        static System.Globalization.CultureInfo RegionEsp = new System.Globalization.CultureInfo("es-es");

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

        public System.Globalization.CultureInfo Region { get; private set; }

        public string[] LetrasCompuestas { get; set; }
        public char[] CaracteresLeft { get; set; }
        public char[] CaracteresRight { get; set; }
        public SortedList<char, char> DicCaracteresOmitirAdivina { get; private set; }
        public SortedList<string, string> PronunciacionEspañola { get; private set; }

        private SpeechSynthesizer Reader { get; set; }

        public Idioma(System.Globalization.CultureInfo region, string[] letrasCompuestas, char[] caracteresLeft, char[] caracteresRight, char[] caracteresOmitirAdivina)
        {
            InstalledVoice voice;
            DicCaracteresOmitirAdivina = new SortedList<char, char>();
            PronunciacionEspañola = new SortedList<string, string>();
            CaracteresLeft = caracteresLeft;
            CaracteresRight = caracteresRight;
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
            for (int i = 0; i < caracteresOmitirAdivina.Length; i++)
                DicCaracteresOmitirAdivina.Add(caracteresOmitirAdivina[i], caracteresOmitirAdivina[i]);
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
                    if (!DicCaracteresOmitirAdivina.ContainsKey(letraCompuestaActual[j]))
                    {
                        strUpperAux = letraCompuestaActual.Substring(0, j);
                        str.Replace(strUpperAux + char.ToUpperInvariant(letraCompuestaActual[j]) + (letraCompuestaActual.Length > j + 1 ? letraCompuestaActual.Substring(j + 1) : ""), caracterAdivinarStr);
                    }
                }
                //pruebo una minus
                for (int j = 0; j < letraCompuestaActual.Length; j++)
                {
                    if (!DicCaracteresOmitirAdivina.ContainsKey(letraCompuestaActual[j]))
                    {
                        strUpperAux = letraCompuestaActual.Substring(0, j);
                        str.Replace(strUpperAux.ToUpperInvariant() + (letraCompuestaActual[j]) + (letraCompuestaActual.Length > j + 1 ? letraCompuestaActual.Substring(j + 1) : "").ToUpperInvariant(), caracterAdivinarStr);
                    }
                }
                //pruebo toda mayus
                str.Replace(letraCompuestaActual.ToUpperInvariant(), caracterAdivinarStr);
            }
            for (int i = 0; i < CaracteresRight.Length; i++)
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == CaracteresRight[i] && str.Length > j + 1 && str[j + 1] == char.ToUpperInvariant(str[j + 1]))
                        str.Replace("" + CaracteresRight[i] + str[j + 1], caracterAdivinarStr);
                }
            for (int i = 0; i < CaracteresLeft.Length; i++)
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == CaracteresLeft[i] && j > 0 && str[j - 1] == char.ToUpperInvariant(str[j - 1]))
                        str.Replace("" + str[j - 1] + CaracteresLeft[i], caracterAdivinarStr);
                }
            for (int i = 0; i < str.Length; i++)
                if (str[i] == char.ToUpperInvariant(str[i]) && !DicCaracteresOmitirAdivina.ContainsKey(str[i]))
                    str.Replace(char.ToUpperInvariant(str[i]) + "", caracterAdivinarStr);

            return str.ToString();

        }
        public async Task Deletrea(string palabra, TextBlock controlTexto, EstadoDeletreo estadoDeletreo)
        {
            Semaphore semaphoreSpell;
            string palabraOri = palabra;
            SortedList<char, string> dicCaracteresCompuestosIn = new SortedList<char, string>();
            SortedList<string, char> dicCaracteresCompuestosOut = new SortedList<string, char>();
            StringBuilder strPalabra = new StringBuilder(palabra.ToLowerInvariant());
            bool pronunciaEsp = Reader.Voice.Culture.Name == RegionEsp.Name;
            string aux;
            for (int i = 0; i < LetrasCompuestas.Length; i++)
            {
                dicCaracteresCompuestosIn.Add((char)i, LetrasCompuestas[i]);
                dicCaracteresCompuestosOut.Add(LetrasCompuestas[i], (char)i);
            }

            for (int i = 0; i < LetrasCompuestas.Length; i++)
                strPalabra.Replace(LetrasCompuestas[i].ToLowerInvariant(), "" + (char)i);
            palabra = strPalabra.ToString();
            semaphoreSpell = new Semaphore(1, 1);
            for (int i = palabra.Length - 1; i >= 0 && !estadoDeletreo.Cancelado; i--)
            {
                aux = dicCaracteresCompuestosIn.ContainsKey(palabra[i]) ? dicCaracteresCompuestosIn[palabra[i]] : palabra[i] + "";

                //muestro la letra compuesta
                controlTexto.Dispatcher.BeginInvoke((ActDelegate)(() => { controlTexto.Text = aux + controlTexto.Text; semaphoreSpell.Release(); }));
                //muestro el caracter

                semaphoreSpell.WaitOne();
                System.Threading.Thread.Sleep(1 * 1000);//1 segundo
                Reader.Rate = aux.Length > 1 ? -2 : 1;


                if (PronunciacionEspañola.ContainsKey(aux))
                {
                    Reader.Speak(PronunciacionEspañola[aux]);
                }
                else
                {
                    Reader.Speak(aux);
                }




            }
            if (SpeakWord && !estadoDeletreo.Cancelado)
            {
                Reader.Rate = -1;
                Reader.Speak(palabraOri);
            }
            estadoDeletreo.Acabado = true;
        }
    }
    public class EstadoDeletreo
    {
        public bool Cancelado { get; set; } = false;
        public bool Acabado { get; set; } = false;
    }
}
