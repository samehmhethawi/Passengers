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
        private ValidationController validation = new ValidationController();
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

            var sql = "SELECT TA.* FROM TRZPROCEDTYPS_ALLOW TA join ZPROCEDTYPS ZP ON ZP.NB = TA.PROCEDNB JOIN ZPROCEDTYPS ZP2 ON ZP2.NB = TA.CHECK_PROCEDNB WHERE 1 = 1  ";
            var PROCEDNB = Request.Form["SPROCEDNB"].Trim();
            var CHECK_PROCEDNB = Request.Form["SCHECK_PROCEDNB"].Trim();
            if (PROCEDNB != "")
            {
                sql += " and ZP.NAME like '%" + PROCEDNB + "%'";
            }
            if (CHECK_PROCEDNB != "")
            {
                sql += " and ZP2.NAME like '%" + CHECK_PROCEDNB + "%'";
            }
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

        public ActionResult Create(List<int> PROCEDNB , List<int> CHECK_PROCEDNB )
        {
            foreach (var baseitem in PROCEDNB)
            {
                foreach(var secitem in CHECK_PROCEDNB)
                {
                    TRZPROCEDTYPS_ALLOW data = new TRZPROCEDTYPS_ALLOW();
                    data.PROCEDNB = baseitem;
                    data.CHECK_PROCEDNB = secitem;
                    data.STATUS = 1;
                    db.TRZPROCEDTYPS_ALLOW.Add(data);

                }

            }
            
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
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Delete(long NB)
        {

            try
            {

                var dd = db.TRZPROCEDTYPS_ALLOW.Find(NB);

                if (dd == null)
                {
                    return Json(new { success = false, responseText = " لا يوجد سجل" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (dd != null)
                            {

                                db.TRZPROCEDTYPS_ALLOW.Attach(dd);
                                db.TRZPROCEDTYPS_ALLOW.Remove(dd);
                                db.SaveChanges();
                                transaction.Commit();

                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                        }
                    }
                }


                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}