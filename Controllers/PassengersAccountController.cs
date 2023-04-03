using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

            //var SESNO = Request.Form["SESNO"].Trim();
            //var SESDATESTART = Request.Form["SESDATESTART"].Trim();
            //var SESDATEEND = Request.Form["SESDATEEND"].Trim();
            //var STATUS = Request.Form["STATUS"].Trim();
            //var SESCITYNB = Request.Form["SESCITYNB"].Trim();
            //var StrSessArc = Request.Form["StrSessArc"].Trim();



            //if (SESNO != "")
            //{
            //    sql += " and SESNO like '%" + SESNO + "%'";
            //}
            //if (STATUS != "")
            //{
            //    sql += " and STATUS =" + STATUS;
            //}

            //if (SESDATESTART != "")
            //{
            //    sql += " and TRUNC(SESDATE) >= TO_DATE('" + SESDATESTART + "','DD/MM/YYYY') ";
            //}

            //if (SESDATEEND != "")
            //{
            //    sql += " and TRUNC(SESDATE) <= TO_DATE('" + SESDATEEND + "','DD/MM/YYYY') ";
            //}
            //if (StrSessArc != "")
            //{
            //    sql += " and IS_ARCHIVED =" + StrSessArc;
            //}

            //CodesController bb = new CodesController();

            //var ci = bb.GetCityForRead();

            //if (ci == "0")
            //{
            //    if (SESCITYNB != "")
            //    {
            //        sql += " and SESCITYNB =" + SESCITYNB;
            //    }

            //}
            //else
            //{
            //    sql += " and SESCITYNB =" + ci;
            //}
            ////  var total = db.Database.SqlQuery<int>(" select /*+ NO_CPU_COSTING */  count(nb)  FROM ("+sql+")").SingleOrDefault();

            sql += " order by nb desc";
            ////OFFSET " + request.PageSize * (request.Page - 1) + " ROWS FETCH NEXT " + request.PageSize + " ROWS ONLY ";


            ////var sql_ = "select ROWNUM +" + (request.Page - 1) + "*" + request.PageSize + " seq, aa.*from(" + sql + ")aa";

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
            //  result.Total = total;

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

        public ActionResult TRGET_ORDERS_Create(TRGET_ORDERS model, string FROMDATE, string TODATE, List<long?> EXCLUDES, List<long?> INCLUDES)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    long? totalval = 0;
                    long? EXCLUDESval = 0;
                    long? INCLUDESval = 0;
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ar-sa");


                    DateTime FROMDATE2 = DateTime.ParseExact(FROMDATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime TODATE2 = DateTime.ParseExact(TODATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    model.STATUS = 0;
                    model.IS_ARCHIVED = false;
                    model.FROMDATE = FROMDATE2;
                    model.TODATE = TODATE2;
                    db.TRGET_ORDERS.Add(model);

                    db.SaveChanges();
                    var NB = model.NB;
                    if (EXCLUDES != null)
                    {
                        foreach (var item in EXCLUDES)
                        {
                            TRGET_ORDER_EXCLUDES ex = new TRGET_ORDER_EXCLUDES();
                            ex.NB = 1;
                            ex.GNB = NB;
                            ex.CARPROCEDNB = item;
                            ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + ") FROM DUAL").FirstOrDefault();
                            db.TRGET_ORDER_EXCLUDES.Add(ex);
                            db.SaveChanges();
                            EXCLUDESval += ex.AMOUNT;
                        }
                    }
                    if (INCLUDES != null)
                    {
                        foreach (var item in INCLUDES)
                        {
                            TRGET_ORDER_INCLUDES ex = new TRGET_ORDER_INCLUDES();
                            ex.NB = 1;
                            ex.GNB = NB;
                            ex.CARPROCEDNB = item;
                            ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED("+ item + ") FROM DUAL").FirstOrDefault();
                            db.TRGET_ORDER_INCLUDES.Add(ex);
                            db.SaveChanges();
                            INCLUDESval += ex.AMOUNT;
                        }
                    }
                   

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
    }
}