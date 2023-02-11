using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MauiGeo.Services;

namespace MauiGeo.ViewModel;

public partial class BeaconPageViewModel : 
    ObservableObject, 
    IRecipient<LocationUpdatedMessage>, 
    IRecipient<LoginStatusMessage>
{
    [ObservableProperty]
    int successfulApiCalls;

    [ObservableProperty]
    int failedApiCalls;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotRunning))]
    bool isRunning;

    public bool IsNotRunning => !IsRunning;

    [ObservableProperty]
    Location lastLocation;

    private ApiService apiService;
    private IForegroundService foregroundService;

    public BeaconPageViewModel(IForegroundService foregroundService, ApiService apiService)
    {
        WeakReferenceMessenger.Default.Register<LoginStatusMessage>(this);
        WeakReferenceMessenger.Default.Register<LocationUpdatedMessage>(this);
        this.apiService = apiService;
        this.foregroundService = foregroundService;
    }


    [RelayCommand]
    public async Task StartLocationService()
    {
        if (!apiService.CheckLogin())
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        foregroundService.Start();
        IsRunning = true;
    }

    [RelayCommand]
    public void StopLocationService()
    {
        foregroundService.Stop();
        IsRunning = false;
    }


    //When a new message about updated location arrives we try to send it to the web-api.
    public void Receive(LocationUpdatedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

            var timeStampAsLocalTimeZone = message.Value.Timestamp.ToOffset(offset);


            message.Value.Timestamp = timeStampAsLocalTimeZone;


            LastLocation = message.Value;
            LastLocation.Timestamp = timeStampAsLocalTimeZone;

            try
            {
                var result = await apiService.SendLocation(LastLocation);

                System.Diagnostics.Debug.WriteLine("Location hashcode: " + LastLocation.GetHashCode());

                if (result)
                {
                    SuccessfulApiCalls++;
                }
                else
                {
                    FailedApiCalls++;
                }
            }
            catch (Exception ex)
            {
                //Redirect to Login page.
                System.Diagnostics.Debug.WriteLine(ex.Message);
                FailedApiCalls++;
            }

        });
    }

    public void Receive(LoginStatusMessage message)
    {
        StopLocationService();
    }
}
