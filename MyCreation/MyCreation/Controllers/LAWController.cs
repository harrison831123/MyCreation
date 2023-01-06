using MyCreation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCreation.Controllers
{
    public class LAWController : Controller
    {
        // GET: LAW
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 匯入報表
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tabUniqueId"></param>
        /// <returns></returns>
        public ActionResult Upload(HttpPostedFileBase file, string tabUniqueId)
        {
            List<string> errorList = new List<string>();  //記錄錯誤
            if (file != null)
            {
                StreamReader sr = new StreamReader(file.InputStream, System.Text.Encoding.Default);
                List<LawReport> lawReportList = new List<LawReport>();
                string data, LawNoteNo = string.Empty, monstr, fileno, superaccount, ymstr = string.Empty, agent_code = string.Empty, blockvm, blockvmcode;

                sr.ReadLine();//標題
                int datacount = 0;

                while (!sr.EndOfStream)
                {
                    datacount++;
                    data = sr.ReadLine();
                    string[] arrData = data.Split(',');
                    //檢查資料格式
                    if (arrData.Length != 10)
                    {
                        errorList.Add("案件匯入格式不符!請重新檢查文件(欄位中勿使用半形逗點符號!)、欄位數目!");
                        break;
                    }

                    LawReport lawReport = new LawReport();
                    lawReport.Order = arrData[0];
                    lawReport.Year = arrData[1];
                    lawReport.Month = arrData[2];
                    lawReport.PaySequence = arrData[3];
                    lawReport.Fileno = arrData[4];
                    //string DueName = Member.Get(arrData[6]).Name;
                    lawReport.DueName = arrData[5];
                    lawReport.Agentcode = arrData[6];
                    lawReport.DueMoney = arrData[7];
                    lawReport.SuperAccount = arrData[8];
                    lawReport.DueReason = arrData[9];
                    lawReportList.Add(lawReport);

                    monstr = arrData[2];
                    if (monstr.Length == 1)
                    {
                        monstr = "0" + monstr;
                    }
                    switch (arrData[1].ToString().Length)
                    {
                        case 2:
                            ymstr = "00" + arrData[1] + "/" + monstr;
                            break;
                        case 3:
                            ymstr = "0" + arrData[1] + "/" + monstr;
                            break;
                        case 4:
                            ymstr = arrData[1] + "/" + monstr;
                            break;
                    }                  
                }

                //新增資料....

                if (errorList.Count == 0)
                {
                    

                    return Json(datacount);
                }
                else
                {

                    return Json(errorList);
                }

            }
            else
            {
    
                return Json(errorList);
            }
            return Json(errorList);
        }
    }
}