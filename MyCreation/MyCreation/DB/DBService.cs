using EPBKS.Db;
using MyCreation.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace MyCreation.DB
{
    public class DBService
    {
        static DbHelperSql tssql = null;
        /// <summary>
        /// 檢查會議時間有無重複
        /// </summary>
        /// <param name="MDID"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public static MeetingDetail ChkMeetingTime(int MDID, DateTime SDate, DateTime EDate)
        {
            tssql = new DbHelperSql("H2O");
            MeetingDetail result = new MeetingDetail();
            string sql = @"with t1 as(select * from MeetOrder)";
            sql += " select * from t1,MeetDevice D,Meeting A";
            sql += " WHERE t1.MDID = D.MDID and t1.MTID = A.MTID and ((t1.MOStartDate <= @MOStartDate AND t1.MOEndDate >=  @MOStartDate) or (t1.MOStartDate <=  @MOEndDate AND t1.MOEndDate >= @MOEndDate)";
            sql += " or (t1.MOStartDate >= @MOStartDate AND t1.MOEndDate <= @MOEndDate))";
            sql += " and t1.MDID = @MDID";

            tssql.sqlParaColl.Clear();
            tssql.sqlParaColl.AddWithValue("@MDID", MDID);
            tssql.sqlParaColl.AddWithValue("@SDate", SDate);
            tssql.sqlParaColl.AddWithValue("@EDate", EDate);
            IDataReader rdr = tssql.SQL_ExecReader(sql);

            while (rdr.Read())
            {
                result.MDID = (int)rdr["MDID"];
            }
            rdr.Close();
            tssql.SQL_Close();


            return result;
        }

        /// <summary>
        /// bulkinsert範例
        /// </summary>
        /// <param name="dt">data</param>
        public void ag078BulkInsert(DataTable dt, List<string> descs)
        {
            try
            {
                string ConnectionString = "";
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnectionString))
                {
                    bulkCopy.DestinationTableName = dt.TableName;
                    List<KeyValuePair<string, string>> kv = GetTableColumnName(dt.TableName);
                    foreach (var d in kv)
                    {
                        if (!d.Key.Equals("iden") && !d.Key.Equals("csub") && !d.Key.Equals("code"))
                        {
                            bulkCopy.ColumnMappings.Add(d.Key, d.Value);
                        }
                    }
                    bulkCopy.WriteToServer(dt);
                }

                string sql = "INSERT INTO ABc " +
                             "VALUES (@G, @P, @S, @D)";

                tssql.SQL_ExecReader(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 取table的欄位對照
        /// </summary>
        /// <param name="tableName">table名稱</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetTableColumnName(string tableName)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            //string sql = @"select column_name as [Key], trim(column_name) as [Value] from v_TableInfo where table_name =@tableName";
            string sql = @"SELECT column_name as [Key], trim(column_name) as [Value] FROM INFORMATION_SCHEMA.COLUMNS where table_name = @tableName";
            IDataReader rdr = tssql.SQL_ExecReader(sql);

            while (rdr.Read())
            {              
                KeyValuePair<string, string> kvalue = new KeyValuePair<string, string>((string)rdr["Key"], (string)rdr["Value"]);
                result.Add(kvalue);
            }
            rdr.Close();
            tssql.SQL_Close();

            return result;
        }

        /// <summary>
        /// 保險公司佣酬檢核表
        /// </summary>
        /// <param name="year">年度</param>
        /// <returns>Stream</returns>
        public Stream GetCompanyMerSalDReportList(MerSalViewModel condition)
        {
            MemoryStream ms = new MemoryStream();
            ExcelPackage excel = new ExcelPackage();

            #region SQL資料
            //string WebBroker = WebBrokerRepository.DbInfo.ConnectionString;
            //DataSet ds = new DataSet();
            //using (SqlConnection sqlcon = new SqlConnection(WebBroker))
            //{
            //    using (SqlCommand cmd = new SqlCommand("usp_MerSalDRpt", sqlcon))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.Add("@ProductionYm", SqlDbType.VarChar).Value = condition.ProductionYM;
            //        cmd.Parameters.Add("@Sequence", SqlDbType.VarChar).Value = condition.Sequence;
            //        cmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = condition.CompanyCode ?? "";
            //        cmd.Parameters.Add("@Rpttype", SqlDbType.VarChar).Value = "Item";

            //        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            //        {
            //            da.Fill(ds);
            //        }
            //    }
            //}

            //// 定義集合    
            //List<MerSalSystemReportModel> merSalSystemReports = new List<MerSalSystemReportModel>();
            //List<MerSalCompanyReportModel> merSalCompanyReports = new List<MerSalCompanyReportModel>();
            //MerSalSystemReportModel t = new MerSalSystemReportModel();
            //MerSalCompanyReportModel t2 = new MerSalCompanyReportModel();
            //PropertyInfo[] prop = t.GetType().GetProperties();
            //PropertyInfo[] prop2 = t2.GetType().GetProperties();
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    t = new MerSalSystemReportModel();
            //    //通過反射獲取T類型的所有成員
            //    foreach (PropertyInfo pi in prop)
            //    {
            //        //DataTable列名=屬性名
            //        if (ds.Tables[0].Columns.Contains(pi.Name))
            //        {
            //            //屬性值不為空
            //            if (dr[pi.Name] != DBNull.Value)
            //            {
            //                object value = Convert.ChangeType(dr[pi.Name], pi.PropertyType);
            //                //給T類型字段賦值
            //                pi.SetValue(t, value, null);
            //            }
            //        }
            //    }
            //    //將T類型添加到集合list
            //    merSalSystemReports.Add(t);
            //}
            //foreach (DataRow dr in ds.Tables[1].Rows)
            //{
            //    t2 = new MerSalCompanyReportModel();
            //    //通過反射獲取T類型的所有成員
            //    foreach (PropertyInfo pi in prop2)
            //    {
            //        //DataTable列名=屬性名
            //        if (ds.Tables[1].Columns.Contains(pi.Name))
            //        {
            //            //屬性值不為空
            //            if (dr[pi.Name] != DBNull.Value)
            //            {
            //                object value = Convert.ChangeType(dr[pi.Name], pi.PropertyType);
            //                //給T類型字段賦值
            //                pi.SetValue(t2, value, null);
            //            }
            //        }
            //    }
            //    //將T類型添加到集合list
            //    merSalCompanyReports.Add(t2);
            //}
            #endregion

            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("保險公司佣酬檢核報表");
            //直排
            int a = 1, b = 2, c = 3, d = 4, e = 5, f = 6, g = 7, h = 8, j = 9, k = 10, l = 11, m = 12, n = 13, o = 14;
            //橫排
            int aa = 3, bb = 13;

            #region 系統執行項目
            //for (int i = 0; i < merSalSystemReports.Count; i++)
            //{
            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].amount_type }, aa, a);
            //    sheet.Cells[aa, a].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].modx_year_name }, aa, b);
            //    sheet.Cells[aa, b].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].Cnt_Cut00 }, aa, c);
            //    sheet.Cells[aa, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].ModePrem_Cut00 }, aa, d);
            //    sheet.Cells[aa, d].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].CommPrem_Cut00 }, aa, e);
            //    sheet.Cells[aa, e].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].Cnt_Cut01 }, aa, f);
            //    sheet.Cells[aa, f].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].ModePrem_Cut01 }, aa, g);
            //    sheet.Cells[aa, g].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].CommPrem_Cut01 }, aa, h);
            //    sheet.Cells[aa, h].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].Cnt_Check }, aa, j);
            //    sheet.Cells[aa, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].ModePrem_Check }, aa, k);
            //    sheet.Cells[aa, k].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].CommPrem_Check }, aa, l);
            //    sheet.Cells[aa, l].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].Cnt_Total }, aa, m);
            //    sheet.Cells[aa, m].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].ModePrem_Total }, aa, n);
            //    sheet.Cells[aa, n].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalSystemReports[i].CommPrem_Total }, aa, o);
            //    sheet.Cells[aa, o].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    aa++;
            //}
            #endregion

            #region 保險公司佣酬檢核表
            //for (int i = 0; i < merSalCompanyReports.Count; i++)
            //{
            //    ExcelSetCell(sheet, new string[] { merSalCompanyReports[i].production_ym }, bb, a);
            //    sheet.Cells[bb, a].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalCompanyReports[i].sequence }, bb, b);
            //    sheet.Cells[bb, b].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalCompanyReports[i].company_code }, bb, c);
            //    sheet.Cells[bb, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalCompanyReports[i].cnt }, bb, d);
            //    sheet.Cells[bb, d].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalCompanyReports[i].mode_prem }, bb, e);
            //    sheet.Cells[bb, e].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //    ExcelSetCell(sheet, new string[] { merSalCompanyReports[i].comm_prem }, bb, f);
            //    sheet.Cells[bb, f].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    bb++;
            //}
            #endregion

            #region 標題
            //sheet 標題 橫排 直排
            ExcelSetCell(sheet, new string[] { "系統執行項目" }, 1, 1);
            ExcelSetCell(sheet, new string[] { "" }, 1, 14);
            sheet.Cells[1, 1, 1, 14].Merge = true;
            sheet.Cells[1, 1, 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //sheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //sheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Blue);

            ExcelSetCell(sheet, new string[] { "佣酬類別" }, 2, 1);
            ExcelSetCell(sheet, new string[] { "年期" }, 2, 2);
            ExcelSetCell(sheet, new string[] { "系統(保留)筆數" }, 2, 3);
            ExcelSetCell(sheet, new string[] { "系統(保留)保費" }, 2, 4);
            ExcelSetCell(sheet, new string[] { "系統(保留)佣金" }, 2, 5);
            ExcelSetCell(sheet, new string[] { "系統(人工調帳)筆數" }, 2, 6);
            sheet.Cells[2, 7].Style.WrapText = true;
            ExcelSetCell(sheet, new string[] { "永達-繳別繳次系統\n(人工調帳)保費" }, 2, 7);
            ExcelSetCell(sheet, new string[] { "系統(人工調帳)佣金" }, 2, 8);
            ExcelSetCell(sheet, new string[] { "轉入佣酬計算筆數" }, 2, 9);
            ExcelSetCell(sheet, new string[] { "轉入佣酬計算保費" }, 2, 10);
            ExcelSetCell(sheet, new string[] { "轉入佣酬計算佣金" }, 2, 11);
            ExcelSetCell(sheet, new string[] { "合計筆數" }, 2, 12);
            ExcelSetCell(sheet, new string[] { "合計保費" }, 2, 13);
            ExcelSetCell(sheet, new string[] { "合計佣金" }, 2, 14);
            for (int y = 1; y <= 14; y++)
            {
                sheet.Cells[2, y].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[2, y].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
            }

            ExcelSetCell(sheet, new string[] { "保險公司佣酬檢核表" }, 11, 1);
            ExcelSetCell(sheet, new string[] { "" }, 11, 2);
            ExcelSetCell(sheet, new string[] { "" }, 11, 3);
            ExcelSetCell(sheet, new string[] { "" }, 11, 4);
            ExcelSetCell(sheet, new string[] { "" }, 11, 5);
            ExcelSetCell(sheet, new string[] { "" }, 11, 6);
            sheet.Cells[11, 1, 11, 6].Merge = true;
            sheet.Cells[11, 1, 11, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ExcelSetCell(sheet, new string[] { "工作月" }, 12, 1);
            ExcelSetCell(sheet, new string[] { "次薪" }, 12, 2);
            ExcelSetCell(sheet, new string[] { "保險公司" }, 12, 3);
            ExcelSetCell(sheet, new string[] { "筆數" }, 12, 4);
            ExcelSetCell(sheet, new string[] { "保費" }, 12, 5);
            ExcelSetCell(sheet, new string[] { "佣金" }, 12, 6);
            for (int y = 1; y <= 6; y++)
            {
                sheet.Cells[12, y].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[12, y].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
            }
            #endregion

            sheet.Column(1).Width = 20;
            sheet.Cells.Style.ShrinkToFit = true;
            //字型
            sheet.Cells.Style.Font.Name = "微軟正黑體";
            //文字大小
            sheet.Cells.Style.Font.Size = 10;
            excel.SaveAs(ms);
            excel.Dispose();
            ms.Position = 0;
            return ms;
            //if (merSalSystemReports.Count != 0 || merSalCompanyReports.Count != 0)
            //{
            //    return ms;
            //}
            //else
            //{
            //    return Stream.Null;
            //}
        }

        #region EPPlus
        /// <summary>
        /// 資料填入Excel的Cell
        /// </summary>
        /// <param name="workSheet">sheet object</param>
        /// <param name="valueList">Data Array</param>
        /// <param name="rowStartPosition">Row Start Position</param>
        /// <param name="columnStartPosition">Column Start Position</param>
        private static void ExcelSetCell(ExcelWorksheet workSheet, string[] valueList, int rowStartPosition, int columnStartPosition)
        {
            var orgColPos = columnStartPosition;
            foreach (var value in valueList)
            {
                workSheet.Cells[rowStartPosition, columnStartPosition++].Value = value;

            }
            columnStartPosition = (columnStartPosition != orgColPos ? columnStartPosition - 1 : columnStartPosition);
            //下框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //上框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //右框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //左框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }

        /// <summary>
        /// 資料填入Excel的Cell
        /// </summary>
        /// <param name="workSheet">sheet object</param>
        /// <param name="valueList">Data Array</param>
        /// <param name="rowStartPosition">Row Start Position</param>
        /// <param name="columnStartPosition">Column Start Position</param>
        private static void ExcelSetCell(ExcelWorksheet workSheet, int[] valueList, int rowStartPosition, int columnStartPosition)
        {
            var orgColPos = columnStartPosition;
            foreach (var value in valueList)
            {
                workSheet.Cells[rowStartPosition, columnStartPosition++].Value = value;

            }
            columnStartPosition = (columnStartPosition != orgColPos ? columnStartPosition - 1 : columnStartPosition);
            //下框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //上框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //右框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //左框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }

        /// <summary>
        /// 資料填入Excel的Cell
        /// </summary>
        /// <param name="workSheet">sheet object</param>
        /// <param name="valueList">Data Array</param>
        /// <param name="rowStartPosition">Row Start Position</param>
        /// <param name="columnStartPosition">Column Start Position</param>
        private static void ExcelSetCell(ExcelWorksheet workSheet, double[] valueList, int rowStartPosition, int columnStartPosition)
        {
            var orgColPos = columnStartPosition;
            foreach (var value in valueList)
            {
                workSheet.Cells[rowStartPosition, columnStartPosition++].Value = value;

            }
            columnStartPosition = (columnStartPosition != orgColPos ? columnStartPosition - 1 : columnStartPosition);
            //下框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //上框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //右框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //左框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }

        /// <summary>
        /// 資料填入Excel的Cell
        /// </summary>
        /// <param name="workSheet">sheet object</param>
        /// <param name="valueList">Data Array</param>
        /// <param name="rowStartPosition">Row Start Position</param>
        /// <param name="columnStartPosition">Column Start Position</param>
        private static void ExcelSetCell(ExcelWorksheet workSheet, float[] valueList, int rowStartPosition, int columnStartPosition)
        {
            var orgColPos = columnStartPosition;
            foreach (var value in valueList)
            {
                workSheet.Cells[rowStartPosition, columnStartPosition++].Value = value;

            }
            columnStartPosition = (columnStartPosition != orgColPos ? columnStartPosition - 1 : columnStartPosition);
            //下框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //上框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //右框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //左框線
            workSheet.Cells[rowStartPosition, orgColPos, rowStartPosition, columnStartPosition].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }
        #endregion
    }
}