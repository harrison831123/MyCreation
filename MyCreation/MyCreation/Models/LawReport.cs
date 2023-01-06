using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCreation.Models
{
    public class LawReport
    {
        public string Order { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string PaySequence { get; set; }
        public string Fileno { get; set; }
        public string DueName { get; set; }
        public string Agentcode { get; set; }
        public string DueMoney { get; set; }
        public string SuperAccount { get; set; }
        public string DueReason { get; set; }
        public string Doprogress { get; set; }
        public string Litigationprogress { get; set; }
    }
}