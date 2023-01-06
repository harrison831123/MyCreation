using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRAPI.Models
{
    public class SignalRCode
    {
        public string name { get; set; }

        public string ConnectionID { get; set; }

        public string message { get; set; }

        public string userID { get; set; }

        public string groupname { get; set; }

        public string SystemType { get; set; }

        public string BrowserType { get; set; }
    }
}