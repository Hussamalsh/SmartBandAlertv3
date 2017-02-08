﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SmartBandAlert3
{
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterPage()

        {

            InitializeComponent();



            var masterPageItems = new List<MasterPageItem>();

            masterPageItems.Add(new MasterPageItem
            {

                Title = "Home Page",

                IconSource = "contacts.png",

                TargetType = typeof(HomePage)

            });

            masterPageItems.Add(new MasterPageItem
            {

                Title = "FriendsList",

                IconSource = "todo.png",

                TargetType = typeof(SkyddarePage)

            });

            masterPageItems.Add(new MasterPageItem
            {

                Title = "GPSTest",

                IconSource = "reminders.png",

                TargetType = typeof(GPStestPage)

            });
            


            listView.ItemsSource = masterPageItems;

        }
    }
}
