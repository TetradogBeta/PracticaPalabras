/*
 * Creado por SharpDevelop.
 * Usuario: PokemonGBAReBuild
 * Fecha: 30/11/2018
 * Hora: 19:30
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MetodeGabarro
{
    /// <summary>
    /// Interaction logic for winDeletreo.xaml
    /// </summary>
    public partial class winDeletreo : Window
    {
        const char LGEMINADA = '&';
        Thread fil;
        string paraula;
        SpeechSynthesizer reader;
        public winDeletreo(string paraula)
        {
            StringBuilder str = new StringBuilder(paraula.ToLower());
            str.Replace("l·l", LGEMINADA + "");
            reader = new SpeechSynthesizer();
            this.paraula = str.ToString();
            InitializeComponent();
            fil = new Thread(() => Deletrea());
            fil.Start();
            
        }

        private void Deletrea()
        {
            string caracterAPosar;
            Action actText;
            Action act = () =>
                 {
                     Hide();
                     for (int i = paraula.Length - 1; i >= 0; i--)
                     {

                         //poso la lletra
                         if ((paraula[i] + "").Equals(LGEMINADA))
                             caracterAPosar = "l·l";
                         else caracterAPosar = "" + paraula[i];

                         actText = () => txtParaulaDeletrejada.Text = caracterAPosar + txtParaulaDeletrejada.Text;
                         Dispatcher.BeginInvoke(actText);
                         System.Threading.Thread.Sleep(1 * 1000);//un segon
                         //dic la lletra
                         Speak(paraula[i]);

                         
                     }
                     if (Window1.SpeakAllWord)
                     {
                         reader.Rate = -1;
                         reader.Speak(paraula);
                     }
                     this.Close();

                 };
            Dispatcher.BeginInvoke(act);
        }

        private void Speak(char caracterADir)
        {
            string textADir;
            switch (char.ToLower(caracterADir))
            {
                case 'ò': textADir = "o amb accent óbert"; break;
                case 'ó': textADir = "o amb accent tancat"; break;
                case 'è': textADir = "e amb accent óbert"; break;
                case 'é': textADir = "e amb accent tancat"; break;
                case 'à': textADir = "a amb accent"; break;
                case 'í': textADir = "i amb accent"; break;
                case 'ú': textADir = "u amb accent"; break;
                case 'ü': textADir = "u amb dieresis"; break;
                case 'ï': textADir = "i amb dieresis"; break;
                case LGEMINADA: textADir = "l geminada"; break;
                case '-': textADir = "guió"; break;
                case '\'': textADir = "apostrof"; break;
                default:
                    textADir = caracterADir + "";
                    break;
            }
            reader.Rate = textADir.Length > 1 ? -2 : 1;
            reader.Speak(textADir);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (fil.IsAlive)
                fil.Abort();
            base.OnClosing(e);
        }
    }
}