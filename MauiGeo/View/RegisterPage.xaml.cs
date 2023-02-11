using MauiGeo.Services;
using MauiGeo.ViewModel;

namespace MauiGeo;

public partial class RegisterPage : ContentPage
{
	public RegisterPageViewModel ViewModel { get; }

	public RegisterPage(RegisterPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		this.ViewModel = viewModel;
	}

}
