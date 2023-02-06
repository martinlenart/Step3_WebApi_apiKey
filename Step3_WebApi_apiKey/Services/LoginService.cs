using System;
using Step3_WebApi_Jwt.Models;


namespace Step3_WebApi_Jwt.Services
{
    public interface ILoginService
    {
        bool LoginUser(string UserName, string Password, out User user);

        public bool ValidateApiKey(string sapiKey, out User loggedInUser);
        bool ValidateApiKey(Guid apiKey, out User loggedInUser);

        List<User> LoggedinUsers { get; }
    }


    public class LoginService : ILoginService
	{
        private static readonly object _locker = new();

        private List<User> _users = AppConfig.GetUsers;
        private Dictionary<Guid, User> _apiKeysInUse = new Dictionary<Guid, User>();

		public LoginService()
		{
		}

        public bool LoginUser(string UserName, string Password, out User user)
        {
            lock (_locker)
            {
                var u = _users.Find(x => ((x.Name.Equals(UserName, StringComparison.OrdinalIgnoreCase) || x.Email.Equals(UserName, StringComparison.OrdinalIgnoreCase))
                    && x.Password.Equals(Password, StringComparison.OrdinalIgnoreCase)));
                if (u != null)
                {
                    user = u;
                    _apiKeysInUse.TryAdd(u.apiKey, user);
                    return true;
                }

                user = null;
                return false;
            }
        }

        public void LogoutUser(Guid apiKey)
        {
            lock (_locker)
            {
                _apiKeysInUse.Remove(apiKey);
            }
        }

        public bool ValidateApiKey(string sapiKey, out User loggedInUser)
        {
            lock (_locker)
            {
                loggedInUser = null;
                Guid apiKey;

                if (!Guid.TryParse(sapiKey, out apiKey) || apiKey == Guid.Empty)
                    return false;

                return _apiKeysInUse.TryGetValue(apiKey, out loggedInUser);
            }
        }

        public bool ValidateApiKey(Guid apiKey, out User loggedInUser)
        {
            lock (_locker)
            {
                loggedInUser = null;
                if (apiKey != Guid.Empty)
                    return false;

                return _apiKeysInUse.TryGetValue(apiKey, out loggedInUser);
            }
        }

        public List<User> LoggedinUsers
        {
            get
            {
                lock (_locker) return _apiKeysInUse.Values.ToList();
            }
        }
    }
}

