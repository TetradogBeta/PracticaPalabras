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
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        Task tskDeletreo;
        CancellationToken cancelTask;
        public winDeletreo(Window parent, Idioma idioma, string paraula)
        {


            InitializeComponent();
            cancelTask = new CancellationToken();
            tskDeletreo=  new Task(() => { idioma.Deletrea(paraula, txtParaulaDeletrejada).RunSynchronously();Dispatcher.BeginInvoke((ActDelegate)(()=>this.Close())); }, cancelTask);
            Left = parent.Left + (parent.Width / 2) - (Width / 2);
            Top = parent.Top + (parent.Height / 2) - (Height / 2);
            tskDeletreo.Start();

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!tskDeletreo.IsCompleted)
                cancelTask.ThrowIfCancellationRequested();
            base.OnClosing(e);
        }
    }
}