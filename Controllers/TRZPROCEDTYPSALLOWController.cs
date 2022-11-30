using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Passengers.ViewModel;
using System.Data.Entity;
using Proced.DataAccess.Models.CF;
namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class TRZPROCEDTYPSALLOWController : Controller
    {
        private ProcedContext db = new ProcedContext();
        // GET: TRZPROCEDTYPSALLOW
        public ActionResult Index()
        {
            ViewData["STATUS"] = db.TRSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["PROCEDS"] = db.ZPROCEDTYPS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "SELECT * FROM TRZPROCEDTYPS_ALLOW WHERE 1 = 1 ";
            var data = db.Database.SqlQuery<TRZPROCEDTYPS_ALLOW>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                PROCEDNB = commm.PROCEDNB,
                CHECK_PROCEDNB = commm.CHECK_PROCEDNB,
                STATUS = commm.STATUS,


                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
        }

        public ActionResult Create(TRZPROCEDTYPS_ALLOW model)
        {
            db.TRZPROCEDTYPS_ALLOW.Add(model);
            db.SaveChanges();
            return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(TRZPROCEDTYPS_ALLOW model)
        {
            try
            {

                var dd = db.TRZPROCEDTYPS_ALLOW.Find(model.NB);

                if (dd.CHECK_PROCEDNB != model.CHECK_PROCEDNB)
                {
                    dd.CHECK_PROCEDNB = model.CHECK_PROCEDNB;
                }
                if (dd.STATUS != model.STATUS)
                {
                    dd.STATUS = model.STATUS;
                }

                if (dd.PROCEDNB != model.PROCEDNB)
                {
                    dd.PROCEDNB = model.PROCEDNB;
                }
                db.Entry(dd).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, responseText = ex }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}