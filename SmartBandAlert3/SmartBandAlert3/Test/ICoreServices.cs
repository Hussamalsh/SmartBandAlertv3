using Acr.Ble;
using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Test
{
    public interface ICoreServices
    {
        IAppState AppState { get; }
        IAppSettings AppSettings { get; }
        IUserDialogs Dialogs { get; }
        IViewModelManager VmManager { get; }
        IAdapter BleAdapter { get; }
    }
}
