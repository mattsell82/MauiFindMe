using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGeo.Services;
using MauiGeo.Model;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;

namespace MauiGeo.ViewModel
{
    public partial class LoginPageViewModel : ObservableObject, IRecipient<LoginStatusMessage>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
        bool isLoggedIn;
        public bool IsNotLoggedIn => !IsLoggedIn;
        public ApiService ApiService { get; }

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        bool isLoading;
        public bool IsNotLoading => !IsLoading;

        TrackingPageViewModel trackingPageViewModel;


        public LoginPageViewModel(ApiService apiService, TrackingPageViewModel trackingPageViewModel)
        {
            ApiService = apiService;
            this.trackingPageViewModel = trackingPageViewModel;
            WeakReferenceMessenger.Default.Register<LoginStatusMessage>(this);
        }



        public async Task LoadLoginDetails()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading credentials");
                string storedEmail = await SecureStorage.Default.GetAsync("Email");

                if (!string.IsNullOrEmpty(storedEmail))
                    Email = storedEmail;

                string storedPassword = await SecureStorage.Default.GetAsync("Password");

                if (!string.IsNullOrEmpty(storedPassword))
                    Password = storedPassword;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading credentials: {ex}");
            }

        }

        [RelayCommand]
        public async Task Login()
        {
            IsLoading = true;

            try
            {
                var result = await ApiService.Login(new Credentials { Email = this.Email, Password = this.Password });

                if (!result)
                {
                    await Shell.Current.DisplayAlert("Info", "Login failed", "Ok");
                }

                IsLoggedIn = ApiService.CheckLogin();

                if (IsLoggedIn)
                {
                    SecureStorage.RemoveAll();

                    await SecureStorage.Default.SetAsync("Email", Email);
                    await SecureStorage.Default.SetAsync("Password", Password);

                    await trackingPageViewModel.LoadUsers();
                    await Shell.Current.GoToAsync($"//{nameof(BeaconPage)}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                IsLoading = false;
            }



        }



        [RelayCommand]
        public async Task LogOut()
        {
            var result = await ApiService.Logout(); ;
            IsLoggedIn = ApiService.CheckLogin();

            if (IsNotLoggedIn)
            {
                Application.Current.MainPage = new AppShell();
            }

        }

        [RelayCommand]
        public async Task Register()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        public void CheckLogin()
        {
            IsLoggedIn = ApiService.CheckLogin();
        }

        public void Receive(LoginStatusMessage message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Logout message recieved:");

                if (message.Value == false)
                {
                    await LogOut();
                }
            });
        }
    }
}
