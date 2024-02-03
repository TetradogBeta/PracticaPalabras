using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaPalabrasMAUI
{
    public class Speak
    {

        public string LangCode { get; set; }
        public SortedList<string, string> DicComplexPronuntiation { get;private set; }=new SortedList<string, string>();
        public bool Can {  get; set; }

        public bool SpeakAllWord { get; set; } = true;

        public async Task Read(string word, VisualitzationWordPage page)
        {
            //l·l,ny,ó,à
            //word:piranya -> pira0a
            List<string> list = new List<string>();//ny,l·l en el orden de aparición
            List<string> complexList = new List<string>();
            IList<string> complex = DicComplexPronuntiation.Keys;
            StringBuilder wordDigestedBl= new StringBuilder(word);
            string wordDigested;
            string toSpeak;

            Can = true;

            for(int i=0;i<complex.Count;i++)
            {
                if (word.Contains(complex[i]))
                {
                    wordDigestedBl.Replace(complex[i], list.Count.ToString());//suponiendo que no superan los 10 caracteres complejos
                    list.Add(DicComplexPronuntiation[complex[i]]);
                    complexList.Add(complex[i]);
                    
                }
            }
            page.Word = "";
            wordDigested= wordDigestedBl.ToString();
            for(int i = wordDigested.Length - 1; i >= 0 && Can; i--)
            {
                if (char.IsDigit(wordDigested[i]))
                {
                    toSpeak = list[int.Parse(wordDigested[i]+"")];
                    page.Word = complexList[int.Parse(wordDigested[i]+"")] + page.Word;
                }
                else
                {
                    toSpeak = wordDigested[i] + "";
                    page.Word = wordDigested[i] + page.Word;
                }
                if (Can)
                {
                    await TextToSpeech.Default.SpeakAsync(toSpeak);
                    await Task.Delay(1000);
                }
            }
            if(Can && SpeakAllWord)
            {
                await TextToSpeech.Default.SpeakAsync(word);
            }
        }
    }
}
