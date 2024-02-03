using System.Text;


namespace PracticaPalabrasMAUI
{
    public class Speak
    {
        public static bool SpeakAllWord { 
            get => Preferences.Get(nameof(SpeakAllWord), true);
            set => Preferences.Set(nameof(SpeakAllWord), value);
        }

        public string LangCode { get; set; }
        public SortedList<string, string> DicComplexPronuntiation { get;private set; }=new SortedList<string, string>();



        public async Task Read(string word, VisualitzationWordPage page)
        {
            //l·l,ny,ó,à
            //word:piranya -> pira0a
            List<string> list = new();//ny,l·l en el orden de aparición
            List<string> complexList = new();
            IList<string> complex = DicComplexPronuntiation.Keys;
            StringBuilder wordDigestedBl= new(word);
            string wordDigested;
            string toSpeak;

        

            for(int i=0;i<complex.Count;i++)
            {
                if (word.Contains(complex[i].ToUpper()))
                {
                    wordDigestedBl.Replace(complex[i].ToUpper(), list.Count.ToString());//suponiendo que no superan los 10 caracteres complejos
                    list.Add(DicComplexPronuntiation[complex[i]]);
                    complexList.Add(complex[i]);
                    
                }
            }
            page.Word = "";
            wordDigested= wordDigestedBl.ToString();
            for(int i = wordDigested.Length - 1; i >= 0 && page.Can; i--)
            {
                if (char.IsDigit(wordDigested[i]))
                {
                    toSpeak = list[int.Parse(wordDigested[i]+"")];
                    page.Word = complexList[int.Parse(wordDigested[i]+"")].ToUpper() + page.Word;
                }
                else
                {
                    toSpeak = wordDigested[i] + "";
                    page.Word = wordDigested[i] + page.Word;
                }
                if (page.Can)
                {
                    await TextToSpeech.SpeakAsync(toSpeak);
                    await Task.Delay(1000);
                }
            }
            if(page.Can && SpeakAllWord)
            {
                await TextToSpeech.SpeakAsync(word);
            }
        }
    }
}
