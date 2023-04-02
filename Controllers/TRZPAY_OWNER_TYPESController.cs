using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Passengers.Controllers
{
    public class TRZPAY_OWNER_TYPESController : Controller
    {
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        // GET: TRZPAY_OWNER_TYPES
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "SELECT * FROM TRZPAY_OWNER_TYPES WHERE 1 = 1 ";
            var data = db.Database.SqlQuery<TRZPAY_OWNER_TYPES>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                NAME = commm.NAME,
                MAXAMOUNT = commm.MAXAMOUNT,


                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
        }

        public ActionResult Create(TRZPAY_OWNER_TYPES model)
        {
            db.TRZPAY_OWNER_TYPES.Add(model);
            db.SaveChanges();
            return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(TRZPAY_OWNER_TYPES model)
        {
            try
            {

                var dd = db.TRZPAY_OWNER_TYPES.Find(model.NB);

                if (dd.NAME != model.NAME)
                {
                    dd.NAME = model.NAME;
                }
                if (dd.MAXAMOUNT != model.MAXAMOUNT)
                {
                    dd.MAXAMOUNT = model.MAXAMOUNT;
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
                var dd = db.TRZPAY_OWNER_TYPES.Find(NB);

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
                                db.TRZPAY_OWNER_TYPES.Attach(dd);
                                db.TRZPAY_OWNER_TYPES.Remove(dd);
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