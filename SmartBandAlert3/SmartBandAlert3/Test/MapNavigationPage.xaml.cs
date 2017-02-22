using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Xamarin.Forms;
using SmartBandAlert3.Models;

using Plugin.Permissions.Abstractions;
using Plugin.Permissions;

namespace SmartBandAlert3.Test
{
    public partial class MapNavigationPage : ContentPage
    {


        public  MapNavigationPage(string victimId)
        {
            InitializeComponent();

            getVictim(victimId);






        }//50.894967,4.341626

        


        void OnNavigateButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputEntry.Text))
            {
                var address = inputEntry.Text;
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                            new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                            new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        Device.OpenUri(
                            new Uri(string.Format("bingmaps:?where={0}", Uri.EscapeDataString(address))));
                        break;
                }
            }
        }

        public async void getVictim(string victimId)
        {

            var v = await App.VictimManager.SearchVictimAsync(victimId);


            if (!string.IsNullOrWhiteSpace(inputEntry.Text))
            {
                var address = inputEntry.Text;
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                            new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                            new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(v.Latitude + "," + v.Longitude))));
                        break;
                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        Device.OpenUri(
                            new Uri(string.Format("bingmaps:?where={0}", Uri.EscapeDataString(address))));
                        break;
                }
            }
        }









        


    }
}
