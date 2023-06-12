using System.Net.NetworkInformation;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace TestReact
{
    public interface IUserManagement
    {
        public LoginResult Login(string user, string password);
        public LoginResult Logout(string userId);
        public IEnumerable<UserInfo> GetUsers();
        public UserInfo GetUser(string userId);
    }

    public class UserManagement : IUserManagement
    {
        private ILogger<UserManagement> _logger;
        private ConcurrentDictionary<string, UserInfo> _users;
        public UserManagement(ILogger<UserManagement> logger)
        {
            _logger = logger;
            _users = new ConcurrentDictionary<string, UserInfo>();
        }

        public LoginResult Login(string name, string password)
        {
            String id = System.Guid.NewGuid().ToString();
            UserInfo inf = new UserInfo
            {

                Name = name,
                Id = id,
                LastSeen = DateTime.Now,
            };

            LoginResult result = new LoginResult();
            result.LoginResultValue = true;
            return result;

        }



        public IEnumerable<UserInfo> GetUsers()
        {

            IEnumerable<UserInfo> res = _users.Values;
            return res;

        }



        public UserInfo? GetUser(string userId)
        {
            return _users.TryGetValue(userId, out UserInfo? user) ? user : null;
        }

        public LoginResult Logout(string userdId)
        {
            UserInfo user;
            bool res=  _users.TryRemove(userdId, out user);
            LoginResult result = new LoginResult();
            result.LoginResultValue = res;

            return result;

        }
    }
}
