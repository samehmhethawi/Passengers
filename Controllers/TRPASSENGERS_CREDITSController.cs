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
    public class TRPASSENGERS_CREDITSController : Controller
    {
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        // GET: TRPASSENGERS_CREDITS
        public ActionResult Index()
        {
            var mycitynb = Utility.MyCityNb();
            var rwo_cont = db.Database.SqlQuery<long>("SELECT COUNT(*) FROM TRPASSENGERS_CREDITS WHERE CITYNB ="+ mycitynb).FirstOrDefault();

            if (rwo_cont == 1 )
            {
                long? amount = db.TRPASSENGERS_CREDITS.Find(mycitynb).AMOUNT;
                var cityname = db.ZCITYS.Find(mycitynb).NAME;
                var all = amount.GetValueOrDefault().ToString("###,###");
                if (all == "" || all == null)
                {
                    all = "0";
                }
                ViewBag.amount = all;
                ViewBag.cityname = cityname;
                ViewBag.citynb = mycitynb;

                return View();
            }
            else 
            {
                return RedirectToAction("Index_Detalis");
            }

           
        }

        public ActionResult Index_Detalis()
        {
            var mycitynb = Utility.MyCityNb();
            var sql = "SELECT * FROM(SELECT * FROM TRPASSENGERS_CREDITS WHERE CITYNB = " + mycitynb + " ORDER BY FIXDATE DESC) WHERE ROWNUM = 1 ";
            var amount = db.Database.SqlQuery<TRPASSENGERS_CREDITS>(sql).FirstOrDefault();
            long? alllong = amount.AMOUNT;
            // amount = db.TRPASSENGERS_CREDITS.Find(mycitynb).AMOUNT;
            var cityname = db.ZCITYS.Find(mycitynb).NAME;

            var all = alllong.GetValueOrDefault().ToString("###,###");
            if (all == "" || all == null)
            {
                all = "0";
            }
            ViewBag.amount = all;
            ViewBag.cityname = cityname;
            ViewBag.citynb = mycitynb;

            return View();
        }
   
        public ActionResult UpdateAmount(int citynb , long Amount)
        {
            try
            {
                var data = db.TRPASSENGERS_CREDITS.Find(citynb);
                data.AMOUNT = Amount;


                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    // return View("TRSESSIONSPROCEDS?id=181");
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
    
    
        public ActionResult GetAmountUnConfierm(int citynb)
        {

            try
            {
                long amountExclud = 0;
                var da = DateTime.Now;
                var sql2 = "SELECT *  FROM (  SELECT * FROM TRPASSENGERS_CREDITS  WHERE CITYNB = "+ citynb + " and CTYPE = 1 ORDER BY CDATE DESC) WHERE ROWNUM = 1";
                var data = db.Database.SqlQuery<TRPASSENGERS_CREDITS>(sql2).FirstOrDefault();
               
                



                string sql = "BEGIN VEHICLES.PASSENGERS_PKG.GETRESULT_PASSENGER_TAX(:PCITYNB,:FROMDATE, :TODATE,:CARBILLS_COUNT,:TOTVAL); END;";
                var PCITYNB = new OracleParameter("PCITYNB", OracleDbType.Double, citynb, ParameterDirection.Input);
                var PFROMDATE = new OracleParameter("FROMDATE", OracleDbType.Date, data.CDATE, ParameterDirection.Input);
                var PTODATE = new OracleParameter("TODATE", OracleDbType.Date, da, ParameterDirection.Input);


                var bills = new OracleParameter("CARBILLS_COUNT", OracleDbType.Double, ParameterDirection.Output);
                var totals = new OracleParameter("TOTVAL", OracleDbType.Double, ParameterDirection.Output);
                db.Database.ExecuteSqlCommand(sql, PCITYNB, PFROMDATE, PTODATE, bills, totals);
                var x = bills.Value;
                var y = totals.Value;

                var ff = y.ToString();
                long? fff = long.Parse(ff);
                List<long> listpro = db.Database.SqlQuery<long>("SELECT CARPROCEDNB FROM TRGET_ORDER_EXCLUDES WHERE CARPROCEDNB NOT IN (SELECT CARPROCEDNB FROM TRGET_ORDER_INCLUDES)").ToList();
                foreach(var item in listpro)
                {
                
                    var val = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + " , "+ citynb + ") FROM DUAL").FirstOrDefault();
                    amountExclud += val;
                }



              var  totval = fff + amountExclud;
                var all = totval.GetValueOrDefault().ToString("###,###");
                if (all == "" || all == null)
                {
                    all = "0";
                }
                // return View("TRSESSIONSPROCEDS?id=181");
                return Json(new { success = true, TOTVA2L = all.ToString(), amountExclud = amountExclud, responseText = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}