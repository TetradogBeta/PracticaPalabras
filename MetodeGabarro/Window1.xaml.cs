/*
 * Created by SharpDevelop.
 * User: Miquel
 * Date: 20/11/2018
 * Time: 19:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MetodeGabarro
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public const string DICFILE = "file.dic";
        public const string REPEATFILE = "repit.dic";
        public const int INDEXREPEAT = 6;
        public const int INDEXTOTAL = 10;
        public const int REPEATMAX = 5;
        public static char[] CaracteresSplit = new char[] { ';', ',', '/' };
        public static char[] CaracteresSinGuion = new char[] { '·', '-', '\'' };
        public static readonly string FileName = System.IO.Path.Combine(Environment.CurrentDirectory, DICFILE);
        static Random llavor;
        string[] paraulesDic;
        int numParaulesResoltes;

        SortedList<string, int> dicRepetides;
        List<string> lstRepetidas;
        public Window1()
        {
            dicRepetides = new SortedList<string, int>();
            lstRepetidas = new List<string>();
            numParaulesResoltes = 0;
            llavor = new Random();
            InitializeComponent();
            if (System.IO.File.Exists(FileName))
            {
                txtDic.Text = System.IO.File.ReadAllText(FileName);
            
                CarregaParaules();
                SeguentParaula();

            }
            else
            {
                //no hi ha diccionari
                tabDiccionari.IsSelected = true;

            }

            if (System.IO.File.Exists(REPEATFILE))
            {
                lstRepetidas.AddRange(System.IO.File.ReadAllLines(REPEATFILE));
                for (int i = lstRepetidas.Count - 1; i >= 0; i--)
                    if (!dicRepetides.ContainsKey(lstRepetidas[i]))
                        dicRepetides.Add(lstRepetidas[i], 0);
                    else lstRepetidas.RemoveAt(i);
            }
        }
        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDic.Text))
                System.IO.File.WriteAllText(FileName, txtDic.Text);
            else System.IO.File.Delete(FileName);
            if (lstRepetidas.Count > 0)
            {
                System.IO.File.WriteAllLines(REPEATFILE, lstRepetidas.ToArray());
            }
            else
            {
                System.IO.File.Delete(REPEATFILE);
            }
        }

        void SeguentParaula()
        {
            int randomPos;
            string[] camps = null;
            string palabra = null;
            do
            {
                if (lstRepetidas.Count > 0 && llavor.Next(0, INDEXTOTAL) < INDEXREPEAT)
                {
                    randomPos = llavor.Next(lstRepetidas.Count);
                    palabra = DameParaulaDic(lstRepetidas[randomPos]);
                    if (palabra != null)
                        camps = palabra.Split(CaracteresSplit);
                    else
                    {
                        dicRepetides.Remove(palabra);
                        lstRepetidas.RemoveAt(randomPos);
                    }
                }
                else
                {
                    randomPos = llavor.Next(paraulesDic.Length);
                    palabra = paraulesDic[randomPos];
                    camps = palabra.Split(CaracteresSplit);
                }

            } while (palabra == null || tbParaula.Text.Equals(camps[0]));
            txtPista.Text = camps[1];
            tbParaula.Tag = camps[0].ToLower();
            tbParaula.Text = "";
            for (int i = 0; i < camps[0].Length; i++)
                if (!Char.IsLower(camps[0][i]) && !CaracteresSinGuion.Contains(camps[0][i]))//si es mayuscula o un caracter especial
                    tbParaula.Text += '_';
                else tbParaula.Text += camps[0][i];


        }

        private string DameParaulaDic(string palabra)
        {
            string paraulaDic = null;
            for (int i = 0; i < paraulesDic.Length && paraulaDic == null; i++)
                if (paraulesDic[i].Split(CaracteresSplit)[0].ToUpper().Equals(palabra))
                    paraulaDic = paraulesDic[i];
            return paraulaDic;
        }

        void CarregaParaules()
        {
            paraulesDic = txtDic.Text.Trim('\r', '\n').Replace('\r', ' ').Split('\n');


        }
        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                MostrarResposta();
        }

        private void MostrarResposta()
        {
            if (tbParaula.Tag != null)
                tbParaula.Text = tbParaula.Tag.ToString().ToUpper();
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtDic != null && !string.IsNullOrEmpty(txtDic.Text))
            {
                CarregaParaules();
                SeguentParaula();

            }
        }

        private void txtParaulaUser_KeyDown(object sender, KeyEventArgs e)
        {
            string resposta;
            if (e.Key == Key.Enter)
            {
                resposta = tbParaula.Tag.ToString().ToUpper();
                if (resposta.Equals(txtParaulaUser.Text.ToUpper()))
                {
                    SeguentParaula();
                    numParaulesResoltes++;
                    Title = string.Format("Paraules Resoltes {0}", numParaulesResoltes);
                    txtParaulaUser.Text = "";
                    if (dicRepetides.ContainsKey(resposta))
                    {
                        dicRepetides[resposta]++;
                        if (dicRepetides[resposta] >= REPEATMAX)
                        {
                            dicRepetides.Remove(resposta);
                            lstRepetidas.Remove(resposta);
                        }
                    }
                }
                else
                {
                    //se ha equivocado
                    MessageBox.Show("Error, paraula incorrecte");
                    MostrarResposta();
                    if (!dicRepetides.ContainsKey(resposta))
                    {
                        dicRepetides.Add(resposta, 0);
                        lstRepetidas.Add(resposta);
                    }
                    else
                    {
                        dicRepetides[resposta] -= 2;
                    }
                }
            }
        }
    }
}