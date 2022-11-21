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


        public ActionResult Read_Member_Session([DataSourceRequest] DataSourceRequest request , long NB)
        {
            var sql = "SELECT TSM.NB ,TSM.SESSIONNB , TSM.MEMBERNB , TSM.ISPRESENT ,TM.MEMBERNAME ,TM.MEMBERSHIPNB , TM.MEMBERPOSITIONNB ,TS.STATUS AS SESSIONSTATUS FROM TRSESSIONS_MEMBERS_PRESENT  TSM JOIN TRCOMMITTEES_MEMBERS TM ON TM.NB = TSM.MEMBERNB JOIN TRSESSIONS TS ON TS.NB = TSM.SESSIONNB  WHERE TSM.SESSIONNB  =  " + NB; 

            var data = db.Database.SqlQuery<TRSESSIONS_MEMBERS_PRESENTVM>(sql);

            return Json(data.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
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

        public ActionResult TRSESSIONSPROCEDS_Read([DataSourceRequest] DataSourceRequest request ,long nb)
        {

            string sql = "SELECT TSP.NB AS NB ,ZP.NAME  AS PROCEDNAME, TSP.SESSIONNB  AS SESSIONNB, TSP.CARPROCEDNB AS CARPROCEDNB, CP.RECDAT  AS RECDAT, TSP.PSTATUS   AS PSTATUS, CP.CARNB  AS CARNB, TSP.ORDR AS ORDR, TSP.CARPROCEDSTEPNB  AS CARPROCEDSTEPNB  FROM TRSESSIONS_PROCEDS  TSP JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB JOIN ZPROCEDTYPS ZP ON ZP.NB = CP.PROCEDNB where TSP.SESSIONNB = " + nb + " ORDER BY TSP.ORDR ASC";
            var data = db.Database.SqlQuery<TRSESSIONS_PROCEDS_VM>(sql).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCount555(long ID)
        {

            string sql = "SELECT COUNT (*) AS TOTALCOUNT, NVL (SUM (CASE WHEN PSTATUS = 1 THEN 1 ELSE 0 END),0) AS AGREE, NVL (SUM (CASE WHEN PSTATUS = 2 THEN 1 ELSE 0 END),0) AS NOTAGREE, NVL (SUM (CASE WHEN PSTATUS = 3 THEN 1 ELSE 0 END),0) AS DELAYED, NVL (SUM (CASE WHEN PSTATUS = 4 THEN 1 ELSE 0 END),0) AS UNANSWERED  FROM TRSESSIONS_PROCEDS WHERE SESSIONNB =" + ID;

            var data = db.Database.SqlQuery<CountTotal>(sql).FirstOrDefault();

            return Json(new { success = true, TOTALCOUNT = data.TOTALCOUNT , AGREE = data.AGREE , NOTAGREE = data.NOTAGREE , DELAYED = data.DELAYED , UNANSWERED = data.UNANSWERED }, JsonRequestBehavior.AllowGet);
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

        public ActionResult FinishSession(long ID)
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
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
        }

   
    }
}