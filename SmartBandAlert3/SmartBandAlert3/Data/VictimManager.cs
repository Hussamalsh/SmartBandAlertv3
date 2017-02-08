using SmartBandAlert3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Data
{
    public class VictimManager
    {

        IRestService restService;



        public VictimManager(IRestService service)

        {

            restService = service;

        }

        public Task SaveTaskAsync(Victim item, bool isNewItem = false)

        {

            return restService.SaveVictimAsync(item, isNewItem);

        }



    }
}
