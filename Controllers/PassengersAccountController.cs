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
            List<TRPASSENGERS_CREDITSVM> TRPASSENGERS_CREDITS1 = new List<TRPASSENGERS_CREDITSVM>();

            long? totalamount = 0;
            for (var i = 1; i <= 14; i++)
            {
                var sql = "SELECT * FROM( SELECT  TP.NB, TP.CITYNB, ZC.NAME     AS CITYNAME, TP.AMOUNT, TP.CDATE FROM TRPASSENGERS_CREDITS TP JOIN ZCITYS ZC ON TP.CITYNB = ZC.NB WHERE CITYNB = " + i + " ORDER BY FIXDATE DESC) WHERE ROWNUM =1 ";

                var data = db.Database.SqlQuery<TRPASSENGERS_CREDITSVM>(sql).FirstOrDefault();

                if (data != null)
                {

                    long? alllong = long.Parse(data.AMOUNT);
                    totalamount = totalamount + alllong;
                    var all = alllong.GetValueOrDefault().ToString("###,###");
                    if (all == "" || all == null)
                    {
                        all = "0";
                    }
                    data.AMOUNT = all;
                    TRPASSENGERS_CREDITS1.Add(data);
                }

            }

            ViewBag.TotalAmount = totalamount.GetValueOrDefault().ToString("###,###");
            if (ViewBag.TotalAmount == "" || ViewBag.TotalAmount == null)
            {
                ViewBag.TotalAmount = "0";
            }
            return View(TRPASSENGERS_CREDITS1);
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
            var sql = "SELECT * FROM(SELECT * FROM TRPASSENGERS_CREDITS WHERE CITYNB = " + citynb + " ORDER BY FIXDATE DESC) WHERE ROWNUM = 1 ";
            var DDD = db.Database.SqlQuery<TRPASSENGERS_CREDITS>(sql).FirstOrDefault();
            long? alllong = DDD.AMOUNT;

            var all = alllong.GetValueOrDefault().ToString("###,###");
            if (all == "" || all == null)
            {
                all = "0";
            }
            var cityname = db.ZCITYS.Find(citynb).NAME;
            ViewBag.amount = all;
            ViewBag.cityname = cityname;
            ViewBag.citynb = citynb;
            return View();
        }
        ///////////////////////////////////////////////////////////////////
        ///-----------------------------------------------
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

            ViewData["STATUS"] = db.TRPASSENGER_ACCOUNT_STATUS.Select(x => new
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
            if (IsAdmin || IsAdminCity)
            {
                ViewBag.IsAdmin = true;
            }
            else
            {
                ViewBag.IsAdmin = false;
            }

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
            //var citynb = Request.Form["citynb"].Trim();


            //if (citynb != "")
            //{
            //    sql += " and CITYNB = " + citynb;
            //}
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


        public ActionResult Read_Proced_ex([DataSourceRequest] DataSourceRequest request, long? Nb)
        {
            var sql = " select * from TRGET_ORDER_EXCLUDES where 1 = 1 and GNB=" + Nb;





            var data = db.Database.SqlQuery<TRGET_ORDER_EXCLUDES>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                GNB = commm.GNB,
                CARPROCEDNB = commm.CARPROCEDNB,




                Seq = (request.Page - 1) * request.PageSize + (++index)
            });

            return Json(result);



        }
        public ActionResult Read_Proced_in([DataSourceRequest] DataSourceRequest request, long? Nb)
        {
            var sql = " select * from TRGET_ORDER_INCLUDES where 1 = 1 and GNB=" + Nb;





            var data = db.Database.SqlQuery<TRGET_ORDER_INCLUDES>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                GNB = commm.GNB,
                CARPROCEDNB = commm.CARPROCEDNB,




                Seq = (request.Page - 1) * request.PageSize + (++index)
            });

            return Json(result);



        }

        public ActionResult Delete_proced_EX(long nb)
        {
            try
            {

                var dd = db.TRGET_ORDER_EXCLUDES.Find(nb);



                if (dd == null)
                {
                    return Json(new { success = false, responseText = " لا يوجد سجل" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var stauts_payget = db.TRGET_ORDERS.Find(dd.GNB).STATUS;
                    if (stauts_payget == 1)
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                if (dd != null)
                                {

                                    db.TRGET_ORDER_EXCLUDES.Attach(dd);
                                    db.TRGET_ORDER_EXCLUDES.Remove(dd);
                                    db.SaveChanges();
                                    transaction.Commit();

                                }
                            }
                            catch (Exception e)
                            {
                                var SS = validation.OracleExceptionValidation(e);
                                transaction.Rollback();
                                return Json(new { success = false, responseText = SS }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    else
                    {
                        return Json(new { success = false, responseText = "لا يمكن حذف هذه المعاملة لان الكتاب مثبت مسبقاً أو ملغى" }, JsonRequestBehavior.AllowGet);
                    }

                }


                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var SS = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = SS }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Delete_proced_IN(long nb)
        {
            try
            {

                var dd = db.TRGET_ORDER_INCLUDES.Find(nb);

                if (dd == null)
                {
                    return Json(new { success = false, responseText = " لا يوجد سجل" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var stauts_payget = db.TRGET_ORDERS.Find(dd.GNB).STATUS;
                    if (stauts_payget == 1)
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                if (dd != null)
                                {

                                    db.TRGET_ORDER_INCLUDES.Attach(dd);
                                    db.TRGET_ORDER_INCLUDES.Remove(dd);
                                    db.SaveChanges();
                                    transaction.Commit();

                                }
                            }
                            catch (Exception e)
                            {
                                var SS = validation.OracleExceptionValidation(e);
                                transaction.Rollback();
                                return Json(new { success = false, responseText = SS }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    else
                    {
                        return Json(new { success = false, responseText = " لا يمكن حذف هذه المعاملة لان الكتاب مثبت مسبقاً أو ملغى" }, JsonRequestBehavior.AllowGet);
                    }
                }


                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var SS = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = SS }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult Add_proced_IN(long Gnb,long procednb)
        {
            try 
            {
                var sql = "select count(*) from carproceds where nb = " + procednb;
                var datacount = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                if (datacount == 0)
                {
                    return Json(new { success = false, responseText = "المعاملة " + procednb + " غير موجودة" }, JsonRequestBehavior.AllowGet);
                }


                var data = db.TRGET_ORDERS.Find(Gnb);

                if (data.STATUS == 1) 
                {
                    TRGET_ORDER_INCLUDES model = new TRGET_ORDER_INCLUDES();
                    model.GNB = Gnb;
                    model.CARPROCEDNB = procednb;
                    db.TRGET_ORDER_INCLUDES.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, responseText = " لا يمكن اضافة هذه المعاملة لان الكتاب مثبت مسبقاً أو ملغى" }, JsonRequestBehavior.AllowGet);

                }



            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
           
        }

        public ActionResult Add_proced_EX (long Gnb, long procednb)
        {
            try
            {
                var sql = "select count(*) from carproceds where nb = " + procednb;
                var datacount = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                if (datacount == 0)
                {
                    return Json(new { success = false, responseText = "المعاملة " + procednb + " غير موجودة" }, JsonRequestBehavior.AllowGet);
                }


                var data = db.TRGET_ORDERS.Find(Gnb);

                if (data.STATUS == 1)
                {
                    TRGET_ORDER_EXCLUDES model = new TRGET_ORDER_EXCLUDES();
                    model.GNB = Gnb;
                    model.CARPROCEDNB = procednb;
                    db.TRGET_ORDER_EXCLUDES.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, responseText = " لا يمكن اضافة هذه المعاملة لان الكتاب مثبت مسبقاً أو ملغى" }, JsonRequestBehavior.AllowGet);

                }



            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }

        }

        
        public ActionResult GetEditAllProcednb(long nb,int type )
        {
            try
            {
                if (type == 1)
                {

                    var data = db.Database.SqlQuery<TRGET_ORDER_EXCLUDES>("SELECT * FROM TRGET_ORDER_EXCLUDES WHERE GNB = " + nb).ToList();
                    return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var data = db.Database.SqlQuery<TRGET_ORDER_EXCLUDES>("SELECT * FROM TRGET_ORDER_INCLUDES WHERE GNB = " + nb).ToList();
                    return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "حدث خطأ " }, JsonRequestBehavior.AllowGet);
            }
           
          
        }
        public ActionResult GetResult(int? citynb, string FROMDATE, string TODATE, int numoftry)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ar-sa");


            DateTime FROMDATE2 = DateTime.ParseExact(FROMDATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime TODATE2 = DateTime.ParseExact(TODATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            if (numoftry == 1)
            {
                List<DateTime> alldatte = new List<DateTime>();
                for (DateTime i = FROMDATE2; i <= TODATE2;)
                {
                    alldatte.Add(i);
                    i = i.AddDays(1);
                }

                foreach (DateTime item in alldatte)
                {
                    var sql2 = "select count(*) from TRGET_ORDERS where  (TO_DATE ('" + item.ToString("dd/MM/yyyy") + "', 'DD/MM/YYYY') BETWEEN TRUNC (FROMDATE)  AND TRUNC(TODATE))";


                    var count = db.Database.SqlQuery<long?>(sql2).FirstOrDefault();
                    if (count != 0)
                    {
                        return Json(new { success = false, numoftry = 1, responseText = "يوجد كتاب تحويل تواريخه متداخلة مع التواريخ المدخلة" });
                    }
                }
            }


            string sql = "BEGIN VEHICLES.PASSENGERS_PKG.GETRESULT_PASSENGER_TAX(:PCITYNB,:FROMDATE, :TODATE,:CARBILLS_COUNT,:TOTVAL); END;";
            var PCITYNB = new OracleParameter("PCITYNB", OracleDbType.Double, citynb, ParameterDirection.Input);
            var PFROMDATE = new OracleParameter("FROMDATE", OracleDbType.Date, FROMDATE2, ParameterDirection.Input);
            var PTODATE = new OracleParameter("TODATE", OracleDbType.Date, TODATE2, ParameterDirection.Input);


            var bills = new OracleParameter("CARBILLS_COUNT", OracleDbType.Double, ParameterDirection.Output);
            var totals = new OracleParameter("TOTVAL", OracleDbType.Double, ParameterDirection.Output);
            db.Database.ExecuteSqlCommand(sql, PCITYNB, PFROMDATE, PTODATE, bills, totals);
            var x = bills.Value;
            var y = totals.Value;
            return Json(new { success = true, CARBILLS_COUNT2 = x.ToString(), TOTVA2L = y.ToString() }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CheckAllProced(string procednb)
        {
            try
            {
                long data = 0;
                var sql = "select count(*) from carproceds where nb = " + procednb;
                var datacount = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                if (datacount == 0)
                {
                    return Json(new { success = false, responseText = "المعاملة " + procednb + " غير موجودة" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    data = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + procednb + " , NULL) FROM DUAL").FirstOrDefault();
                }

                return Json(new { success = true, TOTVA2L = data, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = "حصل خطأ اثناء التحقق" }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult TRGET_ORDERS_Create(HttpPostedFileBase Files, string NO, DateTime GDATE, int? CITYNB, string FROMDATE, string TODATE, string EXCLUDES, string INCLUDES, long? PTOTALVAL, string AddNOTES)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    TRGET_ORDERS Model = new TRGET_ORDERS();
                    Model.NO = NO;
                    Model.GDATE = GDATE;
                    Model.CITYNB = CITYNB;
                    Model.NOTES = AddNOTES;
                    long? totalval = 0;
                    long? EXCLUDESval = 0;
                    long? INCLUDESval = 0;
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ar-sa");


                    DateTime FROMDATE2 = DateTime.ParseExact(FROMDATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime TODATE2 = DateTime.ParseExact(TODATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    Model.STATUS = 1;
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
                            if (item != null && item != "")
                            {
                                TRGET_ORDER_EXCLUDES ex = new TRGET_ORDER_EXCLUDES();
                                ex.NB = 1;
                                ex.GNB = NB;
                                ex.CARPROCEDNB = long.Parse(item);
                                ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + " , NULL) FROM DUAL").FirstOrDefault();
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
                            if (item != null && item != "")
                            {
                                TRGET_ORDER_INCLUDES ex = new TRGET_ORDER_INCLUDES();
                                ex.NB = 1;
                                ex.GNB = NB;
                                ex.CARPROCEDNB = long.Parse(item);
                                ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + " , NULL) FROM DUAL").FirstOrDefault();
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

                    if (CITYNB == null)
                    {
                        for (var i = 1; i <= 14; i++)
                        {
                            TRGET_ORDERS_ITEMS Model2 = new TRGET_ORDERS_ITEMS();
                            Model2.NO = NO;
                            Model2.GDATE = GDATE;
                            Model2.CITYNB = i;
                            Model2.TRGET_ORDERNB = Model.NB;
                            Model2.STATUS = 1;
                            Model2.FROMDATE = FROMDATE2;
                            Model2.TODATE = TODATE2;
                            db.TRGET_ORDERS_ITEMS.Add(Model2);
                            db.SaveChanges();
                        }


                    }
                    else
                    {
                        TRGET_ORDERS_ITEMS Model2 = new TRGET_ORDERS_ITEMS();
                        Model2.NO = NO;
                        Model2.GDATE = GDATE;
                        Model2.CITYNB = CITYNB;
                        Model2.TRGET_ORDERNB = Model.NB;
                        Model2.STATUS = 1;
                        Model2.FROMDATE = FROMDATE2;
                        Model2.TODATE = TODATE2;
                        db.TRGET_ORDERS_ITEMS.Add(Model2);
                        db.SaveChanges();
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



        public ActionResult TRGET_ORDERS_Update(HttpPostedFileBase Files, long oldNB,string NO, DateTime GDATE, int? CITYNB, string FROMDATE, string TODATE, string EXCLUDES, string INCLUDES, long? PTOTALVAL, string AddNOTES)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                   
                   var Model = db.TRGET_ORDERS.Find(oldNB);

                  
                    if (Model.NO != NO)
                    {
                        Model.NO = NO;
                    }
                  



                    if (Model.GDATE != GDATE)
                    {
                        Model.GDATE = GDATE;
                    }
                   

                    if (Model.NOTES != AddNOTES)
                    {
                        Model.NOTES = AddNOTES;
                    }
                  


                    Model.CITYNB = CITYNB;
                

                    long? totalval = 0;
                    long? EXCLUDESval = 0;
                    long? INCLUDESval = 0;
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ar-sa");


                    DateTime FROMDATE2 = DateTime.ParseExact(FROMDATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime TODATE2 = DateTime.ParseExact(TODATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);


                    if (Model.FROMDATE != FROMDATE2)
                    {
                        Model.FROMDATE = FROMDATE2;
                    }
                   


                    if (Model.TODATE != TODATE2)
                    {
                        Model.TODATE = TODATE2;
                    }
                   

                    Model.STATUS = Model.STATUS;

                 

                 
                   
                    db.Database.ExecuteSqlCommand(" DELETE FROM TRGET_ORDER_EXCLUDES WHERE GNB = " + oldNB );
                    db.Database.ExecuteSqlCommand(" DELETE FROM TRGET_ORDER_INCLUDES WHERE GNB =  " + oldNB );
                    db.Database.ExecuteSqlCommand(" DELETE FROM TRGET_ORDERS_ITEMS WHERE TRGET_ORDERNB =  " + oldNB);
                  //  db.Database.ExecuteSqlCommand(" DELETE FROM TRGET_ORDERS WHERE NB =  " + oldNB );
                    db.SaveChanges();

                    db.Entry(Model).State = EntityState.Modified;
                  //  db.TRGET_ORDERS.Add(Model);

                    db.SaveChanges();
                    var NB = Model.NB;
                    if (EXCLUDES != null)
                    {
                        var xx = EXCLUDES.Split(',');
                        foreach (var item in xx)
                        {
                            if (item != null && item != "")
                            {
                                TRGET_ORDER_EXCLUDES ex = new TRGET_ORDER_EXCLUDES();
                                ex.NB = 1;
                                ex.GNB = NB;
                                ex.CARPROCEDNB = long.Parse(item);
                                ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + " , NULL) FROM DUAL").FirstOrDefault();
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
                            if (item != null && item != "")
                            {
                                TRGET_ORDER_INCLUDES ex = new TRGET_ORDER_INCLUDES();
                                ex.NB = 1;
                                ex.GNB = NB;
                                ex.CARPROCEDNB = long.Parse(item);
                                ex.AMOUNT = db.Database.SqlQuery<long>("SELECT VEHICLES.PASSENGERS_PKG.GET_VAL_TAX_PROCED(" + item + " , NULL) FROM DUAL").FirstOrDefault();
                                db.TRGET_ORDER_INCLUDES.Add(ex);
                                db.SaveChanges();
                                INCLUDESval += ex.AMOUNT;
                            }
                        }
                    }

                    if (Files != null)
                    {
                       
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
                    }
                   
                    totalval = PTOTALVAL + INCLUDESval - EXCLUDESval;
                    if (totalval >= 0)
                    {
                        Model.AMOUNT = (long)totalval;
                    }
                    else
                    {
                        Model.AMOUNT = 0;
                    }
                   
                    db.SaveChanges();

                    if (CITYNB == null)
                    {
                        for (var i = 1; i <= 14; i++)
                        {
                            TRGET_ORDERS_ITEMS Model2 = new TRGET_ORDERS_ITEMS();
                            Model2.NO = NO;
                            Model2.GDATE = GDATE;
                            Model2.CITYNB = i;
                            Model2.TRGET_ORDERNB = Model.NB;
                            Model2.STATUS = 1;
                            Model2.FROMDATE = FROMDATE2;
                            Model2.TODATE = TODATE2;
                            db.TRGET_ORDERS_ITEMS.Add(Model2);
                            db.SaveChanges();
                        }


                    }
                    else
                    {
                        TRGET_ORDERS_ITEMS Model2 = new TRGET_ORDERS_ITEMS();
                        Model2.NO = NO;
                        Model2.GDATE = GDATE;
                        Model2.CITYNB = CITYNB;
                        Model2.TRGET_ORDERNB = Model.NB;
                        Model2.STATUS = 1;
                        Model2.FROMDATE = FROMDATE2;
                        Model2.TODATE = TODATE2;
                        db.TRGET_ORDERS_ITEMS.Add(Model2);
                        db.SaveChanges();
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


        public ActionResult Read_TRGET_ORDERS_ITEMS([DataSourceRequest] DataSourceRequest request, long? Nb)
        {
            var sql = " select * from TRGET_ORDERS_ITEMS where 1 = 1 and TRGET_ORDERNB = " + Nb;



            CodesController cc = new CodesController();
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
            if (!IsAdmin && !IsAdminCity)
            {
                var mycitynb = Utility.MyCityNb();
                sql += " and CITYNB = " + mycitynb;
            }


            var data = db.Database.SqlQuery<TRGET_ORDERS_ITEMS>(sql).ToList();

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




                Seq = (request.Page - 1) * request.PageSize + (++index)
            });

            return Json(result);



        }


        public ActionResult SAVECONFIRM_GET(long NB)
        {
            try
            {


                var data = db.TRGET_ORDERS.Find(NB);

                List<DateTime> alldatte = new List<DateTime>();
                for (DateTime i = data.FROMDATE; i <= data.TODATE;)
                {
                    alldatte.Add(i);
                    i = i.AddDays(1);
                }

                foreach (DateTime item in alldatte)
                {
                    var sql2 = "select count(*) from TRGET_ORDERS where nb != " + NB + "and  (TO_DATE ('" + item.ToString("dd/MM/yyyy") + "', 'DD/MM/YYYY') BETWEEN TRUNC (FROMDATE)  AND TRUNC(TODATE))";


                    var count = db.Database.SqlQuery<long?>(sql2).FirstOrDefault();
                    if (count != 0)
                    {
                        return Json(new { success = false, responseText = "يوجد كتاب تحويل تواريخه متداخلة مع التواريخ المدخلة" });
                    }
                }



                if (data == null)
                {
                    return Json(new { success = false, responseText = "لا يوجد سجل" });
                }
                else
                {
                    if (data.STATUS == 2)
                    {
                        return Json(new { success = false, responseText = "امر الصرف مثبت سابقاً" });
                    }
                    if (data.STATUS == 3)
                    {
                        return Json(new { success = false, responseText = "امر الصرف ملغى" });
                    }
                    if (data.STATUS == 1)
                    {
                        string sql = "BEGIN VEHICLES.PASSENGERS_PKG.CONFIRM_GET_ORDER(:PNB,:PSTATUS); END;";
                        var PNB = new OracleParameter("PNB", OracleDbType.Double, NB, ParameterDirection.Input);



                        var STATUS = new OracleParameter("PSTATUS", OracleDbType.Double, ParameterDirection.Output);

                        db.Database.ExecuteSqlCommand(sql, PNB, STATUS);
                        var x = STATUS.Value;
                        if (x.ToString() == "0")
                        {
                            return Json(new { success = false, responseText = "حصل خطأ اثناء التنفيذ" });
                        }


                    }
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "حصل خطأ اثناء التنفيذ" });
            }

        }

        public ActionResult DeleteDocumentGET(long paynb)
        {
            try
            {
                var data = db.TRGET_ORDERS.Find(paynb);
                if (data != null)
                {
                    if (data.IS_ARCHIVED == false)
                    {
                        return Json(new { success = false, responseText = "لا يوجد ارشفة  " }, JsonRequestBehavior.AllowGet);

                    }
                    var path = data.FTP_PATH;

                    var is_true = CodesController.DeleteFTPFile(path);
                    if (is_true)
                    {
                        data.IS_ARCHIVED = false;
                        data.FTP_PATH = "";
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { success = false, responseText = "حدث خطأ " }, JsonRequestBehavior.AllowGet);

                    }



                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = "حدث خطأ " }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = true, responseText = "تم الحذف" }, JsonRequestBehavior.AllowGet);

        }
        /////////////////////////////////////////////////////////////////    
        ///----------------------------------------------------
        public ActionResult TRPAY_ORDERS_Index(int citynb)
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

            ViewData["STATUS"] = db.TRPASSENGER_ACCOUNT_STATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["TRZEXPENSE_TYPES"] = db.TRZEXPENSE_TYPES.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["TRZPAY_OWNER_TYPES"] = db.TRZPAY_OWNER_TYPES.Select(x => new
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
        public ActionResult TRPAY_ORDERS_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = " select * from TRPAY_ORDERS where 1 = 1 ";

            var NO = Request.Form["NO"].Trim();

            var SPDATE = Request.Form["SPDATE"].Trim();
            var SOWNERTYPENB = Request.Form["SOWNERTYPENB"].Trim();
            var SEXPENSETYPENB = Request.Form["SEXPENSETYPENB"].Trim();
            var SAmount = Request.Form["SAmount"].Trim();

            var Status = Request.Form["Status"].Trim();
            var StrSessArc = Request.Form["StrSessArc"].Trim();
            var citynb = Request.Form["citynb"].Trim();

            //var CAUSES = Request.Form["CAUSES"].Trim();







            if (citynb != "")
            {
                sql += " and CITYNB = " + citynb;
            }
            if (NO != "")
            {
                sql += " and NO like '" + NO + "'";
            }

            if (SPDATE != "")
            {
                sql += " and TRUNC(PDATE) = TO_DATE('" + SPDATE + "','DD/MM/YYYY') ";
            }



            if (StrSessArc != "")
            {
                sql += " and IS_ARCHIVED =" + StrSessArc;
            }

            if (Status != "")
            {
                sql += " and STATUS =" + Status;
            }

            if (SOWNERTYPENB != "")
            {
                sql += " and PAY_OWNER_TYPENB =" + SOWNERTYPENB;
            }

            if (SEXPENSETYPENB != "")
            {
                sql += " and EXPENSE_TYPENB =" + SEXPENSETYPENB;
            }

            if (SAmount != "")
            {
                sql += " and AMOUNT =" + SAmount;
            }

            sql += " order by nb desc";


            var data = db.Database.SqlQuery<TRPAY_ORDERS>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                CITYNB = commm.CITYNB,
                NO = commm.NO,
                PDATE = commm.PDATE,
                AMOUNT = commm.AMOUNT,
                CAUSES = commm.CAUSES,
                STATUS = commm.STATUS,
                NOTES = commm.NOTES,
                EXPENSE_TYPENB = commm.EXPENSE_TYPENB,
                PAY_OWNER_TYPENB = commm.PAY_OWNER_TYPENB,
                IS_ARCHIVED = commm.IS_ARCHIVED,
                FTP_PATH = commm.FTP_PATH,



                Seq = (request.Page - 1) * request.PageSize + (++index)
            });

            return Json(result);



        }

        public ActionResult TRPAY_ORDERS_Create(HttpPostedFileBase Files, string AddPAYNo, DateTime AddPDATEDate, int CITYNB, string AddNOTES, string AddCAUSES, int OWNERTYPENB, int EXPENSETYPENB, long Amount)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var OwnerAmount = db.TRZPAY_OWNER_TYPES.Find(OWNERTYPENB).MAXAMOUNT;
                    if (OwnerAmount != null)
                    {
                        if (OwnerAmount < Amount)
                        {
                            return Json(new { success = false, responseText = "قيمة امر الصرف تجاوز الحد الاعلى المسموح به" });
                        }
                    }



                    TRPAY_ORDERS Model = new TRPAY_ORDERS();
                    Model.NO = AddPAYNo;
                    Model.PDATE = AddPDATEDate;
                    Model.CITYNB = CITYNB;
                    Model.STATUS = 1;

                    Model.NOTES = AddNOTES;
                    Model.CAUSES = AddCAUSES;
                    Model.EXPENSE_TYPENB = EXPENSETYPENB;
                    Model.PAY_OWNER_TYPENB = OWNERTYPENB;
                    Model.AMOUNT = Amount;
                    db.TRPAY_ORDERS.Add(Model);

                    db.SaveChanges();
                    var NB = Model.NB;



                    byte[] fileContent = null;
                    using (var reader = new System.IO.BinaryReader(Files.InputStream))
                    {
                        fileContent = reader.ReadBytes(Files.ContentLength);
                    }

                    var date = DateTime.Now;
                    var pathNameYear = date.ToString("yyyy");

                    pathNameYear += "/" + Model.CITYNB + "/";

                    var FTPFullPath = ConfigurationManager.AppSettings["FtpHomeTRPAYORDERS"];
                    FTPFullPath += pathNameYear;

                    var uploadedFullPath = CodesController.UploadFile(fileContent, FTPFullPath, Files.FileName, FTPFullPath, NB);

                    Model.FTP_PATH = uploadedFullPath;
                    Model.IS_ARCHIVED = true;


                    db.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true, responseText = "تمت الاضافة بنجاح" }, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    var ss = validation.OracleExceptionValidation(ex);
                    return Json(new { success = false, responseText = ss });
                }
            }

        }


        public ActionResult TRPAY_ORDERS_Update(HttpPostedFileBase Files, long PAYNb, string AddPAYNo, DateTime AddPDATEDate, int CITYNB, string AddNOTES, string AddCAUSES, int OWNERTYPENB, int EXPENSETYPENB, long Amount)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var data = db.TRPAY_ORDERS.Find(PAYNb);
                    if (data == null)
                    {
                        return Json(new { success = false, responseText = "السجل غير موجود" });
                    }

                    if (data.AMOUNT != Amount)
                    {
                        var OwnerAmount = db.TRZPAY_OWNER_TYPES.Find(OWNERTYPENB).MAXAMOUNT;
                        if (OwnerAmount != null)
                        {
                            if (OwnerAmount < Amount)
                            {
                                return Json(new { success = false, responseText = "قيمة امر الصرف تجاوز الحد الاعلى المسموح به" });
                            }
                            else
                            {
                                data.AMOUNT = Amount;
                            }
                        }


                    }


                    if (data.NO != AddPAYNo)
                    {
                        data.NO = AddPAYNo;
                    }

                    if (data.PDATE != AddPDATEDate)
                    {
                        data.PDATE = AddPDATEDate;
                    }
                    if (data.NOTES != AddNOTES)
                    {
                        data.NOTES = AddNOTES;
                    }
                    if (data.CAUSES != AddCAUSES)
                    {
                        data.CAUSES = AddCAUSES;
                    }
                    if (data.PAY_OWNER_TYPENB != OWNERTYPENB)
                    {
                        data.PAY_OWNER_TYPENB = OWNERTYPENB;
                    }
                    if (data.EXPENSE_TYPENB != EXPENSETYPENB)
                    {
                        data.EXPENSE_TYPENB = EXPENSETYPENB;
                    }




                    db.SaveChanges();
                    var NB = data.NB;



                    byte[] fileContent = null;
                    using (var reader = new System.IO.BinaryReader(Files.InputStream))
                    {
                        fileContent = reader.ReadBytes(Files.ContentLength);
                    }

                    var date = DateTime.Now;
                    var pathNameYear = date.ToString("yyyy");

                    pathNameYear += "/" + data.CITYNB + "/";

                    var FTPFullPath = ConfigurationManager.AppSettings["FtpHomeTRPAYORDERS"];
                    FTPFullPath += pathNameYear;

                    var uploadedFullPath = CodesController.UploadFile(fileContent, FTPFullPath, Files.FileName, FTPFullPath, NB);

                    data.FTP_PATH = uploadedFullPath;
                    data.IS_ARCHIVED = true;

                    db.Entry(data).State = EntityState.Modified;

                    db.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true, responseText = "تمت التعديل بنجاح" }, JsonRequestBehavior.AllowGet);

                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    var ss = validation.OracleExceptionValidation(ex);
                    return Json(new { success = false, responseText = ss });
                }
            }

        }





        public ActionResult GetReportPAY(long NB)
        {
            try
            {
                CodesController cc = new CodesController();
                var ses = db.TRPAY_ORDERS.Find(NB);
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


        public ActionResult SAVECONFIRMPAY(long NB)
        {
            try
            {
                var data = db.TRPAY_ORDERS.Find(NB);
                if (data == null)
                {
                    return Json(new { success = false, responseText = "لا يوجد سجل" });
                }
                else
                {
                    if (data.STATUS == 2)
                    {
                        return Json(new { success = false, responseText = "امر الصرف مثبت سابقاً" });
                    }
                    if (data.STATUS == 3)
                    {
                        return Json(new { success = false, responseText = "امر الصرف ملغى" });
                    }
                    if (data.STATUS == 1)
                    {
                        string sql = "BEGIN VEHICLES.PASSENGERS_PKG.CONFIRM_PAY_ORDER(:PNB,:PSTATUS); END;";
                        var PNB = new OracleParameter("PNB", OracleDbType.Double, NB, ParameterDirection.Input);



                        var STATUS = new OracleParameter("PSTATUS", OracleDbType.Double, ParameterDirection.Output);

                        db.Database.ExecuteSqlCommand(sql, PNB, STATUS);
                        var x = STATUS.Value;
                        if (x.ToString() == "0")
                        {
                            return Json(new { success = false, responseText = "حصل خطأ اثناء التنفيذ" });
                        }


                    }
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "حصل خطأ اثناء التنفيث" });
            }

        }


        public ActionResult DeleteDocument(long paynb)
        {
            try
            {
                var data = db.TRPAY_ORDERS.Find(paynb);
                if (data != null)
                {
                    if (data.IS_ARCHIVED == false)
                    {
                        return Json(new { success = false, responseText = "لا يوجد ارشفة  " }, JsonRequestBehavior.AllowGet);

                    }
                    var path = data.FTP_PATH;

                    var is_true = CodesController.DeleteFTPFile(path);
                    if (is_true)
                    {
                        data.IS_ARCHIVED = false;
                        data.FTP_PATH = "";
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { success = false, responseText = "حدث خطأ " }, JsonRequestBehavior.AllowGet);

                    }



                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = "حدث خطأ " }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = true, responseText = "تم الحذف" }, JsonRequestBehavior.AllowGet);

        }
    }
}