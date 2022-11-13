using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError,CanDoIt]
    public class CityManagerController : Controller
    {
        // private IndexesEntities db = new IndexesEntities();
        // private IndexesEntities db = new IndexesEntities();
        private ProcedContext db = new ProcedContext();
        public ActionResult Index()
        {
            ViewBag.Zcitys = new SelectList(db.ZCITYS.OrderBy(x => x.NAME), "NB", "NAME", null);
            return View();
        }
        public ActionResult CityManagers_Read([DataSourceRequest]DataSourceRequest request)
        {
            //IQueryable<ZGROUPSIDE> zgroupsides = db.ZGROUPSIDES.Where(x => x.GNB == Gnb).OrderBy(c => c.ORDR);
            IQueryable<CITY_MANAGER> cityNanagers = db.CITY_MANAGER;
            //cityNanagers = db.CITY_MANAGER.SqlQuery("select * from CITY_MANAGER").AsQueryable();
            DataSourceResult result = cityNanagers.ToDataSourceResult(request, cityManager => new
            {
                cityManager.NB,
                cityManager.MANAGERNAME,
                cityManager.CITYNB,
                ZCITY = new
                {
                    NAME = cityManager.ZCITY != null ? cityManager.ZCITY.NAME: "",
                },
            });
            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            var model = new CITY_MANAGER();
            ViewBag.Zcitys = new SelectList(db.ZCITYS.OrderBy(x => x.NAME), "NB", "NAME", null);
            return PartialView(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(CITY_MANAGER model)
        {
            bool success = false;
            if (model != null)
            {
                if (model.CITYNB == null)
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد المحافظة");
                }
                else
                {
                    if (db.CITY_MANAGER.Any(c => c.CITYNB == model.CITYNB))
                    {
                        ModelState.AddModelError("", "لايمكن الاضافة,يوجد مدير نقل لهذه المحافظة");
                        ModelState.AddModelError("CITYNB", "لايمكن الاضافة,يوجد مدير نقل لهذه المحافظة");
                    }
                }
                if (string.IsNullOrEmpty(model.MANAGERNAME))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الأسم");
                    ModelState.AddModelError("MANAGERNAME", "لايمكن الاضافة,يجب تحديد الأسم");
                }
                else if(db.CITY_MANAGER.Any(c => c.MANAGERNAME == model.MANAGERNAME.Trim()))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,الأسم مكرر");
                    ModelState.AddModelError("MANAGERNAME", "لايمكن الاضافة,الأسم مكرر");
                }
                if (ModelState.IsValid)
                {
                    db.CITY_MANAGER.Add(model);
                    try
                    {
                        db.SaveChanges();
                        success = true;
                    }
                    catch (Exception ee)
                    {
                        ModelState.AddModelError("", "فشلت عملية الإضافة");
                    }
                }
            }
            ViewBag.Zcitys = new SelectList(db.ZCITYS.OrderBy(x => x.NAME), "NB", "NAME", null);
            ViewBag.Success = success;
            return PartialView("_CreatPartial", model);
        }
        public ActionResult Edit(long? id)
        {
            var cityManager = db.CITY_MANAGER.Find(id);
            if (cityManager == null)
            {
                return Content("لا يوجد مدير نقل ");
            }
            ViewBag.Zcitys = new SelectList(db.ZCITYS.OrderBy(x => x.NAME), "NB", "NAME", cityManager.CITYNB);
            return PartialView(cityManager);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(CITY_MANAGER model)
        {
            bool success = false;
            if (model != null)
            {
                if (model.CITYNB == null)
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد المحافظة");
                    ModelState.AddModelError("CITYNB", "لايمكن التعديل,يجب تحديد المحافظة");
                }
                else  if (db.CITY_MANAGER.Any(c => c.CITYNB == model.CITYNB && c.NB != model.NB))
                    {
                    ModelState.AddModelError("", "لايمكن التعديل,يوجد مدير نقل لهذه المحافظة");
                    ModelState.AddModelError("CITYNB", "لايمكن التعديل,يوجد مدير نقل لهذه المحافظة");
                }
                if (string.IsNullOrEmpty(model.MANAGERNAME))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الأسم");
                    ModelState.AddModelError("MANAGERNAME", "لايمكن التعديل,يجب تحديد الأسم");
                }
                else if (db.CITY_MANAGER.Any(c => c.MANAGERNAME == model.MANAGERNAME.Trim() && c.NB != model.NB))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,الأسم مكرر");
                    ModelState.AddModelError("MANAGERNAME", "لايمكن التعديل,الأسم مكرر");
                }
                if (ModelState.IsValid)
                {
                    var cityManager = db.CITY_MANAGER.Find(model.NB);
                    if (model.CITYNB != cityManager.CITYNB)
                    {
                        cityManager.CITYNB = model.CITYNB;
                    }
                    if (model.MANAGERNAME != cityManager.MANAGERNAME.Trim())
                    {
                        cityManager.MANAGERNAME = model.MANAGERNAME.Trim();
                    }
                    db.Entry(cityManager).State = EntityState.Modified;
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
            ViewBag.Zcitys = new SelectList(db.ZCITYS.OrderBy(x => x.NAME), "NB", "NAME", null);
            return PartialView("_EditePartial", model);
        }
        public ActionResult Delete(long? id)
        {
            var cityMananer = db.CITY_MANAGER.Find(id);
            if (cityMananer == null)
            {
                return Content("لا يوجد مدير نقل");
            }
            ViewBag.Name = cityMananer.MANAGERNAME;
            return PartialView(cityMananer);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(CITY_MANAGER model)
        {
            bool success = false;
            var cityManager = db.CITY_MANAGER.Find(model.NB);
            ViewBag.Name = cityManager.MANAGERNAME;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (cityManager != null)
                    {
                        db.CITY_MANAGER.Attach(cityManager);
                        db.CITY_MANAGER.Remove(cityManager);
                        db.SaveChanges();
                        transaction.Commit();
                        success = true;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    success = false;
                }
            }
            ViewBag.Success = success;
            return PartialView("_ZgroupSideDeletePartial", model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZgroupSides_Create([DataSourceRequest]DataSourceRequest request, ZGROUPSIDE zGROUPSIDE)
        {
            if (ModelState.IsValid)
            {
                int TNB = int.Parse(Request["Gnb"]);

                if (ModelState.IsValid)
                {
                    var entity = new ZGROUPSIDE
                    {
                        GNB = TNB,
                        ONB = zGROUPSIDE.ONB,



                    };

                    db.ZGROUPSIDES.Add(entity);
                    db.SaveChanges();
                    zGROUPSIDE.NB = entity.NB;
                }
            }

            return Json(new[] { zGROUPSIDE }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZgroupSides_Update([DataSourceRequest]DataSourceRequest request, ZGROUPSIDE zGROUPSIDE)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZGROUPSIDE
                {
                    NB = zGROUPSIDE.NB,
                    ONB = zGROUPSIDE.ONB,
                };

                db.ZGROUPSIDES.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { zGROUPSIDE }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZgroupSides_Destroy([DataSourceRequest]DataSourceRequest request, ZGROUPSIDE zGROUPSIDE)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZGROUPSIDE
                {
                    NB = zGROUPSIDE.NB,
                    ONB = zGROUPSIDE.ONB,
                    GNB = zGROUPSIDE.GNB,
                };

                db.ZGROUPSIDES.Attach(entity);
                db.ZGROUPSIDES.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zGROUPSIDE }.ToDataSourceResult(request, ModelState));
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
