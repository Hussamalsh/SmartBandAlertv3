﻿using Acr.Ble;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartBandAlert3.Test
{
    public class DeviceViewModel : AbstractRootViewModel
    {
        IDisposable conn;
        IDisposable readRssiTimer;
        IDevice device;


        public DeviceViewModel(ICoreServices services) : base(services)
        {
            this.SelectCharacteristic = new Acr.Command<GattCharacteristicViewModel>(x => x.Select());
            this.SelectDescriptor = new Acr.Command<GattDescriptorViewModel>(x => x.Select());

            this.ConnectionToggle = ReactiveCommand.CreateFromTask(
                x =>
                {
                    if (this.conn == null)
                    {
                        this.conn = this.device.CreateConnection().Subscribe();
                    }
                    else
                    {
                        this.conn?.Dispose();
                        this.conn = null;
                    }
                    return Task.FromResult(Unit.Default);
                },
                this.WhenAny(
                    x => x.Status,
                    x => x.Value != ConnectionStatus.Disconnecting
                )
            );
            this.PairToDevice = ReactiveCommand.CreateFromTask(async x =>
            {
                if (!this.device.IsPairingRequestSupported)
                {
                    this.Dialogs.Alert("Pairing is not supported on this platform");
                }
                else if (this.device.PairingStatus == PairingStatus.Paired)
                {
                    this.Dialogs.Alert("Device is already paired");
                }
                else
                {
                    await this.device.PairingRequest();
                }
            });
            this.RequestMtu = ReactiveCommand.CreateFromTask(
                async x =>
                {
                    if (!this.device.IsMtuRequestAvailable)
                    {
                        this.Dialogs.Alert("MTU Request not supported on this platform");
                    }
                    else
                    {
                        var result = await this.Dialogs.PromptAsync(new PromptConfig()
                            .SetTitle("MTU Request")
                            .SetMessage("Range 20-512")
                            .SetInputMode(InputType.Number)
                            .SetOnTextChanged(args =>
                            {
                                var len = args.Value?.Length ?? 0;
                                if (len > 0)
                                {
                                    if (len > 3)
                                    {
                                        args.Value = args.Value.Substring(0, 3);
                                    }
                                    else
                                    {
                                        var value = Int32.Parse(args.Value);
                                        args.IsValid = value >= 20 && value <= 512;
                                    }
                                }
                            })
                        );
                        if (result.Ok)
                        {
                            this.device.RequestMtu(Int32.Parse(result.Text));
                            this.Dialogs.Alert("MTU Change Requested");
                        }
                    }
                },
                this.WhenAny(
                    x => x.Status,
                    x => x.Value == ConnectionStatus.Connected
                )
            );
        }


        public override void Init(object args)
        {
            this.device = (IDevice)args;
        }


        public override void OnActivate()
        {
            base.OnActivate();
            this.Name = this.device.Name;
            this.Uuid = this.device.Uuid;

            this.device
                .WhenNameUpdated()
                .Subscribe(x => this.Name = this.device.Name);

            this.device
                .WhenStatusChanged()
                .Subscribe(x => Device.BeginInvokeOnMainThread(() =>
                {
                    this.Status = x;

                    switch (x)
                    {
                        case ConnectionStatus.Disconnecting:
                        case ConnectionStatus.Connecting:
                            this.ConnectText = x.ToString();
                            break;

                        case ConnectionStatus.Disconnected:
                            this.ConnectText = "Connect";
                            this.readRssiTimer?.Dispose();
                            this.GattCharacteristics.Clear();
                            this.GattDescriptors.Clear();
                            break;

                        case ConnectionStatus.Connected:
                            this.ConnectText = "Disconnect";
                            this.readRssiTimer = this.device
                                .WhenRssiUpdated()
                                .Subscribe(rssi => this.Rssi = rssi);
                            break;
                    }
                }));

            this.device
                .WhenMtuChanged()
                .Subscribe(x => this.Dialogs.Alert($"MTU Changed size to {x}"));

            this.device
                .WhenServiceDiscovered()
                .Subscribe(service =>
                {
                    var group = new Group<GattCharacteristicViewModel>(service.Uuid.ToString());
                    var characters = service
                        .WhenCharacteristicDiscovered()
                        .Subscribe(character =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var vm = new GattCharacteristicViewModel(this.Dialogs, character);
                                group.Add(vm);
                                if (group.Count == 1)
                                    this.GattCharacteristics.Add(group);
                            });
                            character
                                .WhenDescriptorDiscovered()
                                .Subscribe(desc => Device.BeginInvokeOnMainThread(() =>
                                {
                                    var dvm = new GattDescriptorViewModel(this.Dialogs, desc);
                                    this.GattDescriptors.Add(dvm);
                                }));
                        });
                });
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();

            this.device.Disconnect();
            this.readRssiTimer?.Dispose();
            this.readRssiTimer = null;
            this.conn?.Dispose();
            this.conn = null;
        }


        public ICommand ConnectionToggle { get; }
        public ICommand PairToDevice { get; }
        public ICommand RequestMtu { get; }
        public Acr.Command<GattCharacteristicViewModel> SelectCharacteristic { get; }
        public Acr.Command<GattDescriptorViewModel> SelectDescriptor { get; }
        
        public string Name { get; private set; }
       
        public string ConnectText { get; private set; } = "Connect";
      
        public Guid Uuid { get; private set; }
       
        public int Rssi { get; private set; }
      
        public ConnectionStatus Status { get; private set; } = ConnectionStatus.Disconnected;
        public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics { get; } = new ObservableCollection<Group<GattCharacteristicViewModel>>();
        public ObservableCollection<GattDescriptorViewModel> GattDescriptors { get; } = new ObservableCollection<GattDescriptorViewModel>();
    }
}
