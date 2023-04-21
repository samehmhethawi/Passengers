using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using Passengers.ViewModel;
using Proced.DataAccess.Models.CF;
using Rotativa;
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
                var ddd = db.TRPASSENGERS_CREDITS.Find(mycitynb);
                long? amount = ddd.AMOUNT;
                var cityname = db.ZCITYS.Find(mycitynb).NAME;
                var all = amount.GetValueOrDefault().ToString("###,###");
                if (all == "" || all == null)
                {
                    all = "0";
                }
                ViewBag.amount = all;
                ViewBag.amount222 = amount;
                ViewBag.cityname = cityname;
                ViewBag.citynb = mycitynb;
                ViewBag.CDATE = ddd.CDATE.ToString("dd/MM/yyyy"); 
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
   
        public ActionResult UpdateAmount(int citynb , long Amount , DateTime SSdate)
        {
            try
            {
                var data = db.TRPASSENGERS_CREDITS.Find(citynb);
                data.AMOUNT = Amount;
                data.CDATE = SSdate;
                data.FIXDATE = DateTime.Now;

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
    
    
    public ActionResult TRPASSENGERS_CREDITS_Index_Detalis(int citynb)
        {
            CodesController cc = new CodesController();
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
            if (!IsAdmin && !IsAdminCity)
            {
                var mycitynb = Utility.MyCityNb();
                if (mycitynb != citynb)
                    return RedirectToAction("Index_Detalis", "PassengersAccount", new { citynb = mycitynb });
            }
            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            var cityname = db.ZCITYS.Find(citynb).NAME;
            ViewBag.citynb = citynb;
            ViewBag.cityname = cityname;
            return View();

        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {

            var SFromCDATE = Request.Form["SFromCDATE"].Trim();
            var SToCDATE = Request.Form["SToCDATE"].Trim();
            var SCTYPE = Request.Form["SCTYPE"].Trim();

            var mycitynb = Request.Form["citynb"].Trim();
            var sql = "SELECT * FROM TRPASSENGERS_CREDITS WHERE 1 = 1 and CITYNB = "+ mycitynb;

            if (SCTYPE != "")
            {
                sql += " and CTYPE =" + SCTYPE;
            }

            if (SFromCDATE != "")
            {
                sql += " and TRUNC(CDATE) >= TO_DATE('" + SFromCDATE + "','DD/MM/YYYY') ";
            }

            if (SToCDATE != "")
            {
                sql += " and TRUNC(CDATE) <= TO_DATE('" + SToCDATE + "','DD/MM/YYYY') ";
            }
            sql += " order by  CDATE asc , nb asc";
            var data = db.Database.SqlQuery<TRPASSENGERS_CREDITS>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                CITYNB = commm.CITYNB,
                CDATE = commm.CDATE,
                AMOUNT = commm.AMOUNT,
                NOTES = commm.NOTES,
                FIXDATE = commm.FIXDATE,
                CTYPE = commm.CTYPE,
                ITEM_AMOUNT = commm.ITEM_AMOUNT,





                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
        }



        public ActionResult TRPASSENGERS_CREDITS_PDF(int? SCTYPE, string SFromCDATE, string SToCDATE ,int citynb)
        {
            CodesController cc = new CodesController();
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
            if (!IsAdmin && !IsAdminCity)
            {
                var xmycitynb = Utility.MyCityNb();
                if (xmycitynb != citynb)
                {
                    citynb = xmycitynb;
                }
                    
            }
            var pFromCDATE = SFromCDATE;
            var pToCDATE = SToCDATE;
            var pCTYPE = SCTYPE;

            var mycitynb = citynb;
            var sql = "SELECT TR.NB , ZC.NAME  AS CITYNB, TRUNC(TR.CDATE) as CDATEss, TR.AMOUNT, TR.NOTES,  TR.FIXDATE , CASE  WHEN TR.CTYPE = 1 THEN 'تحويل' WHEN TR.CTYPE = 2 THEN 'صرف' END as CTYPE, TR.ITEM_AMOUNT  FROM TRPASSENGERS_CREDITS TR JOIN ZCITYS ZC ON TR.CITYNB = ZC.NB WHERE 1 = 1 and TR.CITYNB = " + mycitynb;

            if (pCTYPE.HasValue)
            {
                sql += " and TR.CTYPE =" + SCTYPE;
            }

            if (pFromCDATE != "")
            {
                sql += " and TRUNC(TR.CDATE) >= TO_DATE('" + pFromCDATE + "','DD/MM/YYYY') ";
            }

            if (pToCDATE != "")
            {
                sql += " and TRUNC(TR.CDATE) <= TO_DATE('" + pToCDATE + "','DD/MM/YYYY') ";
            }
            sql += " order by  TR.CDATE asc , TR.nb asc";
            var data = db.Database.SqlQuery<TRPASSENGERS_CREDITSVMPDF>(sql).ToList();
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };

        }
        //public ActionResult TRPASSENGERS_CREDITS_ex()
        //{


        //    string query = "SELECT ZC.NAME AS CityName,"
        //           + " TM.NB AS CommNb, "
        //           + " TM.COMNO AS CommNo, "
        //    + " TO_DATE(TM.COMDATE,'DD/MM/YYYY') AS CommDate, "
        //    + " TS.NAME AS CommStatus, "
        //    + " TCS.NB AS SessNb, "
        //    + " TCS.SESNO AS SessNo, "
        //    + " TO_DATE(TCS.SESDATE,'DD/MM/YYYY') AS SessDate, "
        //    + " TS2.NAME AS SessStatus, "
        //    + " TMM.MEMBERNAME AS BossName, "
        //    + " ZP.NAME AS BossPostion "
        //    + " FROM TRCOMMITTEES TM "
        //    + " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB "
        //    + " JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB "
        //    + " JOIN TRSTATUS TS ON TS.NB = TM.STATUS "
        //    + " JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB "
        //    + " JOIN TRSESSIONS TCS ON TCS.COMMITTEENB = TM.NB "
        //    + " JOIN TRSTATUS TS2 ON TS2.NB = TCS.STATUS "
        //    + " WHERE 1 = 1 AND TMM.MEMBERSHIPNB = 1 ";

        //    var data = db.Database.SqlQuery<ViewModel.CommittesAndSessions>(query);

        //    var data2 = from e in data
        //                select new
        //                {
        //                    المحافظة = e.CityName,
        //                    رمز_اللجنة = e.CommNb,
        //                    رقم_اللجنة = e.CommNo,
        //                    تاريخ_اللجنة = e.CommDate,
        //                    حالة_اللجنة = e.CommStatus,
        //                    رمز_الجلسة = e.SessNb,
        //                    رقم_الجلسة = e.SessNo,
        //                    تاريخ_الجلسة = e.SessDate,
        //                    حالة_الجلسة = e.SessStatus,
        //                    اسم_رئيس_الجلسة = e.BossName,
        //                    منصب_رئيس_الجلسة = e.BossPostion,
        //                };

        //    string sheetName = "تقرير اللجان وجلساتها";
        //    XLWorkbook wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(sheetName);
        //    ws.Cell(1, 1).InsertTable(data2);

        //    ws.RightToLeft = true;
        //    ws.ColumnWidth = 30;


        //    //ws.Style.Font.Bold = true;
        //    ws.Style.Font.FontSize = 12;
        //    for (int i = 1; i <= 11; i++)
        //    {
        //        ws.Column(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //    }

        //    Response.Clear();
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", sheetName.Replace(" ", "_")));

        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        wb.SaveAs(memoryStream);
        //        memoryStream.WriteTo(Response.OutputStream);
        //        memoryStream.Close();
        //    }

        //    Response.End();

        //    return View();
        //}

    }
}