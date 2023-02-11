using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Devices.Sensors;
using MauiGeo.ViewModel;

namespace MauiGeo;

public partial class FollowersPage : ContentPage
{
    private FollowersPageViewModel viewModel;
    public FollowersPage(FollowersPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadFollowers();
    }

}

