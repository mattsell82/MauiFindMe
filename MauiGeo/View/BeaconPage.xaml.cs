using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Devices.Sensors;
using MauiGeo.ViewModel;

namespace MauiGeo;

public partial class BeaconPage : ContentPage
{

    public BeaconPage(BeaconPageViewModel beaconPageViewModel)
    {
        InitializeComponent();
        BindingContext = beaconPageViewModel;
    }




}

