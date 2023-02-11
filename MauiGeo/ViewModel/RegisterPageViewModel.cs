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

namespace MauiGeo.ViewModel
{
    public partial class RegisterPageViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
        bool isLoggedIn;
        public bool IsNotLoggedIn => !IsLoggedIn;
        public ApiService ApiService { get; }

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string confirmPassword;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        bool isLoading;
        public bool IsNotLoading => !IsLoading;


        public RegisterPageViewModel(ApiService apiService)
        {
            ApiService = apiService;
        }



        [RelayCommand]
        public async Task Register()
        {
            //TODO: check if password and confirm password are the same.

            if (password != confirmPassword)
            {
                await Shell.Current.DisplayAlert("Info", "Passwords does not match.", "Ok");
                return;
            }


            //TODO: ensure internet connection
            //TODO: add try-catch
            var result = await ApiService.Register(new Credentials { Email = this.Email, Password = this.Password });

            if (result)
            {
                await Shell.Current.DisplayAlert("Info", "User registered", "Ok");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Info", "Registration failed", "Ok");
            }

        }


    }
}
