using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalRAPI
{
    public class SignalRHub : Hub
    {
        /// <summary>
        /// 发送给指定组
        /// </summary>
        //public void CallGroup(string fromname, string content)
        //{
        //    string groupname = Context.QueryString["groupname"]; //获取客户端发送过来的用户名
        //    //根据username获取对应的ConnectionId
        //    Clients.Group(groupname).show(fromname + ":" + content);
        //}

        //public override Task OnConnected()
        //{
        //    string groupname = Context.QueryString["groupname"]; //获取客户端发送过来的用户名
        //    JoinGroup(groupname);//加入群组
        //    return base.OnConnected();
        //}
        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    string groupname = Context.QueryString["groupname"];
        //    LeaveGroup(groupname);//移除组
        //    return base.OnDisconnected(true);
        //}
        //public Task JoinGroup(string groupName)
        //{
        //    return Groups.Add(Context.ConnectionId, groupName);
        //}

        //public Task LeaveGroup(string groupName)
        //{
        //    return Groups.Remove(Context.ConnectionId, groupName);
        //}
    }
}