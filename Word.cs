using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PracticaPalabrasMAUI;

public class Word : INotifyPropertyChanged
{
    private string content = string.Empty;
    private string clue = string.Empty;

    private Speak speak = new Speak();

    public string Clue {
        get => clue;
        set {
            clue = value; OnPropertyChanged();
        }
    }
    public string Content {
        get => content;
        set {
            content = value.Trim();
            OnPropertyChanged();
            OnPropertyChanged(nameof(HiddenContent));
        }
    }

    public string ToSaveString => $"{Content};{Clue}";
    public string HiddenContent {
        get {  
            speak.Config(); 
            return speak.ToHidden(Content); 
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;


 
    public override string ToString()
    {
        return Content.ToLower();
    }
    
    
    private void OnPropertyChanged([CallerMemberName] string name=null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    public static Word FromLine(string line)
    {

        string[] fields =line.Contains(';')? line.Split(';'):new string[] { line };
        return new Word() { Content = fields[0], Clue = fields.Length > 1 ? fields[1]:string.Empty };
    }
}

