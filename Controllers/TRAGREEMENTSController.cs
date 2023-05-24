using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;

namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class TRAGREEMENTSController : Controller
    {
        // GET: TRAGREEMENTS

        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        public ActionResult Index()
        {
            ViewData["AGREEMENTTYPENB"] = db.AGREEMENTTYPES.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["zregs"] = db.ZREGS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["STATUS"]  = db.TRAGREEMENTS_STATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "select * from TRAGREEMENTS where 1 = 1 ";
            var SCARNB = Request.Form["SCARNB"].Trim();
            var STABNB = Request.Form["STABNB"].Trim();
            var SCITYNB = Request.Form["SCITYNB"].Trim();
            var SREGNB = Request.Form["SREGNB"].Trim();
            var SAGRNB = Request.Form["SAGRNB"].Trim();
            var SAGRDATEStart = Request.Form["SAGRDATEStart"].Trim();
            var SAGRDATEEnd = Request.Form["SAGRDATEEnd"].Trim();
            var SCARPROCEDNB = Request.Form["SCARPROCEDNB"].Trim();
            var SSESNO = Request.Form["SSESNO"].Trim();
            var SSESDATEStart = Request.Form["SSESDATEStart"].Trim();
            var SSESDATEEnd = Request.Form["SSESDATEEnd"].Trim();
            var SSESCITYNB = Request.Form["SSESCITYNB"].Trim();


            if (SCARNB != "")
            {
                sql += " and CARNB = " + SCARNB;
            }
            if (STABNB != "")
            {
                sql += " and TABNB LIKE '%" + STABNB + "%'";
            }
            if (SCITYNB != "")
            {
                sql += " and CITYNB = " + SCITYNB;
            }
            if (SREGNB != "")
            {
                sql += " and CARREGNB = " + SREGNB;
            }
            if (SAGRNB != "")
            {
                sql += " and AGREEMENTTYPENB = " + SAGRNB;
            }
            if (SAGRDATEStart != "")
            {
                sql += " and TRUNC(STARTDATE) >= TO_DATE('" + SAGRDATEStart + "','DD/MM/YYYY') ";
            }
            if (SAGRDATEEnd != "")
            {
                sql += " and TRUNC(ENDDATE) <= TO_DATE('" + SAGRDATEEnd + "','DD/MM/YYYY') ";
            }
            if (SCARPROCEDNB != "")
            {
                sql += " and CARPROCEDNB = " + SCARPROCEDNB;
            }
            if (SSESNO != "")
            {
                sql += " and SESNO LIKE '%" + SSESNO + "%'";
            }
            if (SSESDATEStart != "")
            {
                sql += " and TRUNC(SES_DATE) >= TO_DATE('" + SSESDATEStart + "','DD/MM/YYYY') ";
            }
            if (SSESDATEEnd != "")
            {
                sql += " and TRUNC(SES_DATE) <= TO_DATE('" + SSESDATEEnd + "','DD/MM/YYYY') ";
            }
         

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SSESCITYNB != "")
                {
                    sql += " and SESCITY =" + SSESCITYNB;
                }

            }
            else
            {
                sql += " and SESCITY =" + ci;
            }

            sql += " order by nb desc";
            var data = db.Database.SqlQuery<TRAGREEMENTS>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                CARNB = commm.CARNB,
                TABNB = commm.TABNB,
                CITYNB = commm.CITYNB,
                CARREGNB = commm.CARREGNB,
                CARPROCEDSTEPNB = commm.CARPROCEDSTEPNB,
                STARTDATE = commm.STARTDATE,
                ENDDATE = commm.ENDDATE,
                AGREEMENTTYPENB = commm.AGREEMENTTYPENB,
                SESNB = commm.SESNB,
                SESNO = commm.SESNO,
                SES_DATE = commm.SES_DATE,
                NOTES = commm.NOTES,
                AGREENO = commm.AGREENO,
                AGREEDATE = commm.AGREEDATE,
                CARPROCEDNB = commm.CARPROCEDNB,
                SESCITY = commm.SESCITY,

                CARNB2 = commm.CARNB2,

                ASTATUS = commm.ASTATUS,
                EDITCAUSES = commm.EDITCAUSES,
                EDITDATE = commm.EDITDATE,
                EDITUSER = commm.EDITUSER,

                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);

        }

        public ActionResult PrintNotification( long OUTACTSNB, long pCarprocedNB)
        {
            ViewBag.outact = db.ZOUTACTS.Find(OUTACTSNB).NAME;
           
            ViewBag.CarProcedNb = pCarprocedNB;
            var pro = db.CARPROCEDS.Find(pCarprocedNB);
            if (pro.CARNB != null)
            {
                ViewBag.CarNb = pro.CARNB;
            }

            if (pro.PROCEDNB == 2000)
            {
                ViewBag.agrtyp = 2000;           
                var PROCED = db.CARPROCEDS.Find(pCarprocedNB);
                var pro_lin = db.PROCED_LINES.Where(x => x.CARPROCEDNB == pCarprocedNB).FirstOrDefault();
                var sqll = "select * from TRSESSIONS_PROCEDS where CARPROCEDNB = " + pCarprocedNB;
                var ses_pro = db.Database.SqlQuery<TRSESSIONS_PROCEDS>(sqll).FirstOrDefault();
                var ses_info = db.TRSESSIONS.Find(ses_pro.SESSIONNB);
                ViewBag.SESCITY = db.ZCITYS.Find(PROCED.CITYNB).NAME;
                ViewBag.agrname = db.ZPROCEDTYPS.Find(PROCED.PROCEDNB).NAME;
                ViewBag.ses_pro = ses_pro;
                ViewBag.ses_info = ses_info;
                var sql2 = "SELECT MM.*"
                      + " FROM TRCOMMITTEES_MEMBERS  MM "
                      + " JOIN TRSESSIONS_MEMBERS_PRESENT TM ON MM.NB = TM.MEMBERNB WHERE TM.SESSIONNB = " + ses_pro.SESSIONNB + " AND MM.MEMBERSHIPNB IN (1,2,3)";
                var Members = db.Database.SqlQuery<TRCOMMITTEES_MEMBERS>(sql2).ToList();
                ViewBag.Member1 = Members.Where(x => x.MEMBERSHIPNB == 1).Select(y => y.MEMBERNAME).FirstOrDefault();
                ViewBag.Member2 = Members.Where(x => x.MEMBERSHIPNB == 2).Select(y => y.MEMBERNAME).FirstOrDefault();
                ViewBag.Member3 = Members.Where(x => x.MEMBERSHIPNB == 3).Select(y => y.MEMBERNAME).FirstOrDefault();
                ViewBag.PROCED_LINES = pro_lin;
                return View();
            }

            if (pro.PROCEDNB == 2006 || pro.PROCEDNB == 2016 || pro.PROCEDNB == 2018 || pro.PROCEDNB == 2023 || pro.PROCEDNB == 2022 || pro.PROCEDNB == 2008 || pro.PROCEDNB == 2012 || pro.PROCEDNB == 2014)
            {          
                var TRAGR = db.TRAGREEMENTS.Where(x=>x.CARPROCEDNB == pCarprocedNB).FirstOrDefault();
                ViewBag.agrtyp = TRAGR.AGREEMENTTYPENB;
                ViewBag.agrname = db.AGREEMENTTYPES.Where(x => x.NB == TRAGR.AGREEMENTTYPENB).Select(s => s.NAME).FirstOrDefault();
                ViewBag.TRAGR = TRAGR;
                if (TRAGR.SESCITY != null && TRAGR.SESNB != null)
                {
                    ViewBag.SESCITY = db.ZCITYS.Find(TRAGR.SESCITY).NAME;

                    var sql2 = "SELECT MM.*"
                        + " FROM TRCOMMITTEES_MEMBERS  MM "
                        + " JOIN TRSESSIONS_MEMBERS_PRESENT TM ON MM.NB = TM.MEMBERNB WHERE TM.SESSIONNB = " + TRAGR.SESNB + " AND MM.MEMBERSHIPNB IN (1,2,3)";

                    var Members = db.Database.SqlQuery<TRCOMMITTEES_MEMBERS>(sql2).ToList();
                    ViewBag.Member1 = Members.Where(x => x.MEMBERSHIPNB == 1).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member2 = Members.Where(x => x.MEMBERSHIPNB == 2).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member3 = Members.Where(x => x.MEMBERSHIPNB == 3).Select(y => y.MEMBERNAME).FirstOrDefault();
                }
                else
                {                  
                    var sql2 = "SELECT MM.*"
                         + " FROM TRCOMMITTEES_MEMBERS  MM "
                         + " JOIN TRCOMMITTEES TM ON TM.NB = MM.COMMITTEENB WHERE TM.STATUS = 1 and TM.COMCITYNB = " + pro.CITYNB + " AND MM.MEMBERSHIPNB IN (1,2,3)";

                    var Members = db.Database.SqlQuery<TRCOMMITTEES_MEMBERS>(sql2).ToList();
                    ViewBag.Member1 = Members.Where(x => x.MEMBERSHIPNB == 1).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member2 = Members.Where(x => x.MEMBERSHIPNB == 2).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member3 = Members.Where(x => x.MEMBERSHIPNB == 3).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.SESCITY = db.ZCITYS.Find(pro.CITYNB).NAME;
                }
             
                var sql = " SELECT ow.name || ' ' || ow.lastname || ' بن ' || ow.father || ' الوالدة ' || ow.mother as FULLNAME "
                   + " FROM CARPROCEDS cp JOIN carowners ow ON ow.nb = cp.OWNERNB "
                   + "  WHERE cp.nb = " + TRAGR.CARPROCEDNB;

                ViewBag.fullname = db.Database.SqlQuery<string>(sql).FirstOrDefault();
                var linnb = db.CARS.Where(x => x.NB == TRAGR.CARNB).Select(s => s.LIN).FirstOrDefault();
                ViewBag.linename = db.TRLINES.Where(x => x.NB == linnb).Select(x => x.NAME).FirstOrDefault();

                if (pro.PROCEDNB == 2006 || pro.PROCEDNB == 2016 || pro.PROCEDNB == 2018 || pro.PROCEDNB == 2023 || pro.PROCEDNB == 2022 )
                {
                    var pro_lin = db.PROCED_LINES.Where(x => x.CARPROCEDNB == TRAGR.CARPROCEDNB).FirstOrDefault();
                    ViewBag.PROCED_LINES = pro_lin;
                    ViewBag.NEWlinename = db.TRLINES.Where(x => x.NB == pro_lin.LINENB).Select(x => x.NAME).FirstOrDefault();
                }
                return View();
            }

            if (pro.PROCEDNB == 2020)
            { 
                var TRAGR = db.TRAGREEMENTS.Where(x=>x.CARPROCEDNB == pCarprocedNB).FirstOrDefault();
                ViewBag.agrtyp = TRAGR.AGREEMENTTYPENB;
                ViewBag.agrname = db.AGREEMENTTYPES.Where(x => x.NB == TRAGR.AGREEMENTTYPENB).Select(s => s.NAME).FirstOrDefault();
                ViewBag.TRAGR = TRAGR;
                if (TRAGR.SESCITY != null && TRAGR.SESNB != null)
                {
                    ViewBag.SESCITY = db.ZCITYS.Find(TRAGR.SESCITY).NAME;
                    var sql2 = "SELECT MM.*"
                        + " FROM TRCOMMITTEES_MEMBERS  MM "
                        + " JOIN TRSESSIONS_MEMBERS_PRESENT TM ON MM.NB = TM.MEMBERNB WHERE TM.SESSIONNB = " + TRAGR.SESNB + " AND MM.MEMBERSHIPNB IN (1,2,3)";
                    var Members = db.Database.SqlQuery<TRCOMMITTEES_MEMBERS>(sql2).ToList();
                    ViewBag.Member1 = Members.Where(x => x.MEMBERSHIPNB == 1).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member2 = Members.Where(x => x.MEMBERSHIPNB == 2).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member3 = Members.Where(x => x.MEMBERSHIPNB == 3).Select(y => y.MEMBERNAME).FirstOrDefault();
                }
                else
                {
                    var sql2 = "SELECT MM.*"
                         + " FROM TRCOMMITTEES_MEMBERS  MM "
                         + " JOIN TRCOMMITTEES TM ON TM.NB = MM.COMMITTEENB WHERE TM.STATUS = 1 and TM.COMCITYNB = " + pro.CITYNB + " AND MM.MEMBERSHIPNB IN (1,2,3)";
                    var Members = db.Database.SqlQuery<TRCOMMITTEES_MEMBERS>(sql2).ToList();
                    ViewBag.Member1 = Members.Where(x => x.MEMBERSHIPNB == 1).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member2 = Members.Where(x => x.MEMBERSHIPNB == 2).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.Member3 = Members.Where(x => x.MEMBERSHIPNB == 3).Select(y => y.MEMBERNAME).FirstOrDefault();
                    ViewBag.SESCITY = db.ZCITYS.Find(pro.CITYNB).NAME;
                }               
                var sql = " SELECT ow.name || ' ' || ow.lastname || ' بن ' || ow.father || ' الوالدة ' || ow.mother as FULLNAME "
                   + " FROM CARPROCEDS cp JOIN carowners ow ON ow.nb = cp.OWNERNB "
                   + "  WHERE cp.nb = " + TRAGR.CARPROCEDNB;

                ViewBag.fullname = db.Database.SqlQuery<string>(sql).FirstOrDefault();

                var linnb = db.CARS.Where(x => x.NB == TRAGR.CARNB).Select(s => s.LIN).FirstOrDefault();
                ViewBag.linename = db.TRLINES.Where(x => x.NB == linnb).Select(x => x.NAME).FirstOrDefault();               
                var linnb2 = db.CARS.Find(TRAGR.CARNB2);
                ViewBag.linnb2 = linnb2;
                ViewBag.linename2 = db.TRLINES.Where(x => x.NB == linnb2.LIN).Select(x => x.NAME).FirstOrDefault();
                var pro_lin = db.PROCED_LINES.Where(x => x.CARPROCEDNB == TRAGR.CARPROCEDNB).FirstOrDefault();
                    ViewBag.PROCED_LINES = pro_lin;               
                return View();
            }
            return View();
        }
    
        public ActionResult EditAgrStatus(long Agrnb , int AgrStatus,string EDITCAUSES) 
        {
            try
            {
                var data = db.TRAGREEMENTS.Find(Agrnb);
                if (data == null)
                {
                    return Json(new { success = false, responseText = "الموافقة غير موجودة" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (data.ASTATUS == 2)
                    {
                        return Json(new { success = false, responseText = "لا يمكن تعديل الموافقة لانها منتهية" }, JsonRequestBehavior.AllowGet);
                    }
                    if(data.ASTATUS != AgrStatus)
                    {
                        data.ASTATUS = AgrStatus;
                        data.EDITCAUSES = EDITCAUSES;
                        data.EDITUSER= Utility.MyName();
                        data.EDITDATE = DateTime.Now;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}