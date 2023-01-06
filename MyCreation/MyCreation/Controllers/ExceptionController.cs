using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCreation.Controllers
{
    public class ExceptionController :  HandleErrorAttribute
    {
        // GET: Exception
        /// <summary>
        /// 抓取錯誤，除了有寫TryCatch則無法抓取
        /// </summary>
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                base.OnException(filterContext);
            string action = ((System.Web.HttpRequestWrapper)((System.Web.Mvc.Controller)filterContext.Controller).Request).Path;
            string userid = filterContext.HttpContext.User.Identity.Name;
            string msg = "發生時間: " + DateTime.Now + " " + "user" + "操作:" + action + "發生錯誤，<br>錯誤訊息:" + filterContext.Exception.Message + filterContext.Exception.StackTrace;

        }
    }
}