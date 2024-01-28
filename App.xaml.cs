

namespace PracticaPalabrasMAUI;

public partial class App : Application
{

    public static Page CurrentPage { get; set; }
    public static App CurrentApp { get; set; }

    private ResourceDictionary AntDic { get; set; }
    public App()
	{
  
		InitializeComponent();
        UpdateLang();

        MainPage = new AppShell();

        CurrentApp = this;
        

    }



    public void UpdateLang()
    {
        ResourceDictionary dic;

        switch(Language.LangCode)
        {
            case "es-ES":
                dic =new es_ES();
                break;
            case "es-CA":
                dic = new es_CA();
                break;
            default:
                dic = new en_UK();
                break;
        }
        if(!Equals(AntDic,dic) && !Equals(AntDic, null))
        {
            Current.Resources.MergedDictionaries.Remove(AntDic);
        }
        Current.Resources.MergedDictionaries.Add(dic);
        AntDic = dic;
     
    }
}
