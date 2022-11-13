using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class App_SettingsController : Controller
    {
        //private IndexesEntities db = new IndexesEntities();
        private ProcedContext db = new ProcedContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult APP_SETTINGS_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<APP_SETTINGS> APP_SETTINGS = db.APP_SETTINGS;
            DataSourceResult result = APP_SETTINGS.ToDataSourceResult(request, setting => new
            {
                NB = setting.NB,
                NAME = setting.NAME,
                ANAME = setting.ANAME,
                VAL = setting.VAL,
            });

            return Json(result);
        }
        public ActionResult Edit(long? id)
        {
            var setting = db.APP_SETTINGS.Find(id);
            if (setting == null)
            {
                return Content("لايوجد ");
            }
            return PartialView(setting);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(APP_SETTINGS model)
        {
            bool success = false;
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var setting = db.APP_SETTINGS.Find(model.NB);
                    setting.VAL = model.VAL;
                    db.Entry(setting).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                        success = true;
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "فشلت عملية التعديل");
                    }
                }
            }
            ViewBag.Success = success;
            return PartialView("_EditPartial", model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APP_SETTINGS_Update([DataSourceRequest] DataSourceRequest request, APP_SETTINGS setting)
        {
            if (ModelState.IsValid)
            {
                var entity = new APP_SETTINGS
                {
                    NB = setting.NB,
                    NAME = setting.NAME,
                    ANAME = setting.ANAME,
                    VAL= setting.VAL,
                };

                db.APP_SETTINGS.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
