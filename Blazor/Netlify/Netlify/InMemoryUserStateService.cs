using Netlifly.Shared;

namespace Netlify
{
    internal class InMemoryUserStateService
    {
        private readonly Dictionary<string, UserState> _userStates = new();

        public void SetUserState(string userId, UserState userState)
        {
            _userStates[userId] = userState;
        }

        public UserState? GetUserState(string userId)
        {
            _userStates.TryGetValue(userId, out var userState);
            return userState;
        }

        public void RemoveUserState(string userId)
        {
            _userStates.Remove(userId);
        }
    }

    internal class UserState
    {
        public User? User {get; set; }
        public bool IsAuthenticated {get; set;}

        public string? UserId { get; set; }
    }
}
