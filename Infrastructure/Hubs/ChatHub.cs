using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        // kliens csatlakozasnala chat hez ezt a fuggvenyt hivja meg
        [Authorize]
        public async Task JoinChat(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, requestId);
        }

        // Kilepeskor eltávolítjuk
        [Authorize]
        public async Task LeaveChat(string requestId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, requestId);
        }
    }
}
