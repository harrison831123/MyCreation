using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SignalRAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            config.EnableCors();
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "SignalRapi/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Filters.Add(new ApiResponseAttribute());
            //config.Filters.Add(new ApiExceptionResponseAttribute());
        }
    }
}
