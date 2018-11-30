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
        Thread fil;
        string paraula;
        SpeechSynthesizer reader;
        public winDeletreo(string paraula)
        {
            reader = new SpeechSynthesizer();
            this.paraula = paraula;
            InitializeComponent();
            fil = new Thread(() => Deletrea());
            fil.Start();
        }

        private void Deletrea()
        {
            Action act;
            for (int i = paraula.Length - 1; i >= 0; i--)
            {
                act = () =>
                {
                    //poso la lletra
                    txtParaulaDeletrejada.Text = paraula[i] + txtParaulaDeletrejada.Text;
                    //dic la lletra
                    reader.Speak(paraula[i] + "");
                  
                };
                Dispatcher.BeginInvoke(act);
                System.Threading.Thread.Sleep(2* 1000);//un segon
            }
            reader.Speak(paraula);
            System.Threading.Thread.Sleep(3 * 1000);//tres segon
            act=()=>this.Close();
            Dispatcher.BeginInvoke(act);
        }
    }
}