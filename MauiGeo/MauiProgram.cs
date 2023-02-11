using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MauiGeo.Services;
using MauiGeo.ViewModel;

namespace MauiGeo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

#if ANDROID
        builder.Services.AddTransient<IForegroundService, ForegroundService>();
#endif

        builder
        .UseMauiApp<App>()
			.UseMauiMaps()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<LocationService>();

        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<RegisterPageViewModel>();

        builder.Services.AddTransient<BeaconPage>();
        builder.Services.AddTransient<BeaconPageViewModel>();

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginPageViewModel>();

        builder.Services.AddTransient<FollowersPage>();
        builder.Services.AddTransient<FollowersPageViewModel>();

        builder.Services.AddSingleton<TrackingPage>();
        builder.Services.AddSingleton<TrackingPageViewModel>();

        return builder.Build();
	}
}
