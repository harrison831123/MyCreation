using EPBKS.Db;
using MyCreation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyCreation.DBService
{
    public class DBService
    {
        DbHelperSql tssql = null;
        /// <summary>
        /// 檢查會議時間有無重複
        /// </summary>
        public MeetingDetail ChkMeetingTime(int MDID, DateTime SDate, DateTime EDate)
        {
            tssql = new DbHelperSql("H2O");
            MeetingDetail result = new MeetingDetail();
            string sql = @"with t1 as(select * from H2O.dbo.MeetingOrder)";
            sql += " select * from t1,MeetingDevice D,Meeting A";
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
    }
}