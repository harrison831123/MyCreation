using EPBKS.Db;
using SignalRAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
#region 歷程
//調整EIP實作後踢前，更改DB規則後，查詢語句做調整
#endregion
namespace SignalRAPI.Controllers
{
    public class ToApiController : BaseApiController<SignalRHub>
    {
        [AcceptVerbs("Post")]
        public async Task PersonalNotice(SignalRCode SignalRCode)
        {
            try
            {
                SignalRUser rUser = new SignalRUser();
                //DbHelperSql tsSql = new DbHelperSql("CUF");

                //tsSql.sqlParaColl.Clear();
                //tsSql.sqlParaColl.AddWithValue("@userID", SignalRCode.userID);
                //rUser.connectionId = tsSql.SQL_Query(@"select * from SignalRUser where userID = @userID order by Createtime desc").Rows[0]["ConnectionId"].ToString();
                //tsSql.SQL_Close();

                string connectStr = ConfigurationManager.ConnectionStrings["CUF"].ConnectionString;
                string sql = $@"select TOP 1 * from SignalRUser where userID = @userID order by Createtime desc";
                SqlConnection connection = new SqlConnection(connectStr);
                connection.Open();

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Parameters.AddWithValue("@userID", SignalRCode.userID);
                sqlCmd.Connection = connection;
                sqlCmd.CommandText = sql;
                sqlCmd.ExecuteNonQuery();

                SqlDataReader readList = sqlCmd.ExecuteReader();

                if (readList.HasRows)
                {
                    while (readList.Read())
                    {
                        rUser.connectionId = readList["ConnectionID"].ToString();
                    };
                }
                connection.Close();

                DateTime datenow = DateTime.Now;
                string date = datenow.ToString("yyyy-MM-dd HH:mm");
                int id = savemessage(SignalRCode.message, SignalRCode.userID, SignalRCode.name, datenow, SignalRCode.SystemType);

                if (rUser.connectionId == null)
                {
                    return;
                }                
               
                await Clients.Client(rUser.connectionId).SendAsync(SignalRCode.message, SignalRCode.name, id, date, SignalRCode.SystemType);
            }
            catch(Exception ex)
            {
                DbHelperSql tsSql = new DbHelperSql("CUF");

                tsSql.sqlParaColl.Clear();
                tsSql.sqlParaColl.AddWithValue("@error", "Toapi錯誤");
                tsSql.sqlParaColl.AddWithValue("@createtime", DateTime.Now);
                tsSql.sqlParaColl.AddWithValue("@log", "ID:" + SignalRCode.userID + " EX:" +ex.Message);
                int id = tsSql.SQL_ExecuteAdd_OLTP(@"insert into signalrlog (error,createtime,log)  VALUES(@error,@createtime,@log)");

                tsSql.SQL_Close();
            }
          
        }



        [NonAction]
        public int savemessage(string message, string userID,string name, DateTime date,string SystemType)
        {
            DbHelperSql tsSql = new DbHelperSql("CUF");

            tsSql.sqlParaColl.Clear();
            tsSql.sqlParaColl.AddWithValue("@userID", userID);
            tsSql.sqlParaColl.AddWithValue("@Name", name);
            tsSql.sqlParaColl.AddWithValue("@message", message);
            tsSql.sqlParaColl.AddWithValue("@type", 1);
            tsSql.sqlParaColl.AddWithValue("@Createtime", date);
            tsSql.sqlParaColl.AddWithValue("@SystemType", SystemType);
            int id = tsSql.SQL_ExecuteAdd_OLTP(@"insert into SignalRMessage (Message,UserID,Type,Name,Createtime,SystemType)  VALUES(@message,@UserID,@type,@Name,@Createtime,@SystemType)");


            tsSql.SQL_Close();
            return id;
        }

    }
}