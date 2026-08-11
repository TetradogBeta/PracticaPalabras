using System.Text;

namespace PracticaPalabras;

public class Speak
{
    public static bool SpeakAllWord
    {
        get => PreferencesService.Get(nameof(SpeakAllWord), true);
        set => PreferencesService.Set(nameof(SpeakAllWord), value);
    }

    public string LangCode { get; set; } = string.Empty;

    public SortedList<string, string> DicComplexPronuntiation { get; private set; } = new();

    public string ToHidden(string word)
    {
        IList<string> complex = DicComplexPronuntiation.Keys;
        StringBuilder wordDigestedBl = new(word);
        StringBuilder result = new();

        for (int i = 0; i < complex.Count; i++)
        {
            if (word.Contains(complex[i].ToUpper()))
            {
                wordDigestedBl.Replace(complex[i].ToUpper(), "_");
            }
        }

        for (int i = 0; i < wordDigestedBl.Length; i++)
        {
            if (char.IsUpper(wordDigestedBl[i]))
            {
                result.Append('_');
            }
            else
            {
                result.Append(wordDigestedBl[i]);
            }
        }
        return result.ToString();
    }

    public async Task Read(string word, IReadController page, ISpeechService speech)
    {
        List<string> list = new();
        List<string> complexList = new();
        IList<string> complex = DicComplexPronuntiation.Keys;
        StringBuilder wordDigestedBl = new(word);
        string wordDigested;
        string toSpeak;

        for (int i = 0; i < complex.Count; i++)
        {
            if (word.Contains(complex[i].ToUpper()))
            {
                wordDigestedBl.Replace(complex[i].ToUpper(), list.Count.ToString());
                list.Add(DicComplexPronuntiation[complex[i]]);
                complexList.Add(complex[i]);
            }
        }
        page.SetWord(string.Empty);
        wordDigested = wordDigestedBl.ToString();

        for (int i = wordDigested.Length - 1; i >= 0 && page.CanContinue; i--)
        {
            if (char.IsDigit(wordDigested[i]))
            {
                int idx = int.Parse(wordDigested[i].ToString());
                toSpeak = list[idx];
                page.SetWord(complexList[idx].ToUpper() + page.GetWord());
            }
            else
            {
                toSpeak = wordDigested[i].ToString();
                page.SetWord(wordDigested[i] + page.GetWord());
            }

            if (page.CanContinue)
            {
                await speech.SpeakAsync(toSpeak, page.CancellationToken);
                await Task.Delay(1000, page.CancellationToken);
            }
        }

        if (page.CanContinue && SpeakAllWord)
        {
            await speech.SpeakAsync(word, page.CancellationToken);
        }
    }
}

public interface IReadController
{
    bool CanContinue { get; }
    CancellationToken CancellationToken { get; }
    string GetWord();
    void SetWord(string value);
}