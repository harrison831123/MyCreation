using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRAPI
{
    public class ContosoChatHub : Hub
    {
        public void NewContosoChatMessage(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
        public void SendMessage(string name, string message)
        {

            Clients.All.addContosoChatMessageToPage(new ContosoChatMessage() { UserName = name, Message = message });
        }

        public class ContosoChatMessage
        {
            public string UserName { get; set; }
            public string Message { get; set; }
        }
    }
}