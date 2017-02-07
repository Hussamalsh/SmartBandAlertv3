using Microsoft.WindowsAzure.MobileServices;
using SmartBandAlert3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Data
{
    public class FriendsManager
    {
        IRestService restService;

      

        public FriendsManager(IRestService service)

        {

            restService = service;

        }



        public Task<List<FriendsList>> GetTasksAsync()

        {

            return restService.RefreshDataAsyncFriends();

        }



        public Task SaveTaskAsync(User item, bool isNewItem = false)

        {

            return restService.SaveTodoItemAsync(item, isNewItem);

        }



        public Task DeleteTaskAsync(FriendsList item)

        {

            return restService.DeleteTodoItemAsync(App.FacebookId, item.FriendFBID);

        }


    }
}
