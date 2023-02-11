using MauiGeo.ViewModel;

namespace MauiGeo;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
    }

}
