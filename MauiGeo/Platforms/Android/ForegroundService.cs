using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MauiGeo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resource = MauiGeo.Resource;

namespace MauiGeo
{
    [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeLocation)]
    public class ForegroundService : Service, IForegroundService
    {

        LocationService location;

        public ForegroundService()
        {
            location = new LocationService();
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (intent.Action == "START_SERVICE")
            {
                System.Diagnostics.Debug.WriteLine("Tracker started");

                Task.Run(() => location.Run()).Wait(); //kör location service

                RegisterNotification();
            }
            else if (intent.Action == "STOP_SERVICE")
            {
                System.Diagnostics.Debug.WriteLine("Tracker stopped");
                Task.Run(() => location.Stop()).Wait(); //stoppa location service

                StopForeground(true);
                StopSelfResult(startId);

            }
            return StartCommandResult.Sticky;
        }

        public void Start()
        {
            Intent startService = new Intent(MainActivity.ActivityCurrent, typeof(ForegroundService));
            startService.SetAction("START_SERVICE");

            MainActivity.ActivityCurrent.StartService(startService);
        }

        public void Stop()
        {
            Intent stopIntent = new Intent(MainActivity.ActivityCurrent, this.Class);
            stopIntent.SetAction("STOP_SERVICE");
            MainActivity.ActivityCurrent.StartService(stopIntent);
        }

        //Av någon anledning kraschar applikationen i releasmode med denna kod. Men det fungerar i debug..
        //https://stackoverflow.com/questions/68894283/xamarin-forms-how-to-check-setting-of-requestignorebatteryoptimizations-permis
/*        private void Battery()
        {
            String packageName = MainActivity.ActivityCurrent.PackageName;

            PowerManager pm = (PowerManager)MainActivity.ActivityCurrent.GetSystemService(Context.PowerService);
            if (!pm.IsIgnoringBatteryOptimizations(packageName))
            {
                Intent intent = new Intent();
                intent.SetAction(Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                MainActivity.ActivityCurrent.StartActivity(intent);
            }
        }*/

        /*        private void AddWakeLock()
                {
                    PowerManager.WakeLock wakelock = default;
                    PowerManager pm = (PowerManager)MainActivity.ActivityCurrent.GetSystemService(Context.PowerService);
                    wakelock = pm.NewWakeLock(WakeLockFlags.Partial, "locationWakelock");
                    wakelock.SetReferenceCounted(false);
                    wakelock.Acquire();
                }*/



        private void RegisterNotification()
        {
            NotificationChannel channel = new NotificationChannel("LocationChannel", "FindMe", NotificationImportance.Max);
            NotificationManager manager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(Context.NotificationService);
            manager.CreateNotificationChannel(channel);
            Notification notification = new Notification.Builder(this, "LocationChannel")
                .SetContentTitle("Location sharing is active.")
                .SetSmallIcon(Resource.Drawable.btn_checkbox_checked_mtrl)
                .SetOngoing(true)
                .Build();

            StartForeground(100, notification);
        }
    }
}
