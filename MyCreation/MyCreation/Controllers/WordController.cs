using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCreation.Helper;

namespace MyCreation.Controllers
{
    public class WordController : Controller
    {
        // GET: Word
        public ActionResult Index()
        {

            //Example01_WordTmplRendering();
            //Example02_DynamicWordTmplRendering();
            Example03();
            return View();
        }

        public void Example01_WordTmplRendering()
        {
            var docxBytes = WordRender.GenerateDocx(System.IO.File.ReadAllBytes(Path.Combine(@"C:\Users\F129087247\Documents\GitHub\harrison", $"AnnounceTemplate.docx")),
                new Dictionary<string, string>()
                {
                    ["Title"] = "澄清黑暗執行緒部落格併購傳聞",
                    ["SeqNo"] = "2021-FAKE-001",
                    ["PubDate"] = "2021-04-01",
                    ["Source"] = "亞太地區公關部",
                    ["Content"] = @"
　　                坊間媒體盛傳「史塔克工業將以美金 18 億元併購黑暗執行緒部落格」一事，
                本站在此澄清並無此事。
　　                史塔克公司執行長日前確實曾派遣代表來訪，雙方就技術合作一事交換意見，
                相談甚歡，惟本站暫無出售計劃，且傳聞金額亦不符合本站預估價值(謎之聲：180 元都嫌貴)，
                純屬不實資訊。
　　                本站將秉持初衷，持續發揚野人獻曝、敝帚自珍精神，歡迎舊雨新知繼續支持。"
                });
            System.IO.File.WriteAllBytes(Path.Combine(@"C:\Users\F129087247\Documents\GitHub\harrison", $"套表測試-{DateTime.Now:HHmmss}.docx"), docxBytes);
        }

        public void Example02_DynamicWordTmplRendering()
        {
            string path = @"C:\Users\F129087247\Documents\GitHub\harrison\DynamicTemplate.docx";
            string[] sArray = new string[] { "EP369", "EP370" };

            var docxBytes = WordRender.GenVocQuizPaper(path, sArray);
            System.IO.File.WriteAllBytes(Path.Combine(@"C:\Users\F129087247\Documents\GitHub\harrison", $"動態報表-{DateTime.Now:HHmmss}.docx"), docxBytes);
        }

        public void Example03()
        {
            string path = @"C:\Users\F129087247\Documents\GitHub\harrison\程式上線申請單.docx";
            string[] sArray = new string[] { "center_po_tra_search.aspx.vb", "26450", "□VB.NET", "BKS", "$/EPBKS/MIS/project/PBD","□修改" };

            //new Dictionary<string, string>()
            //{
            //    ["Name"] = sArray[0],
            //    ["ChangeSet"] = sArray[1],
            //    ["Type"] = sArray[2],
            //    ["Server"] = sArray[3],
            //    ["TFSPath"] = sArray[4]
            //};

            var docxBytes = WordRender.Gettest(path, sArray);
            System.IO.File.WriteAllBytes(Path.Combine(@"C:\Users\F129087247\Documents\GitHub\harrison", $"程式上線申請單-{DateTime.Now:HHmmss}.docx"), docxBytes);
        }
    }
}