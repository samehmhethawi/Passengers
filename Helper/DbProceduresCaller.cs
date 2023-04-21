using Oracle.ManagedDataAccess.Client;
using Proced.DataAccess.Models.CF;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ProcedBase.Site.Helper
{
    public class DbProceduresCaller
    {
        public static ProcedContext db = new ProcedContext();
        private readonly ProcedContext Context;
        public DbProceduresCaller(ProcedContext context)
        {
            Context = context;
        }

        public virtual int REQUEST_NEW_BILL_NUM(ObjectParameter lAST_NUM, ObjectParameter cUR_DATE)
        {
            return ((IObjectContextAdapter)Context).ObjectContext.ExecuteFunction("REQUEST_NEW_BILL_NUM", lAST_NUM, cUR_DATE);
        }

        public DateNo REQUEST_NEW_CONTRACT_NUM()
        {
            DateTime cdate = DateTime.Now;
            DateNo result = new DateNo();
            long lasContractNo = -1;
        reReg:
            string queryContractNo = "SELECT VEHICLES.SEQ_LASTNUMBERS_CONTRACT.NEXTVAL FROM DUAL";
            try
            {
                lasContractNo = db.Database.SqlQuery<long>(queryContractNo).FirstOrDefault();
            }
            catch (Exception)
            {
                goto reReg;
            }
            result.CUR_DATE = cdate;
            result.LAST_NUM = lasContractNo;
            return result;
        }

        public virtual int REQUEST_NEW_NUM(Nullable<decimal> pTYPE, ObjectParameter lAST_NUM, ObjectParameter cUR_DATE)
        {
            var pTYPEParameter = pTYPE.HasValue ?
                new ObjectParameter("PTYPE", pTYPE) :
                new ObjectParameter("PTYPE", typeof(decimal));

            return ((IObjectContextAdapter)Context).ObjectContext.ExecuteFunction("REQUEST_NEW_NUM", pTYPEParameter, lAST_NUM, cUR_DATE);
        }

        public static void GetLastNum(out long lastnum, out DateTime cur_date, string sp)
        {
            int ret;
            string sql = "BEGIN VEHICLES." + sp + "(:LAST_NUM,:CUR_DATE); END;";

            OracleParameter[] oparams = {
                        new OracleParameter("LAST_NUM", OracleDbType.Long, 12, null, ParameterDirection.Output),
                        new OracleParameter("CUR_DATE", OracleDbType.Date, 100, null, ParameterDirection.Output),
                    };
        ReReqPro:
            try
            {
                ret = db.Database.ExecuteSqlCommand(sql, oparams);

                lastnum = Convert.ToInt64(oparams[0].Value.ToString());
                cur_date = Convert.ToDateTime(oparams[1].Value.ToString());
            }
            catch (Exception ee)
            {
                goto ReReqPro;
                lastnum = -1;
                cur_date = DateTime.Now;

            }
        }

        public bool SaveNewContract(long? stepNb, 
            string description, string intgralinfo, string extranote, string noteplus, string note,
            int contractnb, string contractno, DateTime? contractdate, out string message)
        {
            message = string.Empty;
            try
            {
                string sql = "BEGIN CONTRACT_PKG.SAVE_TMP_CONTRACT (:PSTEPNB, :PDESCRIPTION, :PINTGRALINFO, :PEXTRANOTE, :NOTEPLUS, :NOTE, :PCONTRACTNB, :PCONTRACTNO, :PCONTRACTDATE, :MESSAGE); END; ";
                OracleParameter[] oparams = {
                        new OracleParameter("PSTEPNB", OracleDbType.Int64, stepNb, ParameterDirection.Input),
                        new OracleParameter("PDESCRIPTION", OracleDbType.Varchar2, 4000, description, ParameterDirection.Input),
                        new OracleParameter("PINTGRALINFO", OracleDbType.Varchar2, 4000, intgralinfo, ParameterDirection.Input),
                        new OracleParameter("PEXTRANOTE", OracleDbType.Varchar2, 4000, extranote, ParameterDirection.Input),
                        new OracleParameter("NOTEPLUS", OracleDbType.Varchar2, 4000, noteplus, ParameterDirection.Input),
                        new OracleParameter("NOTE", OracleDbType.Varchar2, 4000, note, ParameterDirection.Input),
                        new OracleParameter("PCONTRACTNB", OracleDbType.Int16, contractnb, ParameterDirection.Input),
                        new OracleParameter("PCONTRACTNO", OracleDbType.Varchar2, 255, contractno, ParameterDirection.Input),
                        new OracleParameter("PCONTRACTDATE", OracleDbType.Date, contractdate, ParameterDirection.Input),
                        new OracleParameter("MESSAGE", OracleDbType.Varchar2, 4000, message, ParameterDirection.Output),
                };
                db.Database.ExecuteSqlCommand(sql, oparams);
                message = oparams[9].Status == OracleParameterStatus.NullFetched ? string.Empty : oparams[9].Value.ToString();
                return string.IsNullOrEmpty(message);
            }
            catch (Exception e)
            {
                message = e.StackTrace;
                return false;
            }
        }
        public bool ContractIsSavedAsNativeDoc(long contractnb)
        {
            return db.NATIVE_DOC.Count(d => d.DNB == contractnb) > 0;
        }
        public bool ConfirmContract(long? stepNb, long? contractnb, long? carprocedsieznb, out string message)
        {
            message = string.Empty;
            try
            {
                string sql = "BEGIN CONTRACT_PKG.CONFIRM_CONTRACT (:PSTEPNB, :PCONTRACTNB, :PCARPROCEDSIEZNB, :MESSAGE); END; ";
                OracleParameter[] oparams = {
                        new OracleParameter("PSTEPNB", OracleDbType.Int64, stepNb, ParameterDirection.Input),
                        new OracleParameter("PCONTRACTNB", OracleDbType.Int64, contractnb, ParameterDirection.Input),
                        new OracleParameter("PCARPROCEDSIEZNB", OracleDbType.Int64, carprocedsieznb, ParameterDirection.Input),
                        new OracleParameter("MESSAGE", OracleDbType.Varchar2, 4000, message, ParameterDirection.Output),
                };
                db.Database.ExecuteSqlCommand(sql, oparams);
                message = oparams[3].Status == OracleParameterStatus.NullFetched ? string.Empty : oparams[3].Value.ToString();
                return string.IsNullOrEmpty(message);
            }
            catch(Exception e)
            {
                SaveErrorDB(HttpContext.Current.Session["UserNB"].ToString(), MyOwnData.MyFullName(), "contracts", "confirm", e.Message, e.StackTrace);
                return false;
            }
        }
        public void SaveErrorDB(string userNb, string userName, string controller, string action, string message, string stack)
        {
            try
            {
                string sql = "BEGIN INSERT INTO APPMGR.LOG_ERRORS (USER_NB, USER_NAME, CONTROLLER, ACTION, MESSAGE, TRACE)";
                sql += " VALUES (:CONTROLLER,:ACTION,:MESSAGE,:TRACE); END; ";
                OracleParameter[] oparams = {
                        new OracleParameter("USER_NB", OracleDbType.Varchar2, 255, userNb, ParameterDirection.Input),
                        new OracleParameter("USER_NAME", OracleDbType.Varchar2, 255, userName, ParameterDirection.Input),
                        new OracleParameter("CONTROLLER", OracleDbType.Varchar2, 255, controller, ParameterDirection.Input),
                        new OracleParameter("ACTION", OracleDbType.Varchar2, 255, action, ParameterDirection.Input),
                        new OracleParameter("MESSAGE", OracleDbType.Varchar2, 4000, message, ParameterDirection.Input),
                        new OracleParameter("TRACE", OracleDbType.Varchar2, 4000, stack, ParameterDirection.Input),
                };
                db.Database.ExecuteSqlCommand(sql, oparams);
            }
            catch
            {
            }
        }
        public DateNo REQUEST_NEW_TAX_NUM()
        {
            long ln = 0;
            DateTime cdate = DateTime.Now;
            GetLastNum(out ln, out cdate, "REQUEST_NEW_TAX_NUM");
            DateNo result = new DateNo()
            {
                CUR_DATE = cdate,
                LAST_NUM = ln
            };
            return result;
        }

        public DateNo REQUEST_NEW_Bill_NUM()
        {
            long ln = 0;
            DateTime cdate = DateTime.Now;
            GetLastNum(out ln, out cdate, "REQUEST_NEW_BILL_NUM");
            DateNo result = new DateNo()
            {
                CUR_DATE = cdate,
                LAST_NUM = ln
            };
            return result;
        }

        public DateNo REQUEST_NEW_SND_NUM()
        {
            long ln = 0;
            DateTime cdate = DateTime.Now;
            GetLastNum(out ln, out cdate, "REQUEST_NEW_SND_NUM");
            DateNo result = new DateNo()
            {
                CUR_DATE = cdate,
                LAST_NUM = ln
            };
            return result;
        }

        public DateNo REQUEST_NEW_Proced_NUM()
        {
            long ln = 0;
            DateTime cdate = DateTime.Now;
            GetLastNum(out ln, out cdate, "REQUEST_NEW_Proc_NUM");
            DateNo result = new DateNo()
            {
                CUR_DATE = cdate,
                LAST_NUM = ln
            };
            return result;
        }

        public long REQUEST_NEW_CARNB(int? cityNb)
        {
            long carNb = 0;

            object ret;


            string sql = "BEGIN VEHICLES.REQUEST_NEW_CARNB(:PCITYNB,:LAST_NUM);END;";

            var cityNbParam = new OracleParameter("PCITYNB", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            cityNbParam.Value = cityNb;
            var carNbParam = new OracleParameter("LAST_NUM", OracleDbType.Long, 12, null, ParameterDirection.Output);
            OracleParameter[] oparams = { cityNbParam, carNbParam };

            try
            {
                ret = db.Database.ExecuteSqlCommand(sql, oparams);
                carNb = Int64.Parse(oparams[1].Value.ToString());
            }
            catch (Exception)
            {
                carNb = -1;



            }
            return carNb;
        }
        public long REQUEST_NEW_PENALTY_NUM(int? cityNb)
        {
            long carNb = 0;
            object ret;
            string sql = "BEGIN VEHICLES.REQUEST_NEW_PENALTY_NUM(:PCITYNB,:LAST_NUM);END;";
            var cityNbParam = new OracleParameter("PCITYNB", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            cityNbParam.Value = cityNb;
            var carNbParam = new OracleParameter("LAST_NUM", OracleDbType.Long, 12, null, ParameterDirection.Output);
            OracleParameter[] oparams = { cityNbParam, carNbParam };

            try
            {
                ret = db.Database.ExecuteSqlCommand(sql, oparams);
                carNb = Int64.Parse(oparams[1].Value.ToString());
            }
            catch (Exception)
            {
                carNb = -1;
            }
            return carNb;
        }
        public bool UPDATE_LICENSE_CREDIT(int? cityNb, int? userNb, int? qnty, int? fPeriod/*, DateTime pd*/)
        {
            bool result = false;
            object ret;
            string sql = "BEGIN VEHICLES.UPDATE_LICENSE_CREDIT(:PC,:PU, :QNTY, :FPERIOD, :PD ,:DONE);END;";

            var cityNbParam = new OracleParameter("PC", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            cityNbParam.Value = cityNb;
            var userNbParam = new OracleParameter("PU", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            userNbParam.Value = userNb;
            var qntyParam = new OracleParameter("QNTY", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            qntyParam.Value = qnty;
            var fPeriodParam = new OracleParameter("FPERIOD", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            fPeriodParam.Value = qnty;
            var dateParam = new OracleParameter("PD", OracleDbType.Date, ParameterDirection.Input);
            dateParam.Value = null;
            var outPutParam = new OracleParameter("DONE", OracleDbType.Int32, 10, null, ParameterDirection.Output);
            OracleParameter[] oparams = { cityNbParam, userNbParam, qntyParam, fPeriodParam, dateParam, outPutParam };

            try
            {
                ret = db.Database.ExecuteSqlCommand(sql, oparams);
                int doneOut = int.Parse(oparams[5].Value.ToString());
                if (doneOut == 1)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;

            }
            return result;
        }
        public bool UPDATE_SND_CREDIT(int? cityNb, int? userNb, int? qnty, int? fPeriod/*, DateTime pd*/)
        {
            bool result = false;
            object ret;

            string sql = "BEGIN VEHICLES.UPDATE_SND_CREDIT(:PC,:PU, :QNTY, :FPERIOD, :PD ,:DONE);END;";

            var cityNbParam = new OracleParameter("PC", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            cityNbParam.Value = cityNb;
            var userNbParam = new OracleParameter("PU", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            userNbParam.Value = userNb;
            var qntyParam = new OracleParameter("QNTY", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            qntyParam.Value = qnty;
            var fPeriodParam = new OracleParameter("FPERIOD", OracleDbType.Int32, 10, null, ParameterDirection.Input);
            fPeriodParam.Value = qnty;
            var dateParam = new OracleParameter("PD", OracleDbType.Date, ParameterDirection.Input);
            dateParam.Value = null;
            var outPutParam = new OracleParameter("DONE", OracleDbType.Int32, 10, null, ParameterDirection.Output);
            OracleParameter[] oparams = { cityNbParam, userNbParam, qntyParam, fPeriodParam, dateParam, outPutParam };

            try
            {
                ret = db.Database.ExecuteSqlCommand(sql, oparams);
                int doneOut = int.Parse(oparams[5].Value.ToString());
                if (doneOut == 1)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;

            }


            return result;
        }

        public int PERIOD_IS_HOLIDAY(DateTime? fromDate, DateTime? toDate)
        {
            int result = 1;
            try
            {

                string sql = "SELECT VEHICLES.PERIOD_IS_HOLIDAY(:FROMDATE_,:TODATE_) FROM DUAL ";

                OracleParameter[] oparams = { new OracleParameter("FROMDATE_", fromDate), new OracleParameter("TODATE_", toDate) };


                var ret = db.Database.ExecuteSqlCommand(sql, oparams);
                if (ret == null || ret + "" == "0")
                    result = 0;
                else if (ret + "" == "1")
                    result = 1;


            }
            catch (Exception)
            {
            }
            return result;
        }

        public void BUILD_PROCED_TAXES(long? PPROCEDNB, long? PCITYNB)
        {
            try
            {
                OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ModelCF"].ConnectionString);
                conn.Open();
                OracleCommand cmd_hor = new OracleCommand("BEGIN VEHICLES.TAX_PKG.BUILD_PROCED_TAXES(" + PPROCEDNB + "," + PCITYNB + ", SYSDATE);END;", conn);

                try
                {
                    OracleDataReader dr_hor = cmd_hor.ExecuteReader();
                    dr_hor.Close();
                    dr_hor.Dispose();
                    cmd_hor.Dispose();
                }
                catch (Exception)
                {
                }

                conn.Close(); conn.Dispose();
            }
            catch (Exception)
            {

            }
            //object ret;
            //     string sql = "BEGIN VEHICLES.TAX_PKG.BUILD_PROCED_TAXES(:PPROCEDNB,:PCITYNB, SYSDATE);END;";
            //OracleParameter[] oparams = {
            //            new OracleParameter("PPROCEDNB", OracleDbType.Long, 12, PPROCEDNB, ParameterDirection.Input),
            //            new OracleParameter("PCITYNB", OracleDbType.Int32, 12, PCITYNB, ParameterDirection.Input),
            //        };

            //try
            //{
            //    ProcedContext db3 = new ProcedContext();
            //    ret = db.Database.ExecuteSqlCommand(sql, oparams);
            //    db3.Dispose();
            //}
            //catch (Exception ex)
            //{


            //}


        }

        public string CHECK_ATTRIBS(long? PPROCEDNB, int? cityNb)
        {
            string result = "";
            #region Oracle_Conn
            OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ModelCF"].ConnectionString);
            try
            {
                conn.Open();
                OracleCommand cmd_hor = new OracleCommand("BEGIN VEHICLES.TAX_PKG.PCHECK_ATTRIBS(:PPROCEDNB,:PCITYNB, :VIDE_ATTRIBS);END;", conn);
                var carProcedNbParam = new OracleParameter("PPROCEDNB", OracleDbType.Long, PPROCEDNB, ParameterDirection.Input);
                var cityNbParam = new OracleParameter("PCITYNB", OracleDbType.Int32, cityNb, ParameterDirection.Input);
                var nullArrtibsParam = new OracleParameter("VIDE_ATTRIBS", OracleDbType.Varchar2, ParameterDirection.Output);
                nullArrtibsParam.Size = 1500;
                nullArrtibsParam.DbType = DbType.String;
                cmd_hor.Parameters.Add(carProcedNbParam);
                cmd_hor.Parameters.Add(cityNbParam);
                cmd_hor.Parameters.Add(nullArrtibsParam);
                cmd_hor.ExecuteScalar();
                var x = nullArrtibsParam.Value;
                if (x != null)
                    result = x.ToString();
                cmd_hor.Dispose();
            }
            catch (Exception ex)
            {
            }
            conn.Close(); conn.Dispose();
            #endregion
            return result;
        }
        public long ROUND_NUMBER(long? PTAXNB, decimal? VAL)
        {
            try
            {
                string sql = "SELECT TAX_PKG.ROUND_NUMBER(" + PTAXNB + ", " + VAL + ") FROM DUAL ";
                long roundedValue = db.Database.SqlQuery<long>(sql).FirstOrDefault();
                if (roundedValue % 10 > 0)
                {
                    roundedValue = roundedValue - (roundedValue % 10) + 10;
                }
                return roundedValue;
            }
            catch (Exception)
            {
                var roundedValue = (long)VAL.GetValueOrDefault(0);
                if (roundedValue % 10 > 0)
                {
                    roundedValue = roundedValue - (roundedValue % 10) + 10;
                }
                return roundedValue;
            }
        }
    }
}