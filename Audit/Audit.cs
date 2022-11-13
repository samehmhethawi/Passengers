using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;

namespace Passengers
{
    /// <summary>
    /// ///////
    /// </summary>
    public class MyDataBase : Page
    {
        public static string RunQuery(string queryStr)
        {
            string retValue = "";
            object ret;
            OracleConnection conn = getConnection();

            using (conn)
            {
                conn.Open();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = queryStr;
                try
                {
                    ret = cmd.ExecuteScalar();
                }
                catch
                {
                    ret = "";
                }
                if (ret != null)
                    retValue = ret.ToString();
                conn.Close(); conn.Dispose();
            }
            return retValue;
        }
        public static string RunQuery_reader(string queryStr)
        {
            string retValue = "";

            OracleConnection conn = getConnection();
            try
            {

                OracleCommand cmd = new OracleCommand(queryStr, conn);
                conn.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    retValue = dr.GetValue(0).ToString();
                }
                else
                {
                    retValue = "0";
                }
            }
            catch (Exception)
            {
                retValue = "0";
            }
            finally
            {
                conn.Close();
            }



            return retValue;
        }


        public static OracleConnection getConnection()
        {
            return null;
            //OracleConnection con = new OracleConnection();
            //con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AuditEntities"].ConnectionString;
            //return con;
        }





        public static string GetEmpFullName(double id)
        {
            string retValue = "";
            OracleConnection conn = getConnection();
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM appmgr.USERS WHERE NB='" + id + "'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string UserNB = dr.GetValue(0).ToString();

                    retValue += dr.GetString(1);
                }
            }
            catch
            {
                retValue = "";
            }
            finally
            {
                conn.Close();
            }
            return retValue;
        }




        public static double ExecQuery(string queryStr)
        {
            double cnt = 0;
            OracleConnection conn = null;
            try
            {
                //conn = new OracleConnection(GetConnectionString());
                conn = getConnection();
                conn.Open();
                OracleCommand cmd = new OracleCommand(queryStr, conn);
                cnt = cmd.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                conn.Close();
            }
            return cnt;
        }
        #region return nb after execute insert query
        public static double ExecQueryRtNB(string queryStr)
        {
            double cnt = 0;
            OracleConnection conn = null;
            try
            {
                // conn = new OracleConnection(GetConnectionString());
                conn = getConnection();
                conn.Open();
                OracleCommand oraCommand = new OracleCommand(queryStr, conn);
                //  oraCommand.Parameters.Add(new OracleParameter("insertedNB", cnt));
                oraCommand.Parameters.Add(":insertedNB", OracleDbType.Int32, 10, cnt, ParameterDirection.ReturnValue);


                //if (oraReader.HasRows)
                //{
                //    while (oraReader.Read())
                //    {
                //        fullName = oraReader.GetString(0);
                //    }
                //}


                //OracleCommand cmd = new OracleCommand(queryStr, conn);
                oraCommand.ExecuteNonQuery();
                cnt = double.Parse(oraCommand.Parameters[":insertedNB"].Value.ToString());

            }
            catch { }
            finally
            {
                conn.Close();
            }
            return cnt;
        }

        #endregion




        //public static string GetSeqValue(string seqName)
        //{
        //    OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
        //    conn.Open();
        //    string queryStr = "SELECT " + seqName + ".NEXTVAL FROM DUAL";
        //    string retValue = "";
        //    OracleCommand sds = new OracleCommand(queryStr, conn);
        //    try
        //    {
        //        retValue = sds.ExecuteScalar().ToString();
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return retValue;
        //}


    }




}
public class Audit : ActionFilterAttribute
{

    public static string RunQuery(string queryStr)
    {
        string retValue = "";
        object ret;
        OracleConnection conn = getConnectionDetain_New();

        using (conn)
        {
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandText = queryStr;
            try
            {
                ret = cmd.ExecuteNonQuery();
            }
            catch
            {
                ret = "";
            }
            if (ret != null)
                retValue = ret.ToString();
            conn.Close(); conn.Dispose();
        }
        return retValue;
    }


    public static OracleConnection getConnectionDetain_New()
    {
        OracleConnection con = new OracleConnection();
        con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DetainEntities_New"].ConnectionString;
        return con;
    }
    public static OracleConnection getConnectionlicense()
    {
        OracleConnection con = new OracleConnection();
        con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ModelCF"].ConnectionString;
        return con;
    }

