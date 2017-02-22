using Acr.UserDialogs;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using SmartBandAlert3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SmartBandAlert3
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        public List<DeviceListItemViewModel> SystemDevices { get; private set; }


        public HomePage()
        {

            var ble = CrossBluetoothLE.Current;
            Adapter = CrossBluetoothLE.Current.Adapter;

            InitializeComponent();


            Adapter.DeviceDiscovered += OnDeviceDiscovered;

            BindingContext = this;



        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            OnAlertYesNoClicked();

            GetSystemConnectedOrPairedDevices();


        }
        async void OnAlertYesNoClicked()
        {
            if (App.HaveSmartBand == false)//ToFix= see if the user answer this quetion or not
            {
                var answer = await DisplayAlert("Question?", "Do you have Smartband Alert product?", "Yes", "No");
                Debug.WriteLine("Answer: " + answer);
                App.HaveSmartBand = answer;
                ((App)App.Current).SaveProfile();
            }

        }


        IDisposable scan;
        //public bool IsScanning { get; private set; }
        public bool IsScanning => Adapter.IsScanning;

        public readonly IAdapter Adapter;
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();

        void Button_OnClickedScanToggle(Object obj, EventArgs e)
        {
            if (this.IsScanning)
            {
                this.scan?.Dispose();
                Adapter.StopScanningForDevicesAsync();
                this.ScanText.Text = Adapter.IsScanning ? "Scan" : "Stop Scan";
            }
            else
            {       
                this.ScanText.Text = "Stop Scan";
                ScanForDevices();
            }
        }
        private CancellationTokenSource _cancellationTokenSource;

        private async void ScanForDevices()
        {
            if(Devices.Count!= 0)
            Devices.Clear();

            foreach (var connectedDevice in Adapter.ConnectedDevices)
            {
                //update rssi for already connected evices (so tha 0 is not shown in the list)
                try
                {
                     connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    //Mvx.Trace(ex.Message);
                    //_userDialogs.ShowError($"Failed to update RSSI for {connectedDevice.Name}");
                }

                AddOrUpdateDevice(connectedDevice);
            }

            _cancellationTokenSource = new CancellationTokenSource();
           // RaisePropertyChanged(() => StopScanCommand);

            Adapter.StartScanningForDevicesAsync(null,null,false,_cancellationTokenSource.Token);
            //RaisePropertyChanged(() => IsRefreshing);
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            AddOrUpdateDevice(args.Device);
        }

        private void AddOrUpdateDevice(IDevice device)
        {


            var vm = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
            if (vm != null)
            {
                vm.Update();
            }
            else
            {
                if(device.Name!=null)
                Devices.Add(new DeviceListItemViewModel(device));
            }

            this.Devicesl.ItemsSource = this.Devices;


        }


        void SelectDevice(object sender, SelectedItemChangedEventArgs e)
        {




        }


        async void ConnectAndDisposeDevice(object o, EventArgs e)
        {


            var mi = ((MenuItem)o);
            DeviceListItemViewModel item = mi.CommandParameter as DeviceListItemViewModel;


            try
            {
                
                    //await Adapter.ConnectToDeviceAsync(Devices.FirstOrDefault().Device);
                Adapter.ConnectToDeviceAsync(item.Device);

                item.Update();

                    for (var i = 2; i >= 1; i--)
                    {

                        await Task.Delay(1000);

                    }
                
            }
            catch (Exception ex)
            {
            }
            finally
            {
               // this.Devicesl.ItemsSource = this.Devices;
                
            }

            this.Devicesl.ItemsSource = this.Devices;
            DisplayAlert("Your connected to ",item.Device.Name,"Ok");

            ScanForDevices();

            App.BlegUID = item.Device.Id.ToString();
            ((App)App.Current).SaveProfile();


        }

        private void GetSystemConnectedOrPairedDevices()
        {
            try
            {
                //heart rate
                //var guid = Guid.Parse("00000000-0000-0000-0000-e81af8931c9f");
                var guid = Guid.Parse(App.BlegUID);

                SystemDevices = Adapter.GetSystemConnectedOrPairedDevices(new[] { guid }).Select(d => new DeviceListItemViewModel(d)).ToList();
                //RaisePropertyChanged(() => SystemDevices);
                //SysDevicesX.ItemsSource = SystemDevices;
                this.Systconnocted.Text = "Device name: " +SystemDevices.FirstOrDefault().Device.Name;
                Adapter.ConnectToDeviceAsync(SystemDevices.FirstOrDefault().Device);
                this.SyststateL.Text = "Connected: " +  SystemDevices.FirstOrDefault().IsConnected.ToString();
            }
            catch (Exception ex)
            {
                Trace.Message("Failed to retreive system connected devices. {0}", ex.Message);
            }
        }


    }
}
