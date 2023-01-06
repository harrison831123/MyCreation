using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRAPI.Models
{
    public class SignalRUser
    {
        /// <summary>
        /// user編號
        /// </summary>
        public string userID { get; set; }
        /// <summary>
        /// 連結編號
        /// </summary>
        public string connectionId { get; set; }

    }
}