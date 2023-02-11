using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MauiGeo.Services;
using MauiGeo.Model;
using System.Collections.ObjectModel;

namespace MauiGeo.ViewModel;

public partial class TrackingPageViewModel : ObservableObject, IRecipient<LoginStatusMessage>
{
    public ObservableCollection<UserName> Following { get; } = new();

    public ObservableCollection<Location> Locations { get; } = new();

    [ObservableProperty]
    string selectedMapType;

    public List<string> MapTypes { get; } = new List<string> { "Satellite", "Hybrid", "Street" };

    [ObservableProperty]
    Location currentLocation;

    [ObservableProperty]
    UserName selectedUser;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTimerNotRunning))]
    bool isTimerRunning;

    public bool IsTimerNotRunning => !IsTimerRunning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    bool isLoading;

    public bool IsNotLoading => !IsLoading;


    private ApiService apiService;

    public TrackingPageViewModel(ApiService apiService)
    {
        this.apiService = apiService;
        WeakReferenceMessenger.Default.Register<LoginStatusMessage>(this);
    }

    public async Task GetLocations()
    {
        IsLoading = true;

        try
        {
            if (SelectedUser is null)
                return;

            var result = await apiService.FriendLocations(SelectedUser);

            if (result is null)
                return;

            Locations.Clear();

            result = result.OrderByDescending(x => x.TimeStamp).ToList();

            currentLocation = new Location
            {
                Accuracy = result[0].Accuracy,
                Altitude = result[0].Altitude,
                Course = result[0].Course,
                IsFromMockProvider = result[0].IsFromMockProvider is null ? false : (bool)result[0].IsFromMockProvider,
                Latitude = result[0].Latitude,
                Longitude = result[0].Longitude,
                ReducedAccuracy = result[0].ReducedAccuracy is null ? false : (bool)result[0].ReducedAccuracy,
                Timestamp = result[0].TimeStamp,
                VerticalAccuracy = result[0].VerticalAccuracy
            };

            foreach (var item in result)
            {
                System.Diagnostics.Debug.WriteLine(item.TimeStamp.ToString());

                Locations.Add(new Location
                {
                    Accuracy = item.Accuracy,
                    Altitude = item.Altitude,
                    Course = item.Course,
                    IsFromMockProvider = item.IsFromMockProvider is null ? false : (bool)item.IsFromMockProvider,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    ReducedAccuracy = item.ReducedAccuracy is null ? false : (bool)item.ReducedAccuracy,
                    Timestamp = item.TimeStamp,
                    VerticalAccuracy = item.VerticalAccuracy
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to get locations: " + ex.Message);
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            IsLoading = false;
        }
    }


    [RelayCommand]
    public async Task LoadUsers()
    {
        IsLoading = true;
        try
        {
            var result = await apiService.Following();

            Following.Clear();

            foreach (var item in result)
            {
                Following.Add(item);
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to Load users: " + ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Receive(LoginStatusMessage message)
    {
        System.Diagnostics.Debug.WriteLine("Logout message recived: Clearing trackingpage viewmodel.");
        Following.Clear();
        Locations.Clear();
        SelectedUser = null;
        CurrentLocation = null;
    }
}
