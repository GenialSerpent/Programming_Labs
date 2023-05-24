using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DemoWebApp.Hubs
{
    [Authorize]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public ChatHub(){}

        private readonly string CHAT_ID = "CommonChat";

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Join()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, CHAT_ID);
        }

        public async Task Leave()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, CHAT_ID);
        }

        public async Task SendMessage(string message)
        {
            var user = Helpers.AuthHelper.GetUser(Context.User);
            await Clients.Group(CHAT_ID).SendAsync("Message", new { user, message });
        }
    }
}