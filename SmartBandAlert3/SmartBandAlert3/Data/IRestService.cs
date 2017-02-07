using SmartBandAlert3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Data
{
    public interface IRestService

    {

        Task<List<User>> RefreshDataAsync();

        Task<List<User>> SearchUsersAsync(string text);
        Task<List<FriendsList>> RefreshDataAsyncFriends();

        Task SaveTodoItemAsync(User item, bool isNewItem);



        Task DeleteTodoItemAsync(String userid, long friendid);

    }
}
