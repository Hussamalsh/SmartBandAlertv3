using Newtonsoft.Json;
using SmartBandAlert3.Data;
using SmartBandAlert3.Models;
using SmartBandAlert3.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Xamarin.Forms;

namespace SmartBandAlert3
{
    public partial class App : Application
    {

        #region Facebook Auth Settings
        public static string AppId = "386661445019743";
        public static string DisplayName = "SBA App";
        public static string ExtendedPermissions = "user_about_me,email,public_profile";
        public static string AuthorizeUrl = "https://www.facebook.com/dialog/oauth";
        public static string RedirectUrl = "https://www.facebook.com/connect/login_success.html";
        #endregion


        #region Google Auth Settings
        public static string ClientId = "350066087874-q5qekvd52j96fkrktdqcu0vhmdepk00e.apps.googleusercontent.com";
        public static string ClientSecret = "jZ-f-oQiqM6dxIjGYjVVS0H2";
        public static string url1 = "https://www.googleapis.com/auth/userinfo.emai";
        public static string url2 = "https://accounts.google.com/o/oauth2/auth";
        public static string url3 = "https://www.youtube.com/channel/UCOjakhXt0i52uwYRjYBKjsg";
        public static string url4 = "https://accounts.google.com/o/oauth2/token";
        #endregion


        public static UserManager UserManager { get; private set; }
        public static FriendsManager FriendsManager { get; private set; }
        readonly IProfileManager _profileManager;
        Profile _profile;



        //HttpClient client;
        public App()
        {
            InitializeComponent();


            /*client = new HttpClient();
            client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var data = client.GetStringAsync("http://sbat1.azurewebsites.net/api/user").Result;
            var teams = JsonConvert.DeserializeObject<IEnumerable<User>>(data);*/


            UserManager = new UserManager(new RestService());
            FriendsManager = new FriendsManager(new RestService());

            IProfileManager profileManager = new ProfileManager();
            _profileManager = profileManager;
            LoadProfile();

            PresentMainPage();
        }


        public void PresentMainPage()
        {

            MainPage = !IsLoggedIn ? (Page)new LoginPage() : new MainPage();

            // MainPage = new LoginPage();

        }



        public void SaveProfile()
        {
            _profile.FBusername = FacebookName;
            _profile.FBimage = ProfilePic;
            _profile.FBid = FacebookId;
            _profileManager.SaveProfile(_profile);
        }
        void LoadProfile()
        {
            _profile = _profileManager.LoadProfile();
            FacebookId = _profile.FBid;
        }

        public static bool IsLoggedIn
        {
            get;
            set;
        }

        public static String FacebookId
        {
            get;
            set;
        }

        public static string FacebookName
        {
            get;
            set;
        }

        public static string EmailAddress
        {
            get;
            set;
        }

        public static string ProfilePic
        {
            get;
            set;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
