using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartBandAlert3.Test
{
    public interface IViewModelManager
    {
        TViewModel Create<TViewModel>(object args = null) where TViewModel : class, IViewModel;
        Page CreatePage<TViewModel>(object args = null) where TViewModel : class, IViewModel;
        Task Push<TViewModel>(object args = null) where TViewModel : class, IViewModel;
        Task Pop();
    }
}
