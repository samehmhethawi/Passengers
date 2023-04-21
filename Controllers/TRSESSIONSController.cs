using System;
using Proced.DataAccess.Models.CF;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Passengers.ViewModel;
using System.Data.Entity;
using ClosedXML.Excel;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Text;


namespace Passengers.Controllers
{

    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class TRSESSIONSController : Controller
    {
        
        public string DateFormat = "yyyy" ;
       // private static string FTPFileName = "";
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        // GET: TRSESSIONS
        public ActionResult Index()
        {
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

            ViewData["TRZMEMBERSHIP"] = db.TRZMEMBERSHIP.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["TRZMEMBERPOSITION"] = db.TRZMEMBERPOSITION.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = " select * from TRSESSIONS where 1 = 1 ";

            var SESNO = Request.Form["SESNO"].Trim();
            var SESDATESTART = Request.Form["SESDATESTART"].Trim();
            var SESDATEEND = Request.Form["SESDATEEND"].Trim();
            var STATUS = Request.Form["STATUS"].Trim();
            var SESCITYNB = Request.Form["SESCITYNB"].Trim();
            var StrSessArc = Request.Form["StrSessArc"].Trim();

            

            if (SESNO != "")
            {
                sql += " and SESNO like '%" + SESNO + "%'";
            }
            if (STATUS != "")
            {
                sql += " and STATUS =" + STATUS;
            }

            if (SESDATESTART != "")
            {
                sql += " and TRUNC(SESDATE) >= TO_DATE('" + SESDATESTART + "','DD/MM/YYYY') ";
            }

            if (SESDATEEND != "")
            {
                sql += " and TRUNC(SESDATE) <= TO_DATE('" + SESDATEEND + "','DD/MM/YYYY') ";
            }
            if (StrSessArc != "")
            {
                sql += " and IS_ARCHIVED =" + StrSessArc;
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SESCITYNB != "")
                {
                    sql += " and SESCITYNB =" + SESCITYNB;
                }

            }
            else
            {
                sql += " and SESCITYNB =" + ci;
            }
          //  var total = db.Database.SqlQuery<int>(" select /*+ NO_CPU_COSTING */  count(nb)  FROM ("+sql+")").SingleOrDefault();

            sql += " order by nb desc";
                //OFFSET " + request.PageSize * (request.Page - 1) + " ROWS FETCH NEXT " + request.PageSize + " ROWS ONLY ";

            
           //var sql_ = "select ROWNUM +" + (request.Page - 1) + "*" + request.PageSize + " seq, aa.*from(" + sql + ")aa";

            var data = db.Database.SqlQuery<TRSESSIONS>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                SESNO = commm.SESNO,
                SESDATE = commm.SESDATE,
                COMMITTEENB = commm.COMMITTEENB,
                COMBOSSNAME = commm.COMBOSSNAME,
                SESCITYNB = commm.SESCITYNB,
                STATUS = commm.STATUS,
                NOTES = commm.NOTES,
                ORDR = commm.ORDR,
                IUSER = commm.IUSER,
                IDATE = commm.IDATE,
                IS_ARCHIVED = commm.IS_ARCHIVED,
                FTP_PATH = commm.FTP_PATH,



                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
          //  result.Total = total;

            return Json(result);
            
           
           


        }
        public ActionResult Create(TRSESSIONS model)
        {
            try
            {

                var count_com_session = db.Database.SqlQuery<int>("select count(nb) from TRCOMMITTEES where COMCITYNB = " + model.SESCITYNB + " and STATUS = 1").FirstOrDefault();
                if (count_com_session == 0)
                {
                    return Json(new { success = false, responseText = "لا يوجد لجنة فعالة لهذه المحافظة " });
                }
                else if (count_com_session > 1)
                {
                    return Json(new { success = false, responseText = " يوجد اكثر من  لجنة فعالة واحدة لهذه المحافظة " });
                }
                else
                {
                    var is_session_exists = db.Database.SqlQuery<int>("select 1 from TRSESSIONS where SESCITYNB = " + model.SESCITYNB + " and STATUS in (1,2)").FirstOrDefault();

                    if (is_session_exists == 1)
                    {
                        return Json(new { success = false, responseText = "يوجد جلسة فعالة لهذه المحافظة " });
                    }
                    else
                    {
                        var comnb = db.Database.SqlQuery<int>("select nb from TRCOMMITTEES where COMCITYNB = " + model.SESCITYNB + " and STATUS = 1").FirstOrDefault();
                        var mem = db.TRCOMMITTEES_MEMBERS.Where(x => x.COMMITTEENB == comnb && x.MEMBERSHIPNB == 1).Select(s => s.MEMBERNAME).FirstOrDefault();

                        if (mem == null)

                        {
                            return Json(new { success = false, responseText = "يجب اضافة رئيس لجنة على الاقل" }, JsonRequestBehavior.AllowGet);
                        }
                        model.COMMITTEENB = comnb;
                        model.COMBOSSNAME = mem;
                        model.IUSER = Utility.MyName();
                        model.IDATE = DateTime.Now;
                        model.IS_ARCHIVED = false;
                        model.FINISH_PRINT = 0;
                        db.TRSESSIONS.Add(model);
                        db.SaveChanges();
                        return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                }




            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss });
            }

        }
        public ActionResult EditSession(TRSESSIONS model)
        {
            try
            {
                var data = db.TRSESSIONS.Find(model.NB);
                if (data != null)
                {
                    if (data.SESNO != model.SESNO)
                    {
                        data.SESNO = model.SESNO;
                    }
                    if (data.SESDATE != model.SESDATE)
                    {
                        data.SESDATE = model.SESDATE;
                    }
                    db.Entry(data).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss });
            }
        }
        public ActionResult Read_Member_Session([DataSourceRequest] DataSourceRequest request, long NB)
        {
            var sql = "SELECT TSM.NB ,TSM.SESSIONNB , TSM.MEMBERNB , TSM.ISPRESENT ,TM.MEMBERNAME ,TSM.MEMBERSHIPNB , TM.MEMBERPOSITIONNB ,TS.STATUS AS SESSIONSTATUS FROM TRSESSIONS_MEMBERS_PRESENT  TSM JOIN TRCOMMITTEES_MEMBERS TM ON TM.NB = TSM.MEMBERNB JOIN TRSESSIONS TS ON TS.NB = TSM.SESSIONNB  WHERE TSM.SESSIONNB  =  " + NB;

            var data = db.Database.SqlQuery<TRSESSIONS_MEMBERS_PRESENTVM>(sql);



            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditMEMBERSHIP(long nb, long MEMBERSHIPNB)
        {
            try
            {
                var data = db.TRSESSIONS_MEMBERS_PRESENT.Find(nb);
                data.MEMBERSHIPNB = MEMBERSHIPNB;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult NotPRESENT(long nb)
        {
            var data = db.TRSESSIONS_MEMBERS_PRESENT.Find(nb);
            data.ISPRESENT = 0;
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult YesPRESENT(long nb)
        {
            var data = db.TRSESSIONS_MEMBERS_PRESENT.Find(nb);
            data.ISPRESENT = 1;
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TRSESSIONSPROCEDS(long id)
        {
            ViewData["TRSESSIONSPROCEDSTATUS"] = db.TRSESSIONSPROCEDSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            CodesController cc = new CodesController();
            var ses = db.TRSESSIONS.Find(id);
            bool IsAdmin = cc.IsAdmin();
            bool IsAdminCity = cc.IsAdminCity();
              if (!IsAdmin &&!IsAdminCity)
               {

                    var mycitynb = Utility.MyCityNb();
                    if (ses.SESCITYNB != mycitynb)
                    {
                        return RedirectToAction("Index");
                    }
               }
           
            

            var temp = DateTime.Parse(ses.SESDATE.ToString()).ToString("dd/MM/yyyy");
            ViewBag.SessionID = id;
            ViewBag.SessionNo = ses.SESNO;
            ViewBag.SessionDate = temp;
            ViewBag.SessionStatus = ses.TRSTATUS.NAME;
            ViewBag.SessionBossName = ses.COMBOSSNAME;
            ViewBag.FINISH_PRINT = ses.FINISH_PRINT;
            string sql = "SELECT COUNT(*) FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + id;
            ViewBag.SessionProExisting = db.Database.SqlQuery<int>(sql).FirstOrDefault();
            ViewBag.IS_ARCHIVED = ses.IS_ARCHIVED;
            ViewBag.SessionIsPrinted = db.Database.SqlQuery<int>("select count(*) from TRSESSIONS_REPORTS where SESSIONNB =" + id).FirstOrDefault();
            return View();
        }
        public ActionResult TRSESSIONSPROCEDS_Read([DataSourceRequest] DataSourceRequest request)
        {

            var nb = Request.Form["nb"].Trim();
            var carnb = Request.Form["carnb"].Trim();
            var carprocednb = Request.Form["carprocednb"].Trim();
            var procedtyp = Request.Form["procedtyp"].Trim();

            string sql = "  SELECT TSP.NB                  AS NB, "
                   + "    ZP.NAME AS PROCEDNAME, "
                   + "    ZP.NB AS PROCEDNB, "
                   + "     TSP.SESSIONNB AS SESSIONNB, "
                   + "       TSP.CARPROCEDNB AS CARPROCEDNB, "
                   + "       CP.RECDAT AS RECDAT, "
                   + "       TSP.PSTATUS AS PSTATUS, "
                   + "       CP.CARNB AS CARNB, "
                   + "       TSP.ORDR AS ORDR, "
                   + "       TSP.CARPROCEDSTEPNB AS CARPROCEDSTEPNB, "
                   + "       TSP.TYPSNAMEAGR AS TYPSNAMEAGR, "
                   + "       TSP.ISDONE AS ISDONE, "

                    + "       TSP.NOTES AS NOTES "

                  
                   + "    FROM TRSESSIONS_PROCEDS TSP "
                   + "       JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB "
                   + "        JOIN ZPROCEDTYPS ZP ON ZP.NB = CP.PROCEDNB "
                   + "      WHERE TSP.SESSIONNB = " + nb;

            if (carnb != "")
            {
                sql += " and CP.CARNB = " + carnb;
            }
            if (carprocednb != "")
            {
                sql += " and TSP.CARPROCEDNB = " + carprocednb;
            }
            if (procedtyp != "")
            {
                sql += " and ZP.NB = " + procedtyp;
            }

            sql += " ORDER BY TSP.ORDR ASC , TSP.ISDONE";
            var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS_VM>(sql).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TRSESSIONSPROCEDS_Exl(string pnb, string pcarnb, string pcarprocednb, string pprocedtyp)
        {

            var nb = pnb;
            var carnb = pcarnb;
            var carprocednb = pcarprocednb;
            var procedtyp = pprocedtyp;

            string sql = "SELECT TSP.NB   AS NB, "
                   + "  ZP.NAME AS PROCEDNAME, "
                   + "  ZP.NB AS PROCEDNB, "
                   + "  TSP.SESSIONNB AS SESSIONNB, "
                   + "  TSP.CARPROCEDNB AS CARPROCEDNB, "
                   + "  CP.RECDAT AS RECDAT, "
                   + "  TSP.PSTATUS AS PSTATUS, "
                   + "  CP.CARNB AS CARNB, "
                   + "  TSP.ORDR AS ORDR, "
                   + "  TSP.CARPROCEDSTEPNB AS CARPROCEDSTEPNB, "
                   + "  TSP.TYPSNAMEAGR AS TYPSNAMEAGR "
                   + "  FROM TRSESSIONS_PROCEDS TSP "
                   + "  JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB "
                   + "  JOIN ZPROCEDTYPS ZP ON ZP.NB = CP.PROCEDNB "
                   + "  WHERE TSP.SESSIONNB = " + nb;

            if (carnb != "")
            {
                sql += " and CP.CARNB = " + carnb;
            }
            if (carprocednb != "")
            {
                sql += " and TSP.CARPROCEDNB = " + carprocednb;
            }
            if (procedtyp != "")
            {
                sql += " and ZP.NB = " + procedtyp;
            }

            sql += " ORDER BY TSP.ORDR ASC ";
            var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS_VM>(sql).ToList();
            var data2 = from e in data
                        select new
                        {
                            اسم_المعاملة = e.PROCEDNAME,
                            رمز_الجلسة = e.SESSIONNB,
                            رمز_المعاملة = e.CARPROCEDNB,
                            تاريخ_المعاملة = e.RECDAT,
                            حالة_الطلب = e.PSTATUS,
                            رمز_المركبة = e.CARNB,
                            نوع_الموافقة = e.TYPSNAMEAGR,
                        };

            string sheetName = "المعلاملات في الجلسة";
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            ws.Cell(1, 1).InsertTable(data2);

            ws.RightToLeft = true;
            ws.ColumnWidth = 30;


            //ws.Style.Font.Bold = true;
            ws.Style.Font.FontSize = 12;
            for (int i = 1; i <= 7; i++)
            {
                ws.Column(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", sheetName.Replace(" ", "_")));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();

            return View();
        }
        public ActionResult GetCount555(long ID)
        {

            string sql = "SELECT COUNT (*) AS TOTALCOUNT, NVL (SUM (CASE WHEN PSTATUS = 1 THEN 1 ELSE 0 END),0) AS AGREE, NVL (SUM (CASE WHEN PSTATUS = 2 THEN 1 ELSE 0 END),0) AS NOTAGREE, NVL (SUM (CASE WHEN PSTATUS = 3 THEN 1 ELSE 0 END),0) AS DELAYED, NVL (SUM (CASE WHEN PSTATUS = 4 THEN 1 ELSE 0 END),0) AS UNANSWERED  FROM TRSESSIONS_PROCEDS WHERE SESSIONNB =" + ID;

            var data = db.Database.SqlQuery<CountTotal>(sql).FirstOrDefault();
            var countproced = ViewBag.Sessionprocedisanswer = db.Database.SqlQuery<int>("select count(*) from TRSESSIONS_PROCEDS where PSTATUS != 4 and SESSIONNB = " + ID).FirstOrDefault();

            return Json(new { success = true, TOTALCOUNT = data.TOTALCOUNT, AGREE = data.AGREE, NOTAGREE = data.NOTAGREE, DELAYED = data.DELAYED, UNANSWERED = data.UNANSWERED, countproced = countproced }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Agree(long nb, string note)
        {
            try
            {
               // var data = db.TRSESSIONS_PROCEDS.Find(nb);

                var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("select * from TRSESSIONS_PROCEDS where nb ="+ nb).FirstOrDefault();
                var setp = db.CARPROCEDSTEPS.Find(data.CARPROCEDSTEPNB);
                var proced = db.CARPROCEDS.Find(data.CARPROCEDNB);
                if (proced.RESULT != "جارية")
                {
                    return Json(new { success = false, responseText = "المعاملة ليست جارية" }, JsonRequestBehavior.AllowGet);
                }
                if (setp.DONE == 1)
                {
                    return Json(new { success = false, responseText = "الخطوة منفذة" }, JsonRequestBehavior.AllowGet);
                }

                data.PSTATUS = 1;
                data.NOTES = note;
                data.RESPONSABLE = Utility.MyName();
                data.RESPONSABLENB = Utility.MyNB();
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }

        }
        public bool AgreeAll(long Sesnb)
        {
            try
            {
                var ses = db.TRSESSIONS.Find(Sesnb);
                var ListOfProceds = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("SELECT * FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + Sesnb + " and PSTATUS = 1 and ISDONE != 1").ToList();
                foreach (var proceds in ListOfProceds)
                {
                    var stepnb = proceds.CARPROCEDSTEPNB;
                    long? status = 0;
                    string sql = "BEGIN VEHICLES.PASSENGERS_PKG.ACCEPT_PROCED(:PSESNB,:PSTEPNB ,:PSTATUS); END;";
                    db.Database.ExecuteSqlCommand(sql, Sesnb, stepnb, status);
                    if (status == 0)
                    {
                        proceds.ISDONE = 1;
                        db.Entry(proceds).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                }
               
                var ListOfProcedsNO = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("SELECT * FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + Sesnb + " and PSTATUS = 2 ").ToList();
                foreach (var proceds in ListOfProcedsNO)
                {
                    var prostep = db.CARPROCEDSTEPS.Find(proceds.CARPROCEDSTEPNB);
                    prostep.DONE = 1;
                    prostep.RESPONSABLE = "جلسة نقل الركاب";
                    prostep.NOTE = prostep.NOTE + "عدم موافقة في جلسة الرمز: " + ses.NB + " والرقم: " + ses.SESNO + " والتاريخ:" + ses.SESDATE?.ToString("dd/MM/yyyy") +" محافظة : " + ses.ZCITY.NAME;

                    db.Entry(prostep).State = EntityState.Modified;

                    var pro = db.CARPROCEDS.Find(proceds.CARPROCEDNB);
                    pro.RESULT = "منتهية";
                    pro.NOTE = pro.NOTE + "تم انهاء المعاملة بسبب " + "عدم موافقة في جلسة الرمز: " + ses.NB + " والرقم: " + ses.SESNO + " والتاريخ:" + ses.SESDATE?.ToString("dd/MM/yyyy") + " محافظة : " + ses.ZCITY.NAME;
                    pro.EDATE = DateTime.Now;
                    db.Entry(pro).State = EntityState.Modified;
                    db.SaveChanges();
                    proceds.ISDONE = 1;
                    db.Entry(proceds).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var ListOfProcedsDE = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("SELECT * FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + Sesnb + " and PSTATUS = 3 ").ToList();
                foreach (var proceds in ListOfProcedsDE)
                {
                    proceds.ISDONE = 1;
                    db.Entry(proceds).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ActionResult NotAgree(long nb, string note)
        {
            try
            {
                var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("select * from TRSESSIONS_PROCEDS where nb =" + nb).FirstOrDefault();
                var setp = db.CARPROCEDSTEPS.Find(data.CARPROCEDSTEPNB);
                var proced = db.CARPROCEDS.Find(data.CARPROCEDNB);
                if (proced.RESULT != "جارية")
                {
                    return Json(new { success = false, responseText = "المعاملة ليست جارية" }, JsonRequestBehavior.AllowGet);
                }
                if (setp.DONE == 1)
                {
                    return Json(new { success = false, responseText = "الخطوة منفذة" }, JsonRequestBehavior.AllowGet);
                }
                data.PSTATUS = 2;
                data.NOTES = note;
                data.RESPONSABLE = Utility.MyName();
                data.RESPONSABLENB = Utility.MyNB();
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delay(long nb,string note)
        {
            try
            {
              
                var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("select * from TRSESSIONS_PROCEDS where nb =" + nb).FirstOrDefault();
                var setp = db.CARPROCEDSTEPS.Find(data.CARPROCEDSTEPNB);
                var proced = db.CARPROCEDS.Find(data.CARPROCEDNB);
                if (proced.RESULT != "جارية")
                {
                    return Json(new { success = false, responseText = "المعاملة ليست جارية" }, JsonRequestBehavior.AllowGet);
                }
                if (setp.DONE == 1)
                {
                    return Json(new { success = false, responseText = "الخطوة منفذة" }, JsonRequestBehavior.AllowGet);
                }
                data.PSTATUS = 3;
                data.NOTES = note;
                data.RESPONSABLE = Utility.MyName();
                data.RESPONSABLENB = Utility.MyNB();
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ConSession(long ID)
        {
            try
            {
                var data = db.TRSESSIONS.Find(ID);
                if (data.STATUS != 1)
                {
                    return Json(new { success = false, responseText = "حالة الجلسة ليست فعالة" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    data.STATUS = 2;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    // return View("TRSESSIONSPROCEDS?id=181");
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CancelSession(long ID)
        {
            try
            {
                var data = db.TRSESSIONS.Find(ID);
                if (data.STATUS != 1)
                {
                    return Json(new { success = false, responseText = "حالة الجلسة ليست فعالة" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    data.STATUS = 3;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    // return View("TRSESSIONSPROCEDS?id=181");
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult FinishSession(long ID)
        {
            try
            {
                var data = db.TRSESSIONS.Find(ID);
                if (data.STATUS != 2)
                {
                    return Json(new { success = false, responseText = "حالة الجلسة ليست منعقدة" }, JsonRequestBehavior.AllowGet);
                }
                var res = AgreeAll(ID);
                if (res == true)
                {
                    var ListOfProceds = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("SELECT * FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + ID + " and PSTATUS = 4 ").ToList();
                    if (ListOfProceds.Count == 0)
                    {
                        data.STATUS = 0;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, responseText = "تم الانتهاء من ادخال جميع الطلبات" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, responseText = "يوجد المزيد من الطلبات يرجى ادخال نتيجتها" }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {

                    return Json(new { success = false, responseText = "حدث خطأ اثناء التنفيذ" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintReportSession(long? ID)
        {
            var ses = db.TRSESSIONS.Find(ID);

            if (ses.STATUS == 1 || ses.STATUS == 2)
            {
                string sql = "BEGIN VEHICLES.PASSENGERS_PKG.TRSESSIONS_REPORTS_PASSENGERS(:SESNB); END;";
                db.Database.ExecuteSqlCommand(sql, ID);
                db.SaveChanges();
            }
            if (ses.STATUS == 0 && ses.FINISH_PRINT == 0)
            {
                try 
                {
                    string sql = "BEGIN VEHICLES.PASSENGERS_PKG.TRSESSIONS_REPORTS_PASSENGERS(:SESNB); END;";
                    db.Database.ExecuteSqlCommand(sql, ID);
                    ses.FINISH_PRINT = 1;
                    db.Entry(ses).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch 
                { 

                }
               
                
            }
            var sesdatayear = DateTime.Parse(ses.SESDATE.ToString()).ToString("yyyy");
            var temp = DateTime.Parse(ses.SESDATE.ToString()).ToString("dd/MM/yyyy");
            ViewBag.SessionID = ID;
            ViewBag.SessionNo = ses.SESNO;
            ViewBag.SessionDate = temp;
            ViewBag.SessionStatus = ses.TRSTATUS.NAME;
            //var sql = "SELECT TSM.NB ,TSM.SESSIONNB , TSM.MEMBERNB , TSM.ISPRESENT ,TM.MEMBERNAME ,TM.MEMBERSHIPNB , TM.MEMBERPOSITIONNB ,TS.STATUS AS SESSIONSTATUS FROM TRSESSIONS_MEMBERS_PRESENT  TSM JOIN TRCOMMITTEES_MEMBERS TM ON TM.NB = TSM.MEMBERNB JOIN TRSESSIONS TS ON TS.NB = TSM.SESSIONNB  WHERE TSM.SESSIONNB  =  " + ID;
            // var data = db.Database.SqlQuery<TRSESSIONS_MEMBERS_PRESENT>("select * from TRSESSIONS_MEMBERS_PRESENT where SESSIONNB =" + ID).ToList();
            var data = db.TRSESSIONS_MEMBERS_PRESENT.Where(x => x.SESSIONNB == ID && x.ISPRESENT == 1).OrderBy(x => x.ORDR);
            List<string> Members = new List<string>();
            List<string> sigMembers = new List<string>();

            foreach (var member in data)
            {
                if (member.MEMBERSHIPNB != 1)
                {
                    var temp3 = "";

                    temp3 += "السيد " + member.TRCOMMITTEES_MEMBERS.MEMBERNAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + " / " + member.TRZMEMBERSHIP.NAME;
                    Members.Add(temp3);
                }
                else
                {
                    ViewBag.SessionBossName = "السيد " + member.TRCOMMITTEES_MEMBERS.MEMBERNAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + " / " + member.TRZMEMBERSHIP.NAME;
                }
            }
            var data1 = data.OrderByDescending(x => x.ORDR).ToList();
            foreach (var member in data1)
            {
                var temp4 = "";
                temp4 += member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + Environment.NewLine + member.TRZMEMBERSHIP.NAME + Environment.NewLine + member.TRCOMMITTEES_MEMBERS.MEMBERNAME;

                sigMembers.Add(temp4);
            }
            ViewBag.SessionMemers = Members;
            ViewBag.sigSessionMemers = sigMembers;



            var sql1 = "SELECT DISTINCT PROCEDNB FROM TRSESSIONS_REPORTS WHERE RYEAR = " + sesdatayear + " and SESSIONNB = " + ses.NB;
            var listofproed = db.Database.SqlQuery<long>(sql1).ToList();

            List<ListPROCEDS_Print_ALL> ListPROCEDS_Print_ALL = new List<ListPROCEDS_Print_ALL>();
            foreach (var item in listofproed)
            {
                ListPROCEDS_Print_ALL ofpro = new ListPROCEDS_Print_ALL();
                ofpro.pronb = item;
                ofpro.proname = db.ZPROCEDTYPS.Where(x => x.NB == item).Select(x => x.NAME).FirstOrDefault();
                var sql = "SELECT * FROM TRSESSIONS_REPORTS WHERE RYEAR = " + sesdatayear + " and SESSIONNB = " + ses.NB + "and PROCEDNB = " + item;
                ofpro.pro = db.Database.SqlQuery<PROCEDS_Print_ALL>(sql).ToList();
                ListPROCEDS_Print_ALL.Add(ofpro);
            }





            return View(ListPROCEDS_Print_ALL);
        }
        public ActionResult GetProcedinfo(long NB, long PRONB)
        {
            // حالة الغاء خط
            if (PRONB == 2002)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var LINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                // var data = db.TRLINES.Find(LINENB);
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + LINENB).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("select zc.name from TRLINE_CITY tr join zcitys zc on zc.nb = tr.CITYNB where  tr.LINENB = " + LINENB).ToList();
                return PartialView("_ProcedInfo");
            }
            //معاملة احداث خط
            else if (PRONB == 2001)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var data = db.Database.SqlQuery<string>("select NAME from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName = data;
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("select zc.name from PROCED_LINES pl join PROCED_LINES_CITY pc on pc.PROCEDLINENB = pl.nb join zcitys zc on zc.nb = pc.CITYNB where pl.CARPROCEDNB =  " + pro).ToList();
                return PartialView("_ProcedInfo");
            }
            // معاملة دمج خطين
            else if (PRONB == 2004)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var LINENB1 = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                var LINENB2 = db.Database.SqlQuery<long>("select LINENB2 from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName1 = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + LINENB1).ToList();
                ViewBag.LineName2 = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + LINENB2).ToList();
                ViewBag.LineNew = db.Database.SqlQuery<string>("select NAME from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                //ViewBag.LineCity = db.Database.SqlQuery<string>("select zc.name from PROCED_LINES pl join PROCED_LINES_CITY pc on pc.PROCEDLINENB = pl.nb join zcitys zc on zc.nb = pc.CITYNB where pl.CARPROCEDNB =  " + pro).ToList();
                return PartialView("_ProcedInfo");
            }
            // تغير اسم او مسار خط
            else if (PRONB == 2003)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var LINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + LINENB).ToList();
                ViewBag.LineNew = db.Database.SqlQuery<string>("select NAME from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + LINENB).ToList();
                ViewBag.NEWLineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM PROCED_LINES  pl JOIN PROCED_LINES_CITY pc ON pc.PROCEDLINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.CARPROCEDNB = " + pro).ToList();
                return PartialView("_ProcedInfo");
            }
            //منح موافقة خط سير 
            else if (PRONB == 2006)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var LINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + LINENB).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + LINENB).ToList();
                return PartialView("_ProcedInfo");
            }
            //منح موافقة الغاء خط سير المحافظة المصدر
            else if (PRONB == 2008)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var LINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + LINENB).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + LINENB).ToList();
                return PartialView("_ProcedInfo");
            }
            //منح موافقة تغيير فئة مركبة عامة إلى خاصة
            else if (PRONB == 2012)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
                var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
                ViewBag.carnb = car.NB;
                ViewBag.tabnb = car.TABNU;
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
                return PartialView("_ProcedInfo");
            }
            //منح موافقة ترقين مركبة عامة
            else if (PRONB == 2014)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
                var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
                ViewBag.carnb = car.NB;
                ViewBag.tabnb = car.TABNU;
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
                return PartialView("_ProcedInfo");
            }
            //منح موافقة تغيير خط سير مركبة استثمار
            else if (PRONB == 2016)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
                var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
                ViewBag.carnb = car.NB;
                ViewBag.tabnb = car.TABNU;
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
                var NEWLINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.NEWLineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + NEWLINENB).ToList();
                ViewBag.NEWLineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + NEWLINENB).ToList();



                return PartialView("_ProcedInfo");
            }
            //منح موافقة تغيير خط سير مركبة عامة
            else if (PRONB == 2018)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
                var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
                ViewBag.carnb = car.NB;
                ViewBag.tabnb = car.TABNU;
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
                var NEWLINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.NEWLineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + NEWLINENB).ToList();
                ViewBag.NEWLineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + NEWLINENB).ToList();



                return PartialView("_ProcedInfo");
            }

            else if (PRONB == 2023)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
                var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
                ViewBag.carnb = car.NB;
                ViewBag.tabnb = car.TABNU;
                var proced_line = db.Database.SqlQuery<PROCED_LINES>("select * from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();
                ViewBag.TRIP_PATH = proced_line.TRIP_PATH;
                ViewBag.CONTRACT_PERIOD = proced_line.CONTRACT_PERIOD;
                ViewBag.DAY_TRIP_FROM = proced_line.DAY_TRIP_FROM;
                ViewBag.DAY_TRIP_TO = proced_line.DAY_TRIP_TO;
                ViewBag.NIGHT_TRIP_FROM = proced_line.NIGHT_TRIP_FROM;
                ViewBag.NIGHT_TRIP_TO = proced_line.NIGHT_TRIP_TO;
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
              //  var NEWLINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                //ViewBag.NEWLineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + NEWLINENB).ToList();
              //  ViewBag.NEWLineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + NEWLINENB).ToList();



                return PartialView("_ProcedInfo");
            }

            else if (PRONB == 2022)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
                var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
                ViewBag.carnb = car.NB;
                ViewBag.tabnb = car.TABNU;
                var proced_line = db.Database.SqlQuery<PROCED_LINES>("select * from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();
             
                
                ViewBag.TRAVEL_MONTH = proced_line.TRAVEL_MONTH;
                ViewBag.TRAVEL_COUNTRY = proced_line.TRAVEL_COUNTRY;
                ViewBag.TRAVEL_BOARDER = proced_line.TRAVEL_BOARDER;
               


                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
                //  var NEWLINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                //ViewBag.NEWLineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + NEWLINENB).ToList();
                //  ViewBag.NEWLineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + NEWLINENB).ToList();



                return PartialView("_ProcedInfo");
            }

            else if (PRONB == 2000)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var Proced = db.CARPROCEDS.Find(pro);
               // var car = db.Database.SqlQuery<CAR>("select * from cars where nb = " + Proced.CARNB).FirstOrDefault();
               // ViewBag.carnb = car.NB;
               // ViewBag.tabnb = car.TABNU;
               var proced_line = db.Database.SqlQuery<PROCED_LINES>("select * from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
               // ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + car.LIN).ToList();


                ViewBag.POSTMAIL = proced_line.POSTMAIL;
                ViewBag.SENDER = proced_line.SENDER;
                ViewBag.POSTNO = proced_line.POSTNO;
                ViewBag.POSTDATE = proced_line.POSTDATE;

                
                ViewBag.ProcedName = db.ZPROCEDTYPS.Find(PRONB).NAME;
                ViewBag.ProcedNb = PRONB;
                //ViewBag.LineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + car.LIN).ToList();
                //  var NEWLINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
                //ViewBag.NEWLineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = " + NEWLINENB).ToList();
                //  ViewBag.NEWLineCity = db.Database.SqlQuery<string>("SELECT zc.name FROM TRLINES  pl JOIN TRLINE_CITY pc ON pc.LINENB  = pl.nb JOIN zcitys zc ON zc.nb = pc.CITYNB WHERE pl.NB = " + NEWLINENB).ToList();



                return PartialView("_ProcedInfo");
            }
            else
            {
                return PartialView("_ProcedInfo");
            }
        }
        public ActionResult GetProcedCount(long? ID)
        {
            try
            {
                var countproced = db.Database.SqlQuery<int>("select count(*) from TRSESSIONS_PROCEDS where PSTATUS != 4 and SESSIONNB = " + ID).FirstOrDefault();
                if (countproced == 0)
                {
                    //var data = db.Database.SqlQuery<TRPROCEDS_AVAILABLEVM>("SELECT PV.NB, PV.PROCEDNB  ,  PV.COUNTAVAILABLE ,PV.COUNTPROCED, ZP.NAME FROM TRPROCEDS_AVAILABLE PV JOIN ZPROCEDTYPS ZP ON ZP.NB =  PV.PROCEDNB").ToList();
                    //ViewBag.Listdata = data;
                    //ViewBag.sesID = data[0].SESSIONNB;
                    var pro_avai_count = db.Database.SqlQuery<int>("select count(*) from TRPROCEDS_AVAILABLE where SESSIONNB = " + ID).FirstOrDefault();
                    if (pro_avai_count !=0)
                    {
                        var ssess = db.TRSESSIONS.Find(ID);
                        var pro_count_in_ses = db.Database.SqlQuery<int>("select sum(COUNTAVAILABLE) from TRPROCEDS_AVAILABLE where SESSIONNB = " + ID).FirstOrDefault();
                        var sss = " SELECT COUNT (*)     AS COUNTPROCED "
                                + " FROM CARPROCEDSTEPS  CPS "
                                + " JOIN CARPROCEDS CP ON CPS.CARPROCEDNB = CP.NB "
                                + " JOIN ZPROCEDTYPS ZP ON ZP.NB = CP.PROCEDNB "
                                + " LEFT JOIN CARS CA ON CA.NB = CP.CARNB "
                                + " WHERE ZP.TYP = 'TRANSSESSION' "
                                + " AND CP.RESULT = 'جارية' "
                                + " AND(CPS.STEPNB = 453 OR CPS.STEPNB = 454) "
                                + " AND CPS.CITYNB = " + ssess.SESCITYNB + " "
                                + " AND CPS.DONE = 3"
                                + " AND(CA.REG != 1 or CA.REG is null) "
                                + " AND CA.CARREG != 6 ";
                        var pro_count_notin_ses = db.Database.SqlQuery<int>(sss).FirstOrDefault();
                        if (pro_count_in_ses != pro_count_notin_ses)
                        {
                            ViewBag.isnewproced = "يوجد معاملات جديدة تم اضافتها هل تريد تحديث قائمة الطلبات";
                        }
                    }
                    return PartialView("_ProcedCount");
                }
                else

                {
                    return Json(new { success = false, responseText = "لا يمكن تغير عدد طلبات الجلسة" }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetProcedCountIstrue(long Sesnb)
        {
            var stat = db.TRSESSIONS.Where(x => x.NB == Sesnb).Select(s => s.STATUS).FirstOrDefault();
            if (stat != 1)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            string sql = "SELECT COUNT(*) FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + Sesnb;

            var data = db.Database.SqlQuery<int>(sql).FirstOrDefault();
            if (data == 0)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Read_TRPROCEDS_AVAILABLE([DataSourceRequest] DataSourceRequest request, long nb)
        {
            var sql="";
            var ses = db.TRSESSIONS.Find(nb);
            var pro_avai_count = db.Database.SqlQuery<int>("select count(*) from TRPROCEDS_AVAILABLE where SESSIONNB = "+ nb).FirstOrDefault();
            if (pro_avai_count == 0 ) 
            {
                 sql = " SELECT  1 as NB ,"
                     + " CP.PROCEDNB AS PROCEDNB,"
                     + " COUNT(*) AS COUNTAVAILABLE,"
                     + " COUNT(*) AS COUNTPROCED,"
                     + " zp.name AS NAME , 1 AS SESSIONNB"
                     + " FROM CARPROCEDSTEPS CPS "
                     + " JOIN CARPROCEDS CP ON CPS.CARPROCEDNB = CP.NB "
                     + " JOIN ZPROCEDTYPS ZP ON ZP.NB = CP.PROCEDNB "
                     + " LEFT JOIN CARS CA ON CA.NB = CP.CARNB "
                     + " WHERE ZP.TYP = 'TRANSSESSION' "
                     + " AND CP.RESULT = 'جارية' "
                     + " AND(CPS.STEPNB = 453 OR CPS.STEPNB = 454) "
                     + " AND CPS.CITYNB = " + ses.SESCITYNB + " "
                     + " AND CPS.DONE = 3 "
                   + " AND(CA.REG != 1 or CA.REG is null) "
                    + "  AND(CA.CARREG != 6 OR CA.CARREG IS NULL) "

                     + " GROUP BY CP.PROCEDNB,zp.name ";
            }
            else 
            {
                   sql = "SELECT PV.NB, PV.PROCEDNB  ,"
                        +"PV.COUNTAVAILABLE ,PV.COUNTPROCED, "
                        +"ZP.NAME ,PV.SESSIONNB "
                        +"FROM TRPROCEDS_AVAILABLE PV "
                        +"JOIN ZPROCEDTYPS ZP ON ZP.NB =  PV.PROCEDNB "
                        +"where PV.SESSIONNB = "+ nb;

            }

            var data = db.Database.SqlQuery<TRPROCEDS_AVAILABLEVM>(sql).ToList();
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                PROCEDNB = commm.PROCEDNB,
                COUNTAVAILABLE = commm.COUNTAVAILABLE,
                NAME = commm.NAME,
                COUNTPROCED = commm.COUNTPROCED,
                SESSIONNB = commm.SESSIONNB,
            });
            return Json(result);
        }
        //public ActionResult UpdateAll([Bind(Prefix = "models")] IEnumerable<TRPROCEDS_AVAILABLEVM> model)
        //{
        //    if (model != null && ModelState.IsValid)
        //    {
        //        foreach (var item in model)
        //        {
        //            try
        //            {
        //                var data = db.TRPROCEDS_AVAILABLE.Find(item.NB);
        //                data.COUNTPROCED = item.COUNTPROCED;
        //                db.Entry(data).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //            catch (Exception ex)
        //            {
        //                var ss = validation.OracleExceptionValidation(ex);
        //                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult SetProcedCount(long Sesnb)
        {
            try
            {
                var sql2 = "delete from TRSESSIONS_PROCEDS where SESSIONNB = " + Sesnb;
                var rowsAffected2 = db.Database.ExecuteSqlCommand(sql2);
                db.SaveChanges();

                long? status = 0;
                string sql = "BEGIN VEHICLES.PASSENGERS_PKG.ADD_SESSION_PROCEDS(:PSESNB,:PSTATUS); END;";
                db.Database.ExecuteSqlCommand(sql, Sesnb, status);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }


        }   
        public ActionResult Saveprocount(string mod,long? sesnb)
        {

            try
            {
                var pro_avai_count = db.Database.SqlQuery<int>("select count(*) from TRPROCEDS_AVAILABLE where SESSIONNB = " + sesnb).FirstOrDefault();
                var subjects = JsonConvert.DeserializeObject<List<TRPROCEDS_AVAILABLEVM>>(mod);
                if (pro_avai_count == 0)
                {


                    foreach (var item in subjects)
                    {
                        TRPROCEDS_AVAILABLE proav = new TRPROCEDS_AVAILABLE();
                        proav.PROCEDNB = item.PROCEDNB;
                        proav.COUNTAVAILABLE = item.COUNTAVAILABLE;
                        proav.COUNTPROCED = item.COUNTPROCED;
                        proav.SESSIONNB = sesnb;
                        db.TRPROCEDS_AVAILABLE.Add(proav);
                        db.SaveChanges();

                    }

                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in subjects)
                    {
                        var data = db.TRPROCEDS_AVAILABLE.Find(item.NB);
                        if (data.COUNTPROCED != item.COUNTPROCED)  
                        {
                            data.COUNTPROCED = item.COUNTPROCED;
                        }
                        
                        db.SaveChanges();

                    }

                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
                       
        }
        public ActionResult refpro(long Sesnb)
        {
            try
            {
                var sql = "delete from TRPROCEDS_AVAILABLE where SESSIONNB = "+ Sesnb;
                var sql2 = "delete from TRSESSIONS_PROCEDS where SESSIONNB = " + Sesnb;

                var can_i_delete = db.Database.SqlQuery<int>("select count(*) from TRSESSIONS_PROCEDS where SESSIONNB = "+ Sesnb + " and PSTATUS != 4").FirstOrDefault();
                if (can_i_delete > 0 ) 
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var rowsAffected = db.Database.ExecuteSqlCommand(sql);
                    var rowsAffected2 = db.Database.ExecuteSqlCommand(sql2);
                    db.SaveChanges();

                }
               
               
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }


        }     
        public ActionResult SaveSingleDocument(HttpPostedFileBase Files,long sesnb)
        {
            try
            {
                var ses = db.TRSESSIONS.Find(sesnb);

                if (ses.IS_ARCHIVED == true)
                {
                    return Json(new { success = false, responseText = "المحضر مؤرشف مسبقاً" }, JsonRequestBehavior.AllowGet);

                }
                byte[] fileContent = null;
                using (var reader = new System.IO.BinaryReader(Files.InputStream))
                {
                    fileContent = reader.ReadBytes(Files.ContentLength);
                }

                var date = DateTime.Now;
                var pathNameYear = date.ToString("yyyy");
                pathNameYear += "/" + ses.SESCITYNB + "/";
                var FTPFullPath = ConfigurationManager.AppSettings["FtpHomeDirectory"];
                FTPFullPath += pathNameYear;
               
                var uploadedFullPath = CodesController.UploadFile(fileContent, FTPFullPath, Files.FileName, FTPFullPath, sesnb);


                if (ses != null)
                {
                    ses.FTP_PATH = uploadedFullPath;
                    ses.IS_ARCHIVED = true;
                    db.Entry(ses).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception ex) 
            { 
            return Json(new { success = false, responseText = "حدث خطأ اثناء الارشفة" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = true, responseText = "تم ارشفة الملف" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetReport(long NB)
        {
            try
            {
                CodesController cc = new CodesController();
                var ses = db.TRSESSIONS.Find(NB);
                bool IsAdmin = cc.IsAdmin();
                bool IsAdminCity = cc.IsAdminCity();
                if (!IsAdmin && !IsAdminCity)
                {

                    var mycitynb = Utility.MyCityNb();
                    if (ses.SESCITYNB != mycitynb)
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
        public ActionResult DeleteDocument(long sesnb)
        {
            try
            {
                var data = db.TRSESSIONS.Find(sesnb);
                if (data != null)
                {
                    if (data.IS_ARCHIVED == false)
                    {
                        return Json(new { success = false, responseText = "لا يوجد ارشفة لهذه الجلسة " }, JsonRequestBehavior.AllowGet);

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
    
    
