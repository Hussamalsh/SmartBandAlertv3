using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Gcm.Client;
using Xamarin.Auth;
using System.Linq;

namespace SmartBandAlert3.Droid
{
    [Activity(Label = "SmartBandAlert3", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public static MainActivity CurrentActivity { get; private set; }
        protected override void OnCreate(Bundle bundle)
        {

            // Set the current instance of MainActivity.
            CurrentActivity = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);

            // On Android:
            var accounts = AccountStore.Create(this).FindAccountsForService("Facebook");
            var account = accounts.FirstOrDefault();
            if (account != null)
                App.IsLoggedIn = true;


            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();


            LoadApplication(new App());



            try
            {
                // Check to ensure everything's setup right
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                // Register for push notifications
                System.Diagnostics.Debug.WriteLine("Registering...");
                GcmClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
            }
            catch (Java.Net.MalformedURLException)
            {
                CreateAndShowDialog("There was an error creating the client. Verify the URL.", "Error");
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }



        }

        private void CreateAndShowDialog(String message, String title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}

