﻿using Acr.Ble;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartBandAlert3.Test
{
    public partial class ScanPage : ContentPage
    {

        IDisposable scan;
        IDisposable connect;

        public IAdapter BleAdapter => BleAdapter;
        public IAppState AppState => AppState;
        IViewModelManager VmManager { get; }
        public ScanPage()
        {

            InitializeComponent();


            this.connect = this.BleAdapter
                .WhenDeviceStatusChanged()
                .Subscribe(x =>
                {
                    var vm = this.Devices.FirstOrDefault(dev => dev.Uuid.Equals(x.Uuid));
                    if (vm != null)
                        vm.IsConnected = x.Status == ConnectionStatus.Connected;
                });

            this.AppState.WhenBackgrounding().Subscribe(_ => this.scan?.Dispose());
            this.BleAdapter.WhenScanningStatusChanged().Subscribe(on =>
            {
                this.IsScanning = on;
                this.ScanText = on ? "Stop Scan" : "Scan";
            });
            this.Devices = new ObservableCollection<ScanResultViewModel>();

            this.SelectDevice = new Acr.Command<ScanResultViewModel>(x =>
            {
                this.scan?.Dispose();
                VmManager.Push<DeviceViewModel>(x.Device);
            });

            this.OpenSettings = new Command(() =>
            {
                if (this.BleAdapter.CanOpenSettings)
                {
                    this.BleAdapter.OpenSettings();
                }
                else
                {
                    this.Dialogs.Alert("Cannot open bluetooth settings");
                }
            });
            this.ToggleAdapterState = ReactiveCommand.CreateFromTask(
                x =>
                {
                    if (this.BleAdapter.CanChangeAdapterState)
                    {
                        this.BleAdapter.SetAdapterState(true);
                    }
                    else
                    {
                        this.Dialogs.Alert("Cannot change bluetooth adapter state");
                    }
                    return Task.FromResult(Unit.Default);
                },
                this.BleAdapter
                    .WhenStatusChanged()
                    .Select(x => x == AdapterStatus.PoweredOff)
            );

            this.ScanToggle = ReactiveCommand.CreateFromTask(
                x =>
                {
                    if (this.IsScanning)
                    {
                        this.scan?.Dispose();
                    }
                    else
                    {
                        this.Devices.Clear();
                        this.ScanText = "Stop Scan";

                        this.scan = this.BleAdapter
                            .Scan()
                            .Subscribe(this.OnScanResult);
                    }
                    return Task.FromResult<object>(null);
                },
                this.WhenAny(
                    x => x.IsSupported,
                    x => x.Value
                )
            );

        }





        public  void OnActivate()
        {
            //base.OnActivate();
            this.BleAdapter
                .WhenStatusChanged()
                .Subscribe(x =>
                {
                    this.IsSupported = x == AdapterStatus.PoweredOn;
                    this.Title = $"BLE Scanner ({x})";
                });
        }




        public ICommand ScanToggle { get; }
        public ICommand OpenSettings { get; }
        public ICommand ToggleAdapterState { get; }
        public Acr.Command<ScanResultViewModel> SelectDevice { get; }
        public ObservableCollection<ScanResultViewModel> Devices { get; }
        public bool IsScanning { get; private set; }
        public bool IsSupported { get; private set; }
        public string ScanText { get; private set; }
        public string Title { get; private set; }


        void OnScanResult(IScanResult result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var dev = this.Devices.FirstOrDefault(x => x.Uuid.Equals(result.Device.Uuid));
                if (dev != null)
                {
                    dev.TrySet(result);
                }
                else
                {
                    dev = new ScanResultViewModel();
                    dev.TrySet(result);
                    this.Devices.Add(dev);
                }
            });
        }
    }
}
