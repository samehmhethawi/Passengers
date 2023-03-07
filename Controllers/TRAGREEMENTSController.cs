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
                sql += " and STARTDATE >= TO_DATE('" + SAGRDATEStart + "','DD/MM/YYYY') ";
            }
            if (SAGRDATEEnd != "")
            {
                sql += " and ENDDATE <= TO_DATE('" + SAGRDATEEnd + "','DD/MM/YYYY') ";
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
                sql += " and SES_DATE >= TO_DATE('" + SSESDATEStart + "','DD/MM/YYYY') ";
            }
            if (SSESDATEEnd != "")
            {
                sql += " and SES_DATE <= TO_DATE('" + SSESDATEEnd + "','DD/MM/YYYY') ";
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

           
                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);

        }
    
        public ActionResult PrintNotification(long agrnb , long OUTACTSNB ) 
        {

            var TRAGR = db.TRAGREEMENTS.Find(agrnb);
            var outact = db.ZOUTACTS.Find(OUTACTSNB);
            ViewBag.agrtyp = TRAGR.AGREEMENTTYPENB;
            ViewBag.agrname = db.AGREEMENTTYPES.Where(x => x.NB == TRAGR.AGREEMENTTYPENB).Select(s => s.NAME).FirstOrDefault();
            ViewBag.outact = outact.NAME;
            ViewBag.TRAGR = TRAGR;
            var linnb = db.CARS.Where(x => x.NB == TRAGR.CARNB).Select(s => s.LIN).FirstOrDefault();
            ViewBag.linename = db.TRLINES.Where(x => x.NB == linnb).Select(x => x.NAME).FirstOrDefault();
            return View();
        }
    }
}