using MyCreation.Helper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCreation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

		#region 工廠模式
		public void Factory()
        {
			IFactory friesFac = new FrenchFriesFactory();
			IProduct fries = friesFac.getProduct();
			IProduct myfries = ((FrenchFriesFactory)friesFac).getProduct("無鹽的");

			fries.describe();//我是有鹽巴薯條
			myfries.describe();//我是無鹽的薯條
		}
		#endregion
	}
}