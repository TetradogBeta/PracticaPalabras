using Microsoft.Maui.Controls;

namespace PracticaPalabrasMAUI;

public partial class AppShell : Shell
{
    private bool notIsActualLang;

    //tengo que sacar los calores del Resources

    static  Color ActualDic => Colors.Orange;
	static Color DefaultColor => Colors.Gray;
	static Color ActualUI => Colors.Blue;



	string LangCodeAnt { get; set; }
	public AppShell()
	{
		Label lblLang=null;
        TapGestureRecognizer tabLbl;
		InitializeComponent();
		BindingContext= this;
		
		foreach(string lang in Language.LangCodes)
		{
			lblLang = new Label()
			{
				Text = lang,
			};
		
			tabLbl = new TapGestureRecognizer();
			
			tabLbl.Tapped += (s,e) =>
			{
				Label l = s as Label;

				if (Equals(LangCodeAnt,null))
				{
					LangCodeAnt = Language.LangCode;
				}
			
				UpdateLangLabelColor(l.Text);
				

				Language.LangCode = l.Text;
				
              
            };
			lblLang.GestureRecognizers.Add(tabLbl);
			lstLangs.Add(lblLang);
            lblLang = new Label()
            {
                Text = " | ",
            };
            lstLangs.Add(lblLang);
        }
		if (lblLang != null)
		{
			lstLangs.Remove(lblLang);
		}
		UpdateLangLabelColor(Language.LangCode);
		
	}

    public bool NotIsActualLang
    {
        get => notIsActualLang; set { notIsActualLang = value;OnPropertyChanged(); OnPropertyChanged(nameof(IsActualLang)); }
    }
    public bool IsActualLang
    {
        get => !NotIsActualLang; set { NotIsActualLang = !value;  }
    }
    private void UpdateLangLabelColor(string langActual)
    {
		bool actualLangSelected;
        foreach (Label l in lstLangs.Children.OfType<Label>())
		{
			if(l.Text == langActual)
			{
				actualLangSelected = Equals(LangCodeAnt, null) || LangCodeAnt == langActual;

                if (actualLangSelected)
				{
					l.TextColor = ActualUI;
				}
				else { 

					l.TextColor = ActualDic;
				}
				NotIsActualLang = !actualLangSelected;


            }
            else if (Language.LangCodes.Contains(l.Text) && LangCodeAnt != l.Text)
			{
				l.TextColor = DefaultColor;
			}
		}
    }
}
