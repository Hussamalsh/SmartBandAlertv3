﻿using Acr.Ble;
using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Test
{
    public abstract class AbstractRootViewModel : AbstractViewModel
    {
        readonly ICoreServices services;


        protected AbstractRootViewModel(ICoreServices services)
        {
            this.services = services;
        }


        protected IUserDialogs Dialogs => this.services.Dialogs;
        protected IViewModelManager VmManager => this.services.VmManager;
        public IAppSettings AppSettings => this.services.AppSettings;
        public IAdapter BleAdapter => this.services.BleAdapter;
        public IAppState AppState => this.services.AppState;
    }
}
