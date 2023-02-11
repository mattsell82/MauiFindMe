using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiGeo;

public class LocationService
{


    Location _location;
    bool stopping = false;
    IGeolocation _geolocation;
    bool _isCheckingLocation;
    GeolocationRequest geolocationRequest;

    public LocationService()
    {
        _geolocation = Geolocation.Default;
        geolocationRequest = new GeolocationRequest
        {
            DesiredAccuracy = GeolocationAccuracy.Medium,
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void Run()
    {
        stopping = false;
        System.Diagnostics.Debug.WriteLine("Starting location service. Stopping is " + stopping);
        Task.Run(() => GetCurrentLocation());
    }

    public void Stop()
    {
        stopping = true;
        System.Diagnostics.Debug.WriteLine("Stopping location service. Stopping is " + stopping);
    }


    public async Task GetCurrentLocation()
    {
        System.Diagnostics.Debug.WriteLine("trying to locate");
        //int counter = 0;


        while (!stopping)
        {
            try
            {




                _isCheckingLocation = true;

                //GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                //_cancelTokenSource = new CancellationTokenSource();

                //_location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);


                var status = await CheckAndRequestLocationPermission();

                if (status != PermissionStatus.Granted)
                {
                    System.Diagnostics.Debug.WriteLine("Location Access denied");
                    break;
                }

                _location = await _geolocation.GetLocationAsync(geolocationRequest);
                System.Diagnostics.Debug.WriteLine(_location);


                if (_location != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Latitude: {_location.Latitude}, Longitude: {_location.Longitude}, Altitude: {_location.Altitude}");

                    UpdateLocation();

                }
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (FeatureNotEnabledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (PermissionException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                _isCheckingLocation = false;
                //counter++;
            }
            await Task.Delay(20000);

        }

    }

    private void UpdateLocation()
    {
        WeakReferenceMessenger.Default.Send(new LocationUpdatedMessage(_location));
    }

    //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?tabs=android
    public async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

        if (status == PermissionStatus.Granted)
            return status;

        /*            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        // Prompt the user to turn on in settings
                        // On iOS once a permission has been denied it may not be requested again from the application
                        return status;
                    }

                    if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
                    {
                        // Prompt the user with additional information as to why the permission is needed
                    }*/

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        });

        return status;
    }

    public async Task<PermissionStatus> CheckAndRequestIgnoreBatteryOptimization()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Battery>();


        if (status == PermissionStatus.Granted)
            return status;

        /*            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        // Prompt the user to turn on in settings
                        // On iOS once a permission has been denied it may not be requested again from the application
                        return status;
                    }

                    if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
                    {
                        // Prompt the user with additional information as to why the permission is needed
                    }*/

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        });

        return status;
    }

}
