using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class SignalRHub:Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Broadcast the message to all clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToken(string user, string message)
        {
            // Send the message to all clients
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
