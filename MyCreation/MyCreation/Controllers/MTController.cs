using MyCreation.DB;
using MyCreation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCreation.Controllers
{
    public class MTController : Controller
    {
        // GET: MT
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  新增會議地點重覆時間判斷
        /// </summary>
        /// <param name="MDID">場地與設備編號</param>
        /// <param name="sdate">開始日期</param>
        /// <param name="edate">結束日期</param>
        /// <returns></returns>
        public ActionResult GetMDIDToChk(string MDID, string sdate, string edate)
        {
            
            //List<int> MDIDList = JsonConvert.DeserializeObject<List<int>>(MDID);
            int iMDID = Convert.ToInt32(MDID);
            MeetingDetail timechk = new MeetingDetail();
            
            DateTime s = Convert.ToDateTime(sdate);
            //bool result = true;
            if (iMDID != 0)
            {
                timechk = DBService.ChkMeetingTime(iMDID, Convert.ToDateTime(sdate), Convert.ToDateTime(edate));
                //if (timechk != null)
                //{
                //    result = false;
                //}
            }
            if (timechk == null || iMDID == 0)
            {
                int a = 0;
                return Json(a, JsonRequestBehavior.AllowGet);
            }
            int havetime = 1;
            return Json(havetime, JsonRequestBehavior.AllowGet);
        }
    }
}