using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Test
{
    public interface IAppSettings : INotifyPropertyChanged
    {
        bool EnableBackgroundScan { get; set; }
        Guid BackgroundScanServiceUuid { get; set; }
        //Guid BgScanToConnect

        //bool AreNotificationsEnabled { get; set; }
        bool IsLoggingEnabled { get; set; }
    }
}
