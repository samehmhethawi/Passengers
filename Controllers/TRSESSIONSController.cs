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

namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class TRSESSIONSController : Controller
    {
        private ProcedContext db = new ProcedContext();
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
            var sql = "select * from TRSESSIONS where 1 = 1 ";
            var SESNO = Request.Form["SESNO"].Trim();
            var SESDATESTART = Request.Form["SESDATESTART"].Trim();
            var SESDATEEND = Request.Form["SESDATEEND"].Trim();
            var STATUS = Request.Form["STATUS"].Trim();
            var SESCITYNB = Request.Form["SESCITYNB"].Trim();




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
                sql += " and SESDATE >= TO_DATE('" + SESDATESTART + "','DD/MM/YYYY') ";
            }

            if (SESDATEEND != "")
            {
                sql += " and SESDATE <= TO_DATE('" + SESDATEEND + "','DD/MM/YYYY') ";
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

            sql += " order by nb desc";
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
               

                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
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
                    var is_session_exists = db.Database.SqlQuery<int>("select 1 from TRSESSIONS where SESCITYNB = " + model.SESCITYNB + " and STATUS = 1").FirstOrDefault();
                  
                    if (is_session_exists == 1)
                    {
                        return Json(new { success = false, responseText = "يوجد جلسة فعالة لهذه المحافظة " });
                    }
                    else
                    {
                        var comnb = db.Database.SqlQuery<int>("select nb from TRCOMMITTEES where COMCITYNB = " + model.SESCITYNB + " and STATUS = 1").FirstOrDefault();
                        model.COMMITTEENB = comnb;
                        db.TRSESSIONS.Add(model);
                        db.SaveChanges();
                        return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                }
            

               

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() });
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
                return Json(new { success = false, responseText = ex.ToString()
    });
            }
        }
        public ActionResult Read_Member_Session([DataSourceRequest] DataSourceRequest request , long NB)
        {
            var sql = "SELECT TSM.NB ,TSM.SESSIONNB , TSM.MEMBERNB , TSM.ISPRESENT ,TM.MEMBERNAME ,TSM.MEMBERSHIPNB , TM.MEMBERPOSITIONNB ,TS.STATUS AS SESSIONSTATUS FROM TRSESSIONS_MEMBERS_PRESENT  TSM JOIN TRCOMMITTEES_MEMBERS TM ON TM.NB = TSM.MEMBERNB JOIN TRSESSIONS TS ON TS.NB = TSM.SESSIONNB  WHERE TSM.SESSIONNB  =  " + NB; 

            var data = db.Database.SqlQuery<TRSESSIONS_MEMBERS_PRESENTVM>(sql);



            return Json(data.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditMEMBERSHIP(long nb , long MEMBERSHIPNB)
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
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
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
        public ActionResult TRSESSIONSPROCEDS(long id )
        {
            ViewData["TRSESSIONSPROCEDSTATUS"] = db.TRSESSIONSPROCEDSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
             var ses = db.TRSESSIONS.Find(id);
        
            var temp = DateTime.Parse(ses.SESDATE.ToString()).ToString("dd/MM/yyyy");
            ViewBag.SessionID = id;
            ViewBag.SessionNo = ses.SESNO;
            ViewBag.SessionDate = temp;
            ViewBag.SessionStatus = ses.TRSTATUS.NAME;
            ViewBag.SessionBossName = ses.COMBOSSNAME;



            return View();
        }
        public ActionResult TRSESSIONSPROCEDS_Read([DataSourceRequest] DataSourceRequest request ,long? nb)
        {
            string sql = "SELECT TSP.NB AS NB ,ZP.NAME  AS PROCEDNAME , ZP.NB AS PROCEDNB, TSP.SESSIONNB  AS SESSIONNB, TSP.CARPROCEDNB AS CARPROCEDNB, CP.RECDAT  AS RECDAT, TSP.PSTATUS   AS PSTATUS, CP.CARNB  AS CARNB, TSP.ORDR AS ORDR, TSP.CARPROCEDSTEPNB  AS CARPROCEDSTEPNB  FROM TRSESSIONS_PROCEDS  TSP JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB JOIN ZPROCEDTYPS ZP ON ZP.NB = CP.PROCEDNB where TSP.SESSIONNB = "+nb+" ORDER BY TSP.ORDR ASC";
            var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS_VM>(sql).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCount555(long ID)
        {

            string sql = "SELECT COUNT (*) AS TOTALCOUNT, NVL (SUM (CASE WHEN PSTATUS = 1 THEN 1 ELSE 0 END),0) AS AGREE, NVL (SUM (CASE WHEN PSTATUS = 2 THEN 1 ELSE 0 END),0) AS NOTAGREE, NVL (SUM (CASE WHEN PSTATUS = 3 THEN 1 ELSE 0 END),0) AS DELAYED, NVL (SUM (CASE WHEN PSTATUS = 4 THEN 1 ELSE 0 END),0) AS UNANSWERED  FROM TRSESSIONS_PROCEDS WHERE SESSIONNB =" + ID;

            var data = db.Database.SqlQuery<CountTotal>(sql).FirstOrDefault();
          var countproced = ViewBag.Sessionprocedisanswer = db.Database.SqlQuery<int>("select count(*) from TRSESSIONS_PROCEDS where PSTATUS != 4 and SESSIONNB = " + ID).FirstOrDefault();

            return Json(new { success = true, TOTALCOUNT = data.TOTALCOUNT , AGREE = data.AGREE , NOTAGREE = data.NOTAGREE , DELAYED = data.DELAYED , UNANSWERED = data.UNANSWERED , countproced = countproced }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Agree(long nb)
        {
            try
            {
                var data = db.TRSESSIONS_PROCEDS.Find(nb);
                data.PSTATUS = 1;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public  bool AgreeAll(long Sesnb)
        {
            try
            {
                var ListOfProceds = db.Database.SqlQuery<TRSESSIONS_PROCEDS>("SELECT * FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = " + Sesnb + " and PSTATUS = 1 ").ToList();
                foreach (var proceds in ListOfProceds)
                {
                    var stepnb = proceds.CARPROCEDSTEPNB;
                    long? status = 0; 
                    string sql = "BEGIN VEHICLES.PASSENGERS_PKG.ACCEPT_PROCED(:PSESNB,:PSTEPNB ,:PSTATUS); END;";
                    db.Database.ExecuteSqlCommand(sql, Sesnb, stepnb, status);
                } 
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ActionResult NotAgree(long nb)
        {
            try
            {
                var data = db.TRSESSIONS_PROCEDS.Find(nb);
                data.PSTATUS = 2;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delay(long nb)
        {
            try
            {
                var data = db.TRSESSIONS_PROCEDS.Find(nb);
                data.PSTATUS = 3;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CloseSession(long ID)
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
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public  ActionResult  FinishSession  (long ID)
        {
            try
            {
                 var res  =  AgreeAll(ID);
                if (res ==  true) {
                    var data = db.TRSESSIONS.Find(ID);
                    if (data.STATUS != 1)
                    {
                        return Json(new { success = false, responseText = "حالة الجلسة ليست فعالة" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //data.STATUS = 2;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else {
                    return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }                                            
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintReportSession(long? ID)
        {


            var ses = db.TRSESSIONS.Find(ID);
            if (ses.STATUS == 1) 
            {
                string sql = "BEGIN VEHICLES.PASSENGERS_PKG.TRSESSIONS_REPORTS_PASSENGERS(:SESNB); END;";

   
                db.Database.ExecuteSqlCommand(sql,ID);
                db.SaveChanges();
            }

            var sesdatayear = DateTime.Parse(ses.SESDATE.ToString()).ToString("yyyy");
            var temp = DateTime.Parse(ses.SESDATE.ToString()).ToString("dd/MM/yyyy");
            ViewBag.SessionID = ID;
            ViewBag.SessionNo = ses.SESNO;
            ViewBag.SessionDate = temp;
            ViewBag.SessionStatus = ses.TRSTATUS.NAME;
           //var sql = "SELECT TSM.NB ,TSM.SESSIONNB , TSM.MEMBERNB , TSM.ISPRESENT ,TM.MEMBERNAME ,TM.MEMBERSHIPNB , TM.MEMBERPOSITIONNB ,TS.STATUS AS SESSIONSTATUS FROM TRSESSIONS_MEMBERS_PRESENT  TSM JOIN TRCOMMITTEES_MEMBERS TM ON TM.NB = TSM.MEMBERNB JOIN TRSESSIONS TS ON TS.NB = TSM.SESSIONNB  WHERE TSM.SESSIONNB  =  " + ID;
           // var data = db.Database.SqlQuery<TRSESSIONS_MEMBERS_PRESENT>("select * from TRSESSIONS_MEMBERS_PRESENT where SESSIONNB =" + ID).ToList();
            var data = db.TRSESSIONS_MEMBERS_PRESENT.Where(x=>x.SESSIONNB == ID);
            List<string> Members = new List<string>(); 
            foreach (var member in data)
            {
                if (member.MEMBERSHIPNB != 1) {
                    var temp3 = "";
                    temp3 += "السيد " + member.TRCOMMITTEES_MEMBERS.MEMBERNAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + " / " + member.TRZMEMBERSHIP.NAME;
                    Members.Add(temp3);
                }
                else
                {
                    ViewBag.SessionBossName = "السيد " + member.TRCOMMITTEES_MEMBERS.MEMBERNAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + " / " + member.TRZMEMBERSHIP.NAME;
                }
            }
            ViewBag.SessionMemers = Members;
            //foreach (var member in data)
            //{
            //    if (member.TRCOMMITTEES_MEMBERS.MEMBERSHIPNB != 1)
            //    {
            //        var temp3 = "";
            //        temp3 += "السيد " + member.TRCOMMITTEES_MEMBERS.MEMBERNAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERSHIP.NAME;
            //        Members.Add(temp3);
            //    }
            //    else
            //    {
            //        ViewBag.SessionBossName = "السيد " + member.TRCOMMITTEES_MEMBERS.MEMBERNAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERPOSITION.NAME + " / " + member.TRCOMMITTEES_MEMBERS.TRZMEMBERSHIP.NAME;
            //    }
            //}
            //ViewBag.SessionMemers = Members;


            var sql1 = "SELECT DISTINCT PROCEDNB FROM TRSESSIONS_REPORTS WHERE RYEAR = " + sesdatayear + " and SESSIONNB = " + ses.NB;
            var listofproed = db.Database.SqlQuery<long>(sql1).ToList();

            List<ListPROCEDS_Print_ALL> ListPROCEDS_Print_ALL = new List<ListPROCEDS_Print_ALL>();
            foreach (var item in listofproed)
            {
                ListPROCEDS_Print_ALL ofpro = new ListPROCEDS_Print_ALL();
                ofpro.pronb = item;
                ofpro.proname = db.ZPROCEDTYPS.Where(x=>x.NB == item).Select(x=>x.NAME).FirstOrDefault();
              var sql = "SELECT * FROM TRSESSIONS_REPORTS WHERE RYEAR = " + sesdatayear + " and SESSIONNB = " + ses.NB + "and PROCEDNB = "+ item;
                ofpro.pro = db.Database.SqlQuery<PROCEDS_Print_ALL>(sql).ToList();
                ListPROCEDS_Print_ALL.Add(ofpro);
            }

           

           
            //PROCEDS_Print_ALL ALLPROCEDS = new PROCEDS_Print_ALL();
            //var pr = db.TRSESSIONS_PROCEDS.Where(x=>x.SESSIONNB == ID);            
            //foreach (var item in pr)
            //{
            //    if (item.CARPROCED.PROCEDNB == 2001)
            //    {
            //        var trli = db.Database.SqlQuery<PROCEDS_2001_VM>("SELECT PL.NB ,PL.NAME , ZT.NAME AS TYP , PL.LINEPATH FROM PROCED_LINES PL JOIN ZTRLINETYPES ZT ON PL.TYP = ZT.NB  WHERE CARPROCEDNB = " + item.CARPROCEDNB).FirstOrDefault();
            //        trli.PROCEDNB = 2001;
            //        trli.PROCEDNAME = "احداث خط";
            //        trli.CARPROCEDNB = item.CARPROCEDNB;
            //        trli.LISTCITYS = db.Database.SqlQuery<string>("SELECT ZC.NAME FROM PROCED_LINES_CITY TC JOIN ZCITYS ZC ON TC.CITYNB = ZC.NB WHERE TC.PROCEDLINENB = "+ trli.NB).ToList();
            //        ALLPROCEDS.proces2001.Add(trli)  ;
            //    }
            //}
            //ViewBag.ALLPROCEDS = ALLPROCEDS.proces2001;
            ////var sql = "SELECT PT.NAME AS PROCEDNAME,CP.PROCEDNB,TP.CARPROCEDNB ,CA.TABNU , ZC.NAME AS CATNAME,ZF.NAME FACNAME,CT.ENGINEFEUL,CA.FACTYY ,CT.SITES"
            ////           + " FROM TRSESSIONS_PROCEDS  TP"
            ////           + " LEFT JOIN CARPROCEDS CP ON TP.CARPROCEDNB = CP.NB"
            ////           + " LEFT JOIN CARS CA ON CA.NB = CP.CARNB"
            ////           + " LEFT JOIN ZPROCEDTYPS PT ON CP.PROCEDNB = PT.NB"
            ////           + " LEFT JOIN ZCARCATEGORYS ZC ON CA.CARCATNB = ZC.NB"
            ////           + " LEFT JOIN ZFACCOMPS ZF ON CA.FACTCOMPNB = ZF.NB"
            ////           + " LEFT JOIN CARATTRIBS CT ON CA.NB = CT.CARNB"
            ////           + " ORDER BY CP.PROCEDNB ASC";



            return View(ListPROCEDS_Print_ALL);
        }
        public ActionResult GetProcedinfo(long NB , long PRONB)
        {
            // حالة الغاء خط
            if (PRONB == 2002)
            {
                var pro = db.Database.SqlQuery<long>("select CARPROCEDNB from TRSESSIONS_PROCEDS where CARPROCEDSTEPNB = " + NB).FirstOrDefault();
                var LINENB = db.Database.SqlQuery<long>("select LINENB from PROCED_LINES where CARPROCEDNB = " + pro).FirstOrDefault();
             // var data = db.TRLINES.Find(LINENB);
                ViewBag.LineName = db.Database.SqlQuery<TRLINE>("select * from TRLINES where nb = "+ LINENB).ToList();
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
            else
            {
                return PartialView("_ProcedInfo");
            }                    
        }    
        public ActionResult GetProcedCount()
        {
            var data = db.Database.SqlQuery<TRPROCEDS_AVAILABLEVM>("SELECT PV.NB, PV.PROCEDNB  ,  PV.COUNTAVAILABLE ,PV.COUNTPROCED, ZP.NAME FROM TRPROCEDS_AVAILABLE PV JOIN ZPROCEDTYPS ZP ON ZP.NB =  PV.PROCEDNB").ToList();
           ViewBag.Listdata = data;
            return PartialView("_ProcedCount");
        }
        public ActionResult GetProcedCountIstrue(long Sesnb)
        {
            string sql = "SELECT COUNT(*) FROM TRSESSIONS_PROCEDS WHERE SESSIONNB = "+ Sesnb;

            var data = db.Database.SqlQuery<int>(sql).FirstOrDefault();
            if (data == 0)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Read_TRPROCEDS_AVAILABLE([DataSourceRequest] DataSourceRequest request, long nb)
        {
            var sql = "SELECT PV.NB, PV.PROCEDNB  ,  PV.COUNTAVAILABLE ,PV.COUNTPROCED, ZP.NAME ,PV.SESSIONNB FROM TRPROCEDS_AVAILABLE PV JOIN ZPROCEDTYPS ZP ON ZP.NB =  PV.PROCEDNB where PV.SESSIONNB = "+ nb;
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
        public ActionResult UpdateAll([Bind(Prefix = "models")] IEnumerable<TRPROCEDS_AVAILABLEVM> model)
        {
            if (model != null && ModelState.IsValid)
            {               
                foreach (var item in model)
                {
                    try
                    {
                    var data = db.TRPROCEDS_AVAILABLE.Find(item.NB);
                        data.COUNTPROCED = item.COUNTPROCED;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();                                             
                    }
                    catch (Exception ex)
                    {
                      return Json(new { success = false}, JsonRequestBehavior.AllowGet);
                    }                   
                }
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetProcedCount(long Sesnb)
        {
            try {
               
                long? status = 0;
                string sql = "BEGIN VEHICLES.PASSENGERS_PKG.ADD_SESSION_PROCEDS(:PSESNB,:PSTATUS); END;";
                db.Database.ExecuteSqlCommand(sql, Sesnb, status);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }


        }
    }
}