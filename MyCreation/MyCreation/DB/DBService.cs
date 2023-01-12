using EPBKS.Db;
using MyCreation.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        /// 大批調整bulkinsert
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