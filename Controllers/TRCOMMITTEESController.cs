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
    public class TRCOMMITTEESController : Controller
    {
        private ProcedContext db = new ProcedContext();
        // GET: TRCOMMITTEES
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

            return View();
        }


        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "select * from TRCOMMITTEES where 1 = 1 ";
            var COMNO = Request.Form["COMNO"].Trim();
            var COMDATESTART = Request.Form["COMDATESTART"].Trim();
            var COMDATEEND = Request.Form["COMDATEEND"].Trim();
            var STATUS = Request.Form["STATUS"].Trim();
            var COMCITYNB = Request.Form["COMCITYNB"].Trim();




            if (COMNO != "")
            {
                sql += " and COMNO like '%" + COMNO + "%'";
            }
            if (STATUS != "")
            {
                sql += " and STATUS =" + STATUS ;
            }

            if (COMDATESTART != "")
            {
                sql += " and COMDATE >= TO_DATE('"+ COMDATESTART+"','DD/MM/YYYY') ";
            }

            if (COMDATEEND != "")
            {
                sql += " and COMDATE <= TO_DATE('" + COMDATEEND + "','DD/MM/YYYY') ";
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and COMCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and COMCITYNB =" + ci;
            }


            var data = db.Database.SqlQuery<TRCOMMITTEES>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                COMNO = commm.COMNO,
                COMDATE = commm.COMDATE,
                COMCITYNB = commm.COMCITYNB,
                STATUS = commm.STATUS,
                ORDR = commm.ORDR,
                IUSER = commm.IUSER,
                IDATE = commm.IDATE,
                Seq = (request.Page -1) * request.PageSize + (++index)
            });
            return Json(result);
           
        }
  
    
        public ActionResult Create(TRCOMMITTEES model)
        {
            try
            {
                db.TRCOMMITTEES.Add(model);
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText =  ex.ToString() });
            }
           
        }

        public ActionResult Update(TRCOMMITTEES model)
        {
            try
            {
                var dd = db.TRCOMMITTEES.Find(model.NB);
                if(dd.COMNO != model.COMNO)
                {
                    dd.COMNO = model.COMNO;
                }
                if (dd.COMDATE != model.COMDATE)
                {
                    dd.COMDATE = model.COMDATE;
                }
                if (dd.COMCITYNB != model.COMCITYNB)
                {
                    dd.COMCITYNB = model.COMCITYNB;
                }
                db.Entry(dd).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() });
            }

        }
    }

   
}