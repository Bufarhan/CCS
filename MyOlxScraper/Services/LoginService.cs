using MyOlxScraper.Dto;

namespace MyOlxScraper.Services
{
    public class LoginService
    {
        private UserDto _currentUser;

        private readonly List<UserDto> _users = new()
    {
        new UserDto { Username = "admin", Password = "1234" }
    };

        public bool Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                _currentUser = user;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public bool IsLoggedIn => _currentUser != null;
        public string CurrentUsername => _currentUser?.Username;
    }

}
