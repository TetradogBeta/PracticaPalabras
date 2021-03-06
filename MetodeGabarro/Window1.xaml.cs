﻿/*
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
using System.Linq;
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
        public static char[] CaracteresSplit = new char[] { ';', '|', '/' };

        static SortedList<string, Idioma> DicIdiomas;
        static readonly Idioma IdiomaPorDefecto;

        public static readonly string FileName = System.IO.Path.Combine(Environment.CurrentDirectory, DICFILE);
        static Random llavor;
        string[] paraulesDic;
        int numParaulesResoltes;

        SortedList<string, int> dicRepetides;
        List<string> lstRepetidas;

        static Window1()
        {
            Idioma idioma;
            DicIdiomas = new SortedList<string, Idioma>();
            //añado el idioma catalan
            idioma = new Idioma(new System.Globalization.CultureInfo("ca-ES"), new string[] { "l·l", "ny", "ss" }, new char[] { ' ' }, new char[] { '-', '\'' },new char[] { '·'});

            idioma.PronunciacionEspañola.Add("ò", "o amb accent ubert");
            idioma.PronunciacionEspañola.Add("ó", "o amb accent tancat");
            idioma.PronunciacionEspañola.Add("è", "e amb accent ubert");
            idioma.PronunciacionEspañola.Add("é", "e amb accent tancat");
            idioma.PronunciacionEspañola.Add("à", "a amb accent");
            idioma.PronunciacionEspañola.Add("í", "i amb accent");
            idioma.PronunciacionEspañola.Add("ú", "u amb accent");
            idioma.PronunciacionEspañola.Add("ü", "u amb dieresis");
            idioma.PronunciacionEspañola.Add("ï", "i amb dieresis");
            idioma.PronunciacionEspañola.Add("l·l", "l geminada");
            idioma.PronunciacionEspañola.Add("ny", "eña");
            idioma.PronunciacionEspañola.Add("-", "guió");
            idioma.PronunciacionEspañola.Add("\"", "apostrof");
            idioma.PronunciacionEspañola.Add(" ", "espai");
            idioma.PronunciacionEspañola.Add("ç", "sé trancada");
            IdiomaPorDefecto = idioma;
            DicIdiomas.Add(IdiomaPorDefecto.Region.Name, IdiomaPorDefecto);
            idioma = new Idioma(new System.Globalization.CultureInfo("es-ES"), new string[] { }, new char[] { }, new char[] { }, new char[] { });
 
            idioma.PronunciacionEspañola.Add("ó", "o con tilde");
            idioma.PronunciacionEspañola.Add("é", "e con tilde ");
            idioma.PronunciacionEspañola.Add("à", "a con tilde");
            idioma.PronunciacionEspañola.Add("í", "i con tilde");
            idioma.PronunciacionEspañola.Add("ú", "u con tilde");
            idioma.PronunciacionEspañola.Add("ü", "u con dieresis");
            idioma.PronunciacionEspañola.Add("ï", "i con dieresis");

            DicIdiomas.Add(idioma.Region.Name, idioma);
        }
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
            if (paraulesDic != null && paraulesDic.Length > 0)
                SeguentParaula();
        }


        public Idioma Idioma
        {
            get { return DicIdiomas.ContainsKey(System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.Name) ? DicIdiomas[System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.Name] : IdiomaPorDefecto; }
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
            string aux;
            StringBuilder str = new StringBuilder();
            do
            {
                if (lstRepetidas.Count > 0 && llavor.Next(0, INDEXTOTAL) < INDEXREPEAT)
                {
                    randomPos = llavor.Next(lstRepetidas.Count);
                    palabra = DameParaulaDic(lstRepetidas[randomPos]);
                    if (palabra != null)
                    {
                        if (palabra.Intersect(CaracteresSplit).ToArray().Length > 0)
                            camps = palabra.Split(CaracteresSplit);
                        else camps = new string[] { palabra, "" };
                    }
                    else if (palabra != null && dicRepetides.ContainsKey(palabra))
                    {
                        dicRepetides.Remove(palabra);
                        lstRepetidas.RemoveAt(randomPos);
                    }
                }
                else
                {
                    randomPos = llavor.Next(paraulesDic.Length);
                    palabra = paraulesDic[randomPos];
                    if (palabra.Intersect(CaracteresSplit).ToArray().Length > 0)
                        camps = palabra.Split(CaracteresSplit);
                    else camps = new string[] { palabra, "" };
                }

            } while (palabra == null || tbParaula.Text.Equals(camps[0]) && paraulesDic.Length > 1);
            txtPista.Text = camps[1];
            tbParaula.Tag = camps[0].ToLower();
            tbParaula.Text = Idioma.StringAdivina(camps[0]);


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
            else if (e.Key == Key.F6)
            {
                Idioma.SpeakWord = !Idioma.SpeakWord;
                MessageBox.Show(this,string.Format("S'ha {0} dir la paraula completa al finalitzar el deletreig", Idioma.SpeakWord ? "activat" : "desactivat"));
            }
        }

        private void MostrarResposta()
        {
            if (tbParaula.Tag != null)
                tbParaula.Text = tbParaula.Tag.ToString().ToUpper();
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabDiccionari != null && !tabDiccionari.IsSelected && txtDic != null && !string.IsNullOrEmpty(txtDic.Text))
            {
                CarregaParaules();
                SeguentParaula();
                
            }
            Title = string.Format("Paraules Resoltes {0} Idioma {1}", numParaulesResoltes, Idioma.Region.NativeName.Substring(0, Idioma.Region.NativeName.IndexOf('(')));
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
                    MessageBox.Show(this, "Error, paraula incorrecte");


                    do
                    {
                        MessageBox.Show(this, resposta, "Visualitza");
                        //deletrearlo!
                        new winDeletreo(this, Idioma, resposta).ShowDialog();
                    } while (MessageBox.Show(this, "L'has visualitzat, correctament?", "", MessageBoxButton.YesNo) == MessageBoxResult.No);


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