using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Passengers.Controllers
{
    public class PassengersAccountController : Controller
    {
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        // GET: PassengersAccount
        public ActionResult Index()
        {
            CodesController cc = new CodesController();
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
            if (!IsAdmin && !IsAdminCity)
            {
                var mycitynb = Utility.MyCityNb();
                return RedirectToAction("Index_Detalis", new { citynb = mycitynb });
            }
            else
            {
                return RedirectToAction("Index_city");

            }
           
        }
         

        public ActionResult Index_city()
        {
            return View();
        }

        public ActionResult Index_Detalis(int citynb)
        {
            CodesController cc = new CodesController();
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
            if (!IsAdmin && !IsAdminCity)
            {
                var mycitynb = Utility.MyCityNb();
                if (mycitynb != citynb)
                return RedirectToAction("Index");
            }
            var amount = db.TRPASSENGERS_CREDITS.Find(citynb).AMOUNT;
            var cityname = db.ZCITYS.Find(citynb).NAME;
            ViewBag.amount = amount;
            ViewBag.cityname = cityname;
            ViewBag.citynb = citynb;
            return View();
        }

        public ActionResult TRGET_ORDERS_Index(int citynb)
        {

            CodesController cc = new CodesController();
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
            if (!IsAdmin && !IsAdminCity)
            {
                var mycitynb = Utility.MyCityNb();
                if (mycitynb != citynb)
                    return RedirectToAction("Index");
            }

            ViewData["STATUS"] = db.TRSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewBag.cityname = db.ZCITYS.Find(citynb).NAME;
            ViewBag.citynb = citynb;
            return View();

        }

        public ActionResult TRGET_ORDERS_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = " select * from TRGET_ORDERS where 1 = 1 ";

            var NO = Request.Form["NO"].Trim();
            var DATEStart = Request.Form["DATEStart"].Trim();
            var ssGDATE = Request.Form["ssGDATE"].Trim();
            var Status = Request.Form["Status"].Trim();
            var StrSessArc = Request.Form["StrSessArc"].Trim();
            var citynb = Request.Form["citynb"].Trim();


            if (citynb != "")
            {
                sql += " and CITYNB = " + citynb;
            }
            if (NO != "")
            {
                sql += " and NO like '" + NO + "'";
            }

            if (ssGDATE != "")
            {
                sql += " and TRUNC(GDATE) = TO_DATE('" + ssGDATE + "','DD/MM/YYYY') ";
            }
            if (DATEStart != "")
            {
                sql += "  AND (TO_DATE ('" + DATEStart + "', 'DD/MM/YYYY') BETWEEN TRUNC (FROMDATE)  AND TRUNC(TODATE)) ";
            }

           
            if (StrSessArc != "")
            {
                sql += " and IS_ARCHIVED =" + StrSessArc;
            }



            sql += " order by nb desc";


            var data = db.Database.SqlQuery<TRGET_ORDERS>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                CITYNB = commm.CITYNB,
                NO = commm.NO,
                GDATE = commm.GDATE,
                AMOUNT = commm.AMOUNT,
                FROMDATE = commm.FROMDATE,
                STATUS = commm.STATUS,
                NOTES = commm.NOTES,
                CONFIRM_DATE = commm.CONFIRM_DATE,
                TODATE = commm.TODATE,
                IS_ARCHIVED = commm.IS_ARCHIVED,
                FTP_PATH = commm.FTP_PATH,



                Seq = (request.Page - 1) * request.PageSize + (++index)
            });

            return Json(result);



        }

        public ActionResult GetResult(int citynb , string FROMDATE, string TODATE)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ar-sa");
           

            DateTime FROMDATE2 = DateTime.ParseExact(FROMDATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime TODATE2 = DateTime.ParseExact(TODATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            string sql = "BEGIN VEHICLES.PASSENGERS_PKG.GETRESULT_PASSENGER_TAX(:PCITYNB,:FROMDATE, :TODATE,:CARBILLS_COUNT,:TOTVAL); END;";
            var PCITYNB = new OracleParameter("PCITYNB", OracleDbType.Double, citynb, ParameterDirection.Input);
            var PFROMDATE = new OracleParameter("FROMDATE", OracleDbType.Date, FROMDATE2, ParameterDirection.Input);
            var PTODATE = new OracleParameter("TODATE", OracleDbType.Date, TODATE2, ParameterDirection.Input);


            var bills = new OracleParameter("CARBILLS_COUNT", OracleDbType.Double, ParameterDirection.Output);
            var totals = new OracleParameter("TOTVAL", OracleDbType.Double, ParameterDirection.Output);
            db.Database.ExecuteSqlCommand(sql, PCITYNB, PFROMDATE, PTODATE,bills, totals);
            var x = bills.Value;
            var y = totals.Value;
            return Json(new { success = true, CARBILLS_COUNT2 = x.ToString(), TOTVA2L = y.ToString() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TRGET_ORDERS_Create(HttpPostedFileBase Files ,string NO , DateTime GDATE, int CITYNB, string FROMDATE, string TODATE,string EXCLUDES, string INCLUDES ,long? PTOTALVAL)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    TRGET_ORDERS Model = new TRGET_ORDERS();
                    Model.NO = NO;
                    Model.GDATE = GDATE;
                    Model.CITYNB= CITYNB;
                    long? totalval = 0;
                    long? EXCLUDESval = 0;
                    long? INCLUDESval = 0;
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ar-sa");


                    DateTime FROMDATE2 = DateTime.ParseExact(FROMDATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime TODATE2 = DateTime.ParseExact(TODATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    Model.STATUS = 0;
                    Model.IS_ARCHIVED = false;
                    Model.FROMDATE = FROMDATE2;
                    Model.TODATE = TODATE2;
                    db.TRGET_ORDERS.Add(Model);

                    db.SaveChanges();
                    var NB = Model.NB;
                    if (EXCLUDES != null)
                    {
                        var xx = EXCLUDES.Split(',');
                        foreach (var item in xx)
                        {
                            if (item != null)
                            {
                                TRGET_ORDER_EXCLUDES ex = new TRGET_ORDER_EXCLUDES();
                                ex.NB = 1;
                                ex.GNB = NB;
                                ex.CARPROCEDNB = long.Parse(item);
                                ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + ") FROM DUAL").FirstOrDefault();
                                db.TRGET_ORDER_EXCLUDES.Add(ex);
                                db.SaveChanges();
                                EXCLUDESval += ex.AMOUNT;

                            }
                           
                        }
                    }
                    if (INCLUDES != null)
                    {
                        var ss = INCLUDES.Split(',');
                        foreach (var item in ss)
                        {
                            if (item != null)
                            {
                                TRGET_ORDER_INCLUDES ex = new TRGET_ORDER_INCLUDES();
                                ex.NB = 1;
                                ex.GNB = NB;
                                ex.CARPROCEDNB = long.Parse(item);
                                ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + ") FROM DUAL").FirstOrDefault();
                                db.TRGET_ORDER_INCLUDES.Add(ex);
                                db.SaveChanges();
                                INCLUDESval += ex.AMOUNT;
                            }
                        }
                    }

                    byte[] fileContent = null;
                    using (var reader = new System.IO.BinaryReader(Files.InputStream))
                    {
                        fileContent = reader.ReadBytes(Files.ContentLength);
                    }

                    var date = DateTime.Now;
                    var pathNameYear = date.ToString("yyyy");

                    pathNameYear += "/" + Model.CITYNB + "/";

                    var FTPFullPath = ConfigurationManager.AppSettings["FtpHomeTRGETORDERS"];
                    FTPFullPath += pathNameYear;

                    var uploadedFullPath = CodesController.UploadFile(fileContent, FTPFullPath, Files.FileName, FTPFullPath, NB);

                    Model.FTP_PATH = uploadedFullPath;
                    Model.IS_ARCHIVED = true;
                    totalval = PTOTALVAL + INCLUDESval - EXCLUDESval;
                    Model.AMOUNT = (long)totalval;
                    db.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    var ss = validation.OracleExceptionValidation(ex);
                    return Json(new { success = false, responseText = ss });
                }
            }
                
        }


        public ActionResult GetReport(long NB)
        {
            try
            {
                CodesController cc = new CodesController();
                var ses = db.TRGET_ORDERS.Find(NB);
                bool IsAdmin = cc.IsAdmin();
                bool IsAdminCity = cc.IsAdminCity();
                if (!IsAdmin && !IsAdminCity)
                {

                    var mycitynb = Utility.MyCityNb();
                    if (ses.CITYNB != mycitynb)
                    {
                        return RedirectToAction("Index");
                    }
                }
                var path = ses.FTP_PATH;
                var FTPURL = ConfigurationManager.AppSettings["FtpURL"];
                string fileName = Path.GetFileName(path);
                string mimeType = MimeMapping.GetMimeMapping(fileName);
                string ReportURL = path;
                //  byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
                byte[] FileBytes = CodesController.GetFileContent(path);

                return File(FileBytes, mimeType);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}