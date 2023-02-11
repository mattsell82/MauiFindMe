using CommunityToolkit.Mvvm.Messaging;
using MauiGeo.ViewModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Security.Cryptography.X509Certificates;

namespace MauiGeo;

public partial class TrackingPage : ContentPage, IRecipient<LoginStatusMessage>
{
    private TrackingPageViewModel viewModel;

    //TODO: Flytta timern till viewmodel istället.
    IDispatcherTimer timer;

    public TrackingPage(TrackingPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        WeakReferenceMessenger.Default.Register<LoginStatusMessage>(this);

        timer = this.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(5000);
        timer.Tick += OnDispatcherTimer;
        timer.IsRepeating = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetTimerStatus();
    }

    private void SetTimerStatus()
    {
        viewModel.IsTimerRunning = timer.IsRunning;
        System.Diagnostics.Debug.WriteLine($"Traking timer status: {viewModel.IsTimerRunning}");
    }


    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

/*        timer.Stop();
        myMap.Pins.Clear();
        viewModel.Locations.Clear();
        viewModel.CurrentLocation = null;
        viewModel.SelectedUser = null;*/
    }

    private async void userPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (timer.IsRunning)
        {
            timer.Stop();
            SetTimerStatus();
        }

        myMap.Pins.Clear();
        await viewModel.GetLocations();

        if (viewModel.Locations.Count == 0)
        {
            await Shell.Current.DisplayAlert("Info", "There are no locations to display", "Ok");
            return;
        }

        DrawTrack();
    }

    private void DrawTrack()
    {
        myMap.Pins.Clear();
        myMap.MapType = MapType.Satellite;
        myMap.IsShowingUser = true;

        var startLocation = viewModel.Locations[0];

        MapSpan mapSpan = MapSpan.FromCenterAndRadius(startLocation, Distance.FromKilometers(3));
        myMap.MoveToRegion(mapSpan);

        foreach (var item in viewModel.Locations)
        {
            myMap.Pins.Add(new Pin
            {
                Label = item.Timestamp.ToString(),
                Address = item.Accuracy.ToString(),
                Location = item
            });
        }
    }

    /*    private async void Button_Clicked(object sender, EventArgs e)
        {
            if (viewModel.SelectedUser is null)
            {
                await Shell.Current.DisplayAlert("Info", "A user must first be selected.", "Ok");
                return;
            }

            if (viewModel.CurrentLocation is null)
            {
                await Shell.Current.DisplayAlert("Info", "There are no locations to display", "Ok");
                return;
            }

            var current = viewModel.CurrentLocation.Timestamp.ToString();

            while (true)
            {
                await viewModel.GetLocations();

                if (current != viewModel.CurrentLocation.ToString())
                {
                    current = viewModel.CurrentLocation.ToString();
                    PinCurrentLocation();
                }

                await Task.Delay(5000);
            }
        }*/

    private async void Button_StopTimer_Clicked(object sender, EventArgs e)
    {
        timer.Stop();
        SetTimerStatus();
        await Shell.Current.DisplayAlert("Info", "Live tracking stopped", "Ok");
    }

    private async void Button_StartTimer_Clicked(object sender, EventArgs e)
    {
        if (viewModel.SelectedUser is null)
        {
            await Shell.Current.DisplayAlert("Info", "A user must first be selected.", "Ok");
            return;
        }

        if (viewModel.CurrentLocation is null)
        {
            await Shell.Current.DisplayAlert("Info", "There are no locations to display", "Ok");
            return;
        }

        timer.Start();
        SetTimerStatus();
        await Shell.Current.DisplayAlert("Info", "Live tracking started", "Ok");
    }

    private void Button_Center_Clicked(object sender, EventArgs e)
    {
        if (viewModel.CurrentLocation is null) { return; }

        MapSpan mapSpan = MapSpan.FromCenterAndRadius(viewModel.CurrentLocation, Distance.FromKilometers(0.5));
        myMap.MoveToRegion(mapSpan);
    }

    void OnDispatcherTimer(object sender, EventArgs e)
    {
        Task.Run(viewModel.GetLocations).Wait();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            PinCurrentLocation();
        });


        //await Task.Run(viewModel.GetLocations);
        //await viewModel.GetLocations();
        System.Diagnostics.Debug.WriteLine("Live Updating.");
    }

    private void PinCurrentLocation()
    {
        myMap.Pins.Clear();

        myMap.Pins.Add(new Pin
        {
            Label = viewModel.CurrentLocation.Timestamp.ToString(),
            Location = viewModel.CurrentLocation,
            Address = viewModel.CurrentLocation.Accuracy.ToString(),
        });

/*        MapSpan mapSpan = MapSpan.FromCenterAndRadius(viewModel.CurrentLocation, Distance.FromKilometers(0.2));
        myMap.MoveToRegion(mapSpan);*/
    }

    private void mapPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (viewModel.SelectedMapType is null)
        {
            viewModel.SelectedMapType = "Satellite";
        }

        switch (viewModel.SelectedMapType)
        {
            case "Satellite":
                myMap.MapType = MapType.Satellite;
                break;

            case "Street":
                myMap.MapType = MapType.Street;
                break;

            case "Hybrid":
                myMap.MapType = MapType.Hybrid;
                break;


            default:
                myMap.MapType = MapType.Satellite;
                break;
        }
    }

    public void Receive(LoginStatusMessage message)
    {
        if (!message.Value)
        {
            System.Diagnostics.Debug.WriteLine("Logout message recived: Stopping timer.");
            timer.Stop();
            SetTimerStatus();
            myMap.Pins.Clear();
        }

    }
}