    public static OracleConnection getConnectionDetain()
    {
        OracleConnection con = new OracleConnection();
        con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DetainEntities"].ConnectionString;
        return con;
    }
    public static OracleConnection getConnection()
    {
        OracleConnection con = new OracleConnection();
        con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AuditEntities"].ConnectionString;
        return con;
    }
    public static XmlDocument GetXmlTable(string TableName, string KeyValue)
    {
        try
        {
            OracleConnection conn = getConnection();
            conn.Open();
            OracleCommand cmd = new OracleCommand("SELECT *  FROM " + TableName + " where NB =" + KeyValue, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable MyTable = new DataTable(TableName);

            MyTable.Load(dr);
            conn.Close(); conn.Dispose();
            XmlDocument xml = new XmlDocument();
            System.IO.StringWriter writer = new System.IO.StringWriter();
            MyTable.WriteXml(writer, true);
            xml.LoadXml(writer.ToString());
            string Id = xml.FirstChild.InnerXml;
            xml.LoadXml(Id);
            return xml;
        }
        catch
        { return null; }
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {



        bool notaudited = false;

        notaudited |= filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "POST";
        if (notaudited)
        {

            string param = "";
            AuditRecord audit = new AuditRecord();
            try
            {
                audit.ControllerName = filterContext.Controller.GetType().ToString();
                audit.Area = filterContext.RouteData.DataTokens.ContainsKey("area") ? filterContext.RouteData.DataTokens["area"].ToString() : "";
                audit.ActionName = filterContext.RouteData.Values["action"].ToString();
                audit.IP = filterContext.RequestContext.HttpContext.Request.UserHostAddress;
                audit.User_NB = HttpContext.Current.Session["UserNB"].ToString();
                audit.User_Name = HttpContext.Current.Session["UserName"].ToString();
                audit.Application = ConfigurationManager.AppSettings["PROG_ID"] ?? "ProcedBase";
                audit.AuditedFields_New = param;
            }
            catch
            {


            }
            finally
            {


            }
            string Table_name = "";
            string Id = "";
            foreach (KeyValuePair<string, object> parameter in filterContext.ActionParameters)
            {
                if ((parameter.Key != "request") && (audit.ActionName.Contains("Create") || audit.ActionName.Contains("Update") || audit.ActionName.Contains("Destroy")))
                {

                    try
                    {
                        XmlDocument xml = new XmlDocument();
                        //xml.LoadXml("<CARDETAINS_EN><NB>0</NB><ENDDOCNO>234</ENDDOCNO><DOCPHOTO></DOCPHOTO><ENDDOCACT></ENDDOCACT><ENDOCDATE > 01 / 01 / 0001 12:00:00 AM </ENDOCDATE ><ENDCONTRACTNB >       </ENDCONTRACTNB ><ENDNOTE ></ENDNOTE ><DIWANNO ></DIWANNO ><DIWANDAT ></DIWANDAT ></CARDETAINS_EN > ");
                        xml.LoadXml(parameter.Value.ToString());
                        Id = xml.FirstChild.FirstChild.InnerText;
                        Table_name = xml.DocumentElement.Name;


                        audit.AuditedFields_New = xml.InnerXml;
                    }
                    catch
                    {
                        audit.AuditedFields_New = "";
                    }
                }
                else
                {
                    if (parameter.Key != "request")
                    {
                        audit.AuditedFields_New = audit.AuditedFields_New + " " + parameter.Key + " :  " + parameter.Value;
                    }
                }
            }
            if (audit.ActionName.Contains("Create"))
            {
                AddLog(audit, "Create", "", "");
            }
            else if (audit.ActionName.Contains("Update"))
            {
                XmlDocument xmlNew = new XmlDocument();
                try
                {

                    xmlNew = GetXmlTable(Table_name, Id);
                    audit.AuditedFields_Old = xmlNew.InnerXml;
                }
                catch
                {

                    audit.AuditedFields_Old = "";
                }

                AddLog(audit, "Update", "", "");
            }
            else if (audit.ActionName.Contains("Destroy"))
            {
                AddLog(audit, "Destroy", "", "");
            }
            else
            {
                AddLog(audit, "", "", "");

            }

        }
        base.OnActionExecuting(filterContext);
    }

    public class AuditRecord
    {

        public string AuditedFields_New { get; set; }

        public string AuditedFields_Old { get; set; }
        public string EntityTypeAudited { get; set; }
        public DateTime DateTimeAuditRecorded { get; set; }
        public string User_Name { get; set; }

        public string User_NB { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Area { get; set; }
        public string Application { get; set; }
        public string IP { get; set; }


    }

    public static void AddLog(AuditRecord audit, string type, string KeyName, string keyValue)
    {
        string sql = "INSERT INTO APPMGR.LOG_ACTION (nb , ldate ,USER_NB, USER_NAME, CONTROLLER, ACTION, PARAMS, PARAMS_old , APPLICATION, CLIENTIP)";
        sql += " VALUES (APPMGR.MAIN_SEQUENCE.nextval , sysdate  , ";
        sql += "'" + audit.User_NB + "', '" + audit.User_Name + "', '" + audit.ControllerName + "', '" + audit.ActionName + "' ,'" + audit.AuditedFields_New + "' , '" + audit.AuditedFields_Old + "' , '" + audit.Application + "' , '" + audit.IP + "')";
        OracleConnection conn = getConnection();
        try
        {
            conn.Open();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
        catch
        {
        }
        finally
        {
            conn.Close(); conn.Dispose();
        }
    }
}