using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        // kliens csatlakozasnala chat hez ezt a fuggvenyt hivja meg
        public async Task JoinChat(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, requestId);
        }

        // Kilepeskor eltávolítjuk

        public async Task LeaveChat(string requestId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, requestId);
        }
    }
}
