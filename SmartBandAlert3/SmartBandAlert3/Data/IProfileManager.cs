using SmartBandAlert3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Data
{
    public interface IProfileManager
    {
        void SaveProfile(Profile profile);

        Profile LoadProfile();
    }
}
