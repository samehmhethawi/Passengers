﻿using System;
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
    public class TRZMEMBERPOSITIONController : Controller
    {
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        // GET: TRZMEMBERPOSITION
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "SELECT * FROM TRZMEMBERPOSITION WHERE 1 = 1 ";
            var data = db.Database.SqlQuery<TRZMEMBERPOSITION>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                NAME = commm.NAME,
                ORDR = commm.ORDR,
                NOTES = commm.NOTES,


                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
        }

        public ActionResult Create(TRZMEMBERPOSITION model)
        {
            db.TRZMEMBERPOSITION.Add(model);
            db.SaveChanges();
            return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(TRZMEMBERPOSITION model)
        {
            try
            {

                var dd = db.TRZMEMBERPOSITION.Find(model.NB);

                if (dd.NAME != model.NAME)
                {
                    dd.NAME = model.NAME;
                }
                if (dd.NOTES != model.NOTES)
                {
                    dd.NOTES = model.NOTES;
                }

                if (dd.ORDR != model.ORDR)
                {
                    dd.ORDR = model.ORDR;
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
                var dd = db.TRZMEMBERPOSITION.Find(NB);

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
                                db.TRZMEMBERPOSITION.Attach(dd);
                                db.TRZMEMBERPOSITION.Remove(dd);
                                db.SaveChanges();
                                transaction.Commit();
                            }
                        }
                        catch (Exception e)
                        {
                          var SS= validation.OracleExceptionValidation(e);
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