using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PracticarPalabras
{
    /// <summary>
    /// Lógica de interacción para winDeletreo.xaml
    /// </summary>
    public partial class winDeletreo : Window
    {
        Task tskDeletreo;
        EstadoDeletreo cancelTask;
        Thread filTencar;
        public winDeletreo(Window parent, Idioma idioma, string paraula)
        {


            InitializeComponent();
            cancelTask = new EstadoDeletreo();
            tskDeletreo = new Task(async () => {

                await idioma.Deletrea(paraula, txtParaulaDeletrejada, cancelTask);


            });
            Left = parent.Left + (parent.Width / 2) - (Width / 2);
            Top = parent.Top + (parent.Height / 2) - (Height / 2);
            tskDeletreo.Start();
            filTencar = new Thread(CerrarAlAcabar);
            filTencar.Start();

        }
        void CerrarAlAcabar()
        {
            Action act = () => { try { this.Close(); } catch { } };
            while (!cancelTask.Acabado && !cancelTask.Cancelado)
                System.Threading.Thread.Sleep(500);
            Dispatcher.BeginInvoke(act);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            cancelTask.Cancelado = true;

            base.OnClosing(e);
        }
    }
}
