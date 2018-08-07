using Microsoft.AspNet.SignalR;

namespace UnoGame.Services
{
    public interface IPlayerHub
    {
        void ReloadUsers();
    }

    public class PlayerHub : Hub, IPlayerHub
    {
        private readonly IUserService _userService;

        public PlayerHub(IUserService userService)
        {
            _userService = userService;
        }

        public void ReloadUsers()
        {
            Clients.All.updateUsers(_userService.Users);
        }
    }


    public interface IChatHub
    {
        void Send(string to, string message);
    }

    public class ChatHub : Hub
    {
        public void Send(string to, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(to, message);
        }
    }
}