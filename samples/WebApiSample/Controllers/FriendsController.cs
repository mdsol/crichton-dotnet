using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiSample.Models;

namespace WebApiSample.Controllers
{
    /// <summary>
    /// Alternative controller that uses BuilderDescriptors to create Hypermedia responses
    /// </summary>
    public class FriendsController : ApiController
    {
        private static readonly IReadOnlyList<FriendModel> Data =
            new List<FriendModel> { 
                new FriendModel() { Id = 1, Name = "Bernie", Comment = "Awesome" }, 
                new FriendModel() { Id = 2, Name = "Mark", Comment = "Fantastic" },
                new FriendModel() { Id = 3, Name = "Charles", Comment = "Wonderful" } 
            };

        // GET api/friends/5
        public FriendModel Get(int id)
        {
            return Data.Single(d => d.Id == id);
        }

        public IEnumerable<FriendModel> Get()
        {
            return Data;
        }

    }
}
