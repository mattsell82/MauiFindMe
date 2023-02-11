using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace MauiGeo;

//https://stackoverflow.com/questions/72276421/trust-additional-cas-and-make-use-of-the-android-certificate-store-in-a-net6-mau/72279358
[Application(UsesCleartextTraffic = true)]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{



    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
