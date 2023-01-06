using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SignalRAPI.Controllers
{
    public class BaseApiController<T> : ApiController where T : Hub
    {
        protected IHubConnectionContext<dynamic> Clients { get; private set; }
        protected IGroupManager Groups { get; private set; }
        protected BaseApiController()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<T>();
            Clients = context.Clients;
            Groups = context.Groups;
        }
    }
}