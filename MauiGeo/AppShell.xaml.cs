using CommunityToolkit.Mvvm.Messaging;
using MauiGeo.ViewModel;

namespace MauiGeo;

public partial class AppShell : Shell
{
    public LoginPageViewModel viewModel { get; set; }

    public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(BeaconPage), typeof(BeaconPage));
        Routing.RegisterRoute(nameof(FollowersPage), typeof(FollowersPage));
        Routing.RegisterRoute(nameof(TrackingPage), typeof(TrackingPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        SendLogoutMessage();
        //await Shell.Current.GoToAsync("//LoginPage");
    }

    private void SendLogoutMessage()
    {
        WeakReferenceMessenger.Default.Send(new LoginStatusMessage(false));
    }
}
