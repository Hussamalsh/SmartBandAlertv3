using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlert3.Models
{
    public class FriendsList
    {
        public long FriendFBID { get; set; }
        public String UserName { get; set; }
        public FriendsList(string n)
        {
            UserName = n;
        }

        public String ImgLink { get; set; }

    }
}
