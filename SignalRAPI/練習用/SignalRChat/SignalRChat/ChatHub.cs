using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
namespace SignalRChat
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            var content = $"{name} 於{DateTime.Now.ToShortTimeString()}說：{message}";
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
        /// <summary>
        /// 发送给指定连接
        /// </summary>
        /// <param name="toName"></param>
        /// <param name="content"></param>
        public void CallOne(string toName, string content)
        {
            //根据username获取对应的ConnectionId
            var connectionId = HttpContext.Current.Application[toName].ToString();
            Clients.Client(connectionId).show(content);
            //Clients.Users(new string[] { "myUser", "myUser2" }).show(content);
        }

        /// <summary>
        /// 初次连接
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string username = Context.QueryString["userName"]; //获取客户端发送过来的用户名
            string connectionId = Context.ConnectionId;
            HttpContext.Current.Application.Add(username, connectionId); //存储关系
            return base.OnConnected();
        }
    }
}