using Netlifly.Shared;

namespace Netlify
{
    internal class ServerState
    {
        public User? User { get; private set; }
        public event Action? OnChange;

        public ServerState()
        {
            int a = 0;
        }
        public void SetUser(User user)
        {
            User = user.ShallowClone();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
