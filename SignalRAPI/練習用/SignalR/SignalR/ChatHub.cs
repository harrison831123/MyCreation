using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalR
{
    [HubName("PascalCaseContosoChatHub")]
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, message + "1456446");
            var content = $"{name} 於{DateTime.Now.ToShortTimeString()}說：{message}";
            // Call the addNewMessageToPage method to update clients.          
            Clients.All.addContosoChatMessageToPage(name, message);
        }
        //public async Task NewContosoChatMessage(string name, string message)
        //{
        //    await Clients.All.addNewMessageToPage(name, message);
        //}

    }
}