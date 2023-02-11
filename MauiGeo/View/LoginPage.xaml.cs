using CommunityToolkit.Mvvm.Messaging;
using MauiGeo.Services;
using MauiGeo.ViewModel;

namespace MauiGeo;

public partial class LoginPage : ContentPage
{
	public LoginPageViewModel LoginPageViewModel { get; }

	public LoginPage(LoginPageViewModel loginPageViewModel)
	{
		InitializeComponent();
		BindingContext = loginPageViewModel;
		LoginPageViewModel = loginPageViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();


		LoginPageViewModel.CheckLogin();
		await LoginPageViewModel.LoadLoginDetails();
	}

}
