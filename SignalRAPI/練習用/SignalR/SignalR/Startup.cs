using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using SignalR;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            //app.MapSignalR();
            app.MapSignalR("/signalr", new HubConfiguration());
        }
        //public void Configuration(IAppBuilder app)
        //{
        //    //app.MapSignalR(new HubConfiguration() { EnableJSONP = true });
        //    app.Map("/signalr", map =>
        //    {
        //        map.UseCors(CorsOptions.AllowAll);
        //        var hubConfiguration = new HubConfiguration
        //        {
        //            EnableDetailedErrors = true
        //        };
        //        map.RunSignalR(hubConfiguration);
        //    });
        //}
    }
}