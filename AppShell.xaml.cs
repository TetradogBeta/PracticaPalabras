using Microsoft.Maui.Controls;


namespace PracticaPalabrasMAUI;

public partial class AppShell : Shell
{
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
			if(lang == Language.LangCode)
			{
				lblLang.TextColor =Colors.Blue;
			}
			tabLbl = new TapGestureRecognizer();
			
			tabLbl.Tapped += (s,e) =>
			{
				Label l = s as Label;
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
	}


}
