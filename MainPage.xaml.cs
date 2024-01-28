﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PracticaPalabrasMAUI;

public partial class MainPage : ContentPage
{
    private string text;
    private int max;
    private int current;

    private Random Random { get; set; }
    public MainPage()
    {

        Random = new Random();
        text = string.Empty;
        InitializeComponent();
        max = Preferences.Get(nameof(Max), 0);
        BindingContext = this;
        
        
   
    }

    public IList<Word> Words { get; set; }


    public int Max { 
        get => max; 
        set { 
            max = value;
            Preferences.Set(nameof(Max), max);
            OnPropertyChanged();
        }
    }
    public int Current { 
        get => current; 
        set { 
            current = value; 
            OnPropertyChanged();
            if (current > max)
            {
                Max = current;
            }
        }
    }


    public string Text { get => text; set { text = value; OnPropertyChanged(); } }

    public Word Actual
    {
        get
        {
            return Resources["actual"] as Word;
        }
        set
        {
            Word actual = Actual;
            if (actual != null)
            {
                actual.Content = value.Content;
                actual.Clue = value.Clue;
            }
        }
    }
    private async Task UpdateWord()
    {

        Word newWorld;
        string uri;

        if (Words.Count > 0)
        {
            do
            {
                newWorld = Words[Random.Next(Words.Count)];
            } while (Words.Count > 1 && newWorld.Content == Actual.Content);
            Actual = newWorld;
        }
        else
        {
            uri = $"{nameof(DictionaryPage)}";
            await Shell.Current.GoToAsync(uri);
        }
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        txtWord.Text = "";
        Words = DictionaryPage.AllWords.ToList();
        await UpdateWord();

    }

    private async void Button_Clicked(object sender = null, EventArgs e = null)
    {
        Word word = Actual;
        string uri;
        if (!Equals(word, null))
        {

            if (word.ToString() == Text.Trim().ToLower())
            {
                Text = "";
                Current++;
                await UpdateWord();
            }
            else
            {
                Current = 0;
                uri = $"{nameof(VisualitzationWordPage)}?{nameof(VisualitzationWordPage.Word)}={word}";
                await Shell.Current.GoToAsync(uri);
            }





        }
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry = (Entry)sender;
        if (entry != null && entry.Text.EndsWith(' '))
        {
            Button_Clicked();
        }
    }
}


public class Word : INotifyPropertyChanged
{
    private string content = string.Empty;
    private string clue = string.Empty;

    public string Clue { 
        get => clue; 
        set {
            clue = value; OnPropertyChanged(); 
        } 
    }
    public string Content { 
        get => content; 
        set { 
            content = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(HiddenContent));
        } 
    }
    public string HiddenContent
    {
        get
        {

            string result = "";

            for (int i = 0; i < Content.Length; i++)
            {
                if (Char.IsUpper(Content[i]))
                {
                    result += "_";
                }
                else
                {
                    result += Content[i];
                }
            }


            return result;
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

