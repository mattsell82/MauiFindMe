using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MauiGeo.Services;
using MauiGeo.Model;
using System.Collections.ObjectModel;

namespace MauiGeo.ViewModel;

public partial class FollowersPageViewModel : ObservableObject
{
    public ObservableCollection<UserName> Followers { get; } = new();

    [ObservableProperty]
    string email;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    bool isLoading;

    public bool IsNotLoading => !IsLoading;

    [ObservableProperty]
    string message;

    private ApiService apiService;

    public FollowersPageViewModel(ApiService apiService)
    {
        this.apiService = apiService;
    }

    [RelayCommand]
    public async Task RemoveFollower(UserName follower)
    {
        try
        {
            var result = await apiService.RemoveFollower(follower);

            if (result)
            {
                Email = String.Empty;
                await LoadFollowers();
            }
            else
            {
                await Shell.Current.DisplayAlert("Info", "Unable to remove follower", "Ok");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to add follower: " + ex.Message);
        }
    }

    [RelayCommand]
    public async Task AddFollower()
    {
        try
        {
            var result = await apiService.AddFollower(new UserName { Email = this.Email });
            if (result)
            {
                await LoadFollowers();
            }
            else
            {
                await Shell.Current.DisplayAlert("Info", "Unable to add follower", "Ok");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to add follower: " + ex.Message);
        }
    }

    [RelayCommand]
    public async Task LoadFollowers()
    {
        IsLoading = true;
        try
        {
            var result = await apiService.Followers();

            Followers.Clear();

            foreach (var item in result)
            {
                Followers.Add(item);
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            System.Diagnostics.Debug.WriteLine("Unable to Load followers: " + ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

}
