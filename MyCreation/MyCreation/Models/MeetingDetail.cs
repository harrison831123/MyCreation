using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCreation.Models
{
    public class MeetingDetail
    {
        public int MOID { get; set; }

        public int MDID { get; set; }

        public string imember { get; set; }

        public string iunit { get; set; }

        public string MOCreateIP { get; set; }

        public string MOTitle { get; set; }

        public string MODesc { get; set; }

        public DateTime MOCreateDate { get; set; }

        public DateTime MOStartDate { get; set; }

        public DateTime MOEndDate { get; set; }

        public int MOLendState { get; set; }

        public int MOCheck { get; set; }

        public int MTID { get; set; }

        public string MTName { get; set; }

        public string MDName { get; set; }

        public string nmember { get; set; }

        public string nunit { get; set; }

        public string iaccount { get; set; }
    }
}