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
    public class GroupApiController : BaseApiController<SignalRHub>
    {
        [AcceptVerbs("Post")]
        public async Task GroupNotice(SignalRCode SignalRCode)
        {
            List<SignalRUser> LUser = new List<SignalRUser>();            
            LUser = Callgroup(SignalRCode.groupname);
            if (LUser.Count != 0) 
            {
                for (int i = 0; i < LUser.Count; i++)
                {
                    SignalRUser rUser = new SignalRUser();

                    string connectStr = ConfigurationManager.ConnectionStrings["CUF"].ConnectionString;
                    string sql = $@"select TOP 1 * from SignalRUser where userID = @userID order by Createtime desc";
                    SqlConnection connection = new SqlConnection(connectStr);
                    connection.Open();

                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Parameters.AddWithValue("@userID", LUser[i].userID);
                    sqlCmd.Connection = connection;
                    sqlCmd.CommandText = sql;
                    try
                    {
                        SqlDataReader readList = sqlCmd.ExecuteReader();

                        if (readList.HasRows)
                        {
                            while (readList.Read())
                            {
                                rUser.connectionId = readList["ConnectionID"].ToString();
                            };
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        DbHelperSql tsSql = new DbHelperSql("CUF");

                        tsSql.sqlParaColl.Clear();
                        tsSql.sqlParaColl.AddWithValue("@error", "Groupapi錯誤");
                        tsSql.sqlParaColl.AddWithValue("@createtime", DateTime.Now);
                        tsSql.sqlParaColl.AddWithValue("@log", "下一關ID:" + SignalRCode.userID + "EX:" + ex.Message);
                        int id = tsSql.SQL_ExecuteAdd_OLTP(@"insert into signalrlog (error,createtime,log)  VALUES(@error,@createtime,@log)");

                        tsSql.SQL_Close();

                    }

                    if (rUser.connectionId != null)
                    {
                        DateTime datenow = DateTime.Now;
                        string date = datenow.ToString("yyyy-MM-dd HH:mm");
                        int id = savemessage(SignalRCode.message, LUser[i].userID, SignalRCode.name, datenow, SignalRCode.SystemType);
                        //發送
                        await Clients.Client(rUser.connectionId).show(SignalRCode.name, SignalRCode.message, id, date, SignalRCode.SystemType);
                    }
                    else
                    {
                        continue;
                    }              
                }
            }
        }

        [NonAction]
        public List<SignalRUser> Callgroup(string groupname)
        {         
            string connectStr = ConfigurationManager.ConnectionStrings["CUF"].ConnectionString;
            string sql = $@"select AccountId from AccountGroupRelation a join AccountGroup b on a.GroupNo = b.GroupNo where GroupName = @GroupName";
            List<SignalRUser> LUser = new List<SignalRUser>();
            SqlConnection connection = new SqlConnection(connectStr);
            connection.Open();

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Parameters.AddWithValue("@GroupName",groupname);
            sqlCmd.Connection = connection;
            sqlCmd.CommandText = sql;
            try
            {
                SqlDataReader readList = sqlCmd.ExecuteReader();

                if (readList.HasRows)
                {
                    while (readList.Read())
                    {
                        SignalRUser s = new SignalRUser
                        {
                            userID = readList["AccountId"].ToString()
                        };
                        LUser.Add(s);
                    };
                }
                connection.Close();
            }
            catch(Exception ex)
            {
                DbHelperSql tsSql = new DbHelperSql("CUF");

                tsSql.sqlParaColl.Clear();
                tsSql.sqlParaColl.AddWithValue("@error", "Callgroup錯誤");
                tsSql.sqlParaColl.AddWithValue("@createtime", DateTime.Now);
                tsSql.sqlParaColl.AddWithValue("@log", "EX:" + ex.Message);
                int id = tsSql.SQL_ExecuteAdd_OLTP(@"insert into signalrlog (error,createtime,log)  VALUES(@error,@createtime,@log)");

                tsSql.SQL_Close();

            }
           
            return LUser;
        }

        //[NonAction]
        //public void savemessage(string message, string userID, string name)
        //{
        //    DbHelperSql tsSql = new DbHelperSql("CUF");

        //    tsSql.sqlParaColl.Clear();
        //    tsSql.sqlParaColl.AddWithValue("@userID", userID);
        //    tsSql.sqlParaColl.AddWithValue("@Name", name);
        //    tsSql.sqlParaColl.AddWithValue("@message", message);
        //    tsSql.sqlParaColl.AddWithValue("@type", 1);
        //    tsSql.SQL_Exec_OLTP(@"insert into SignalRMessage (Message,UserID,Type,Name)  VALUES(@message,@UserID,@type,@Name)");
        //    tsSql.SQL_Close();
        //}

        [NonAction]
        public int savemessage(string message, string userID, string name, DateTime date,string SystemType)
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
