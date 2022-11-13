using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
//using Indexes.DataAccess.Models.DataAccess.DAL;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace Passengers.Controllers
{

        [checksession, Authorize, RedirectOnError,CanDoIt]
    public class ZAttribCategoriesController : Controller
    {
        // private IndexesEntities db = new IndexesEntities();
        private ProcedContext db = new ProcedContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ZATTRIBCATEGORYS_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ZATTRIBCATEGORY> zattribcategorys = db.ZATTRIBCATEGORYS;
            DataSourceResult result = zattribcategorys.ToDataSourceResult(request, zATTRIBCATEGORY => new
            {
                NB = zATTRIBCATEGORY.NB,
                NAME = zATTRIBCATEGORY.NAME,
                ORDR = zATTRIBCATEGORY.ORDR,
            });

            return Json(result);
        }
        public ActionResult Create()
        {
            var model = new ZATTRIBCATEGORY();
            if (db.ZATTRIBCATEGORYS.Any())
            {
                model.ORDR = db.ZATTRIBCATEGORYS.Max(m => m.ORDR) + 1;
            }
            else
            {
                model.ORDR = 1;
            }
            return PartialView(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(ZATTRIBCATEGORY model)
        {
            bool success = false;
            if (model != null)
            {
                if (string.IsNullOrWhiteSpace(model.NAME))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الاسم");
                }
                else
                {
                    if (db.ZATTRIBCATEGORYS.Any(c => c.NAME == model.NAME))
                    {
                        ModelState.AddModelError("", "لايمكن الاضافة,الاسم مكرر");
                    }
                }
                if (model.ORDR == null || model.ORDR < 0)
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الترتيب");
                }
                if (db.ZATTRIBCATEGORYS.Any(m => m.ORDR == model.ORDR))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,قيمة الترتيب مكررة");
                }
                if (ModelState.IsValid)
                {
                    db.ZATTRIBCATEGORYS.Add(model);
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
            ViewBag.Success = success;
            return PartialView("_CreatPartial", model);
        }
        public ActionResult Edit(long? id)
        {
            var zAttribCategory = db.ZATTRIBCATEGORYS.Find(id);
            if (zAttribCategory == null)
            {
                return Content("لا يوجد هذا الفئة ");
            }
            return PartialView(zAttribCategory);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(ZATTRIBCATEGORY model)
        {
            bool success = false;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.NAME))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الأسم");
                    ModelState.AddModelError("NAME", "لايمكن التعديل,يجب تحديد الأسم");
                }
                else if (model.ORDR == null)
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الترتيب");
                    ModelState.AddModelError("ORDR", "لايمكن التعديل,يجب تحديد الترتيب");
                }
                else
                {
                    if (db.ZCARKINDS.Any(c => c.NAME == model.NAME && c.NB != model.NB))
                    {
                        ModelState.AddModelError("", "لايمكن التعديل,الاسم مكرر");
                    }
                    if (db.ZCARKINDS.Any(c => c.ORDR == model.ORDR && c.NB != model.NB))
                    {
                        ModelState.AddModelError("", "لايمكن التعديل,الترتيب مكرر");
                    }
                }
                if (ModelState.IsValid)
                {
                    var zAttribCategory = db.ZATTRIBCATEGORYS.Find(model.NB);
                    if (model.NAME != zAttribCategory.NAME)
                    {
                        zAttribCategory.NAME = model.NAME;
                    }
                    if (model.ORDR != zAttribCategory.ORDR)
                    {
                        zAttribCategory.ORDR = model.ORDR;
                    }
                    db.Entry(zAttribCategory).State = EntityState.Modified;
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
            return PartialView("_EditePartial", model);
        }
        public ActionResult Delete(long? id)
        {
            var zAttribCategory = db.ZATTRIBCATEGORYS.Find(id);
            if (zAttribCategory == null)
            {
                return Content("لا يوجد هذا الفئة");
            }
            ViewBag.Name = zAttribCategory?.NAME;
            return PartialView(zAttribCategory);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(ZATTRIBCATEGORY model)
        {
            bool success = false;
            var zAttribCategory = db.ZATTRIBCATEGORYS.Find(model.NB);
            ViewBag.Name = zAttribCategory?.NAME;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (zAttribCategory != null)
                    {
                        db.ZATTRIBCATEGORYS.Attach(zAttribCategory);
                        db.ZATTRIBCATEGORYS.Remove(zAttribCategory);
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
            return PartialView("_ZcarKindDeletePartial", model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBCATEGORYS_Create([DataSourceRequest]DataSourceRequest request, ZATTRIBCATEGORY zATTRIBCATEGORY)
        {
            using (var dbx = new ProcedContext())
            {
                List<ZATTRIBCATEGORY> tccm = dbx.ZATTRIBCATEGORYS.ToList();
                foreach (var m in tccm)
                {
                    if (m.NAME == zATTRIBCATEGORY.NAME && m.NB != zATTRIBCATEGORY.NB)
                    {
                        return this.Json(new DataSourceResult
                        {
                            Errors = "لا يمكنك الإضافة،الاسم مكرر"
                        });
                    }
                }
            }

            int? ord = 0;
            if (!db.ZATTRIBCATEGORYS.Any())
            { ord = 0; }
            else { ord = db.ZATTRIBCATEGORYS.Max(p => p.ORDR); }

            if (ModelState.IsValid)
            {
                var entity = new ZATTRIBCATEGORY
                {
                    NAME = zATTRIBCATEGORY.NAME,
                    ORDR = ord + 1,

                };

                db.ZATTRIBCATEGORYS.Add(entity);
                db.SaveChanges();
                zATTRIBCATEGORY.NB = entity.NB;
            }

            return Json(new[] { zATTRIBCATEGORY }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBCATEGORYS_Update([DataSourceRequest]DataSourceRequest request, ZATTRIBCATEGORY zATTRIBCATEGORY)
        {
            if (ModelState.IsValid)
            {

                using (var dbx = new ProcedContext())
                {
                    List<ZATTRIBCATEGORY> tccm = dbx.ZATTRIBCATEGORYS.ToList();
                    foreach (var m in tccm)
                    {
                        if (m.NAME == zATTRIBCATEGORY.NAME && m.NB != zATTRIBCATEGORY.NB)
                        {
                            return this.Json(new DataSourceResult
                            {
                                Errors = "لا يمكنك التعديل،الاسم  مكرر"
                            });
                        }
                    }
                }
                var entity = new ZATTRIBCATEGORY
                {
                    NB = zATTRIBCATEGORY.NB,
                    NAME = zATTRIBCATEGORY.NAME,
                    ORDR = zATTRIBCATEGORY.ORDR,
                };

                db.ZATTRIBCATEGORYS.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { zATTRIBCATEGORY }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBCATEGORYS_Destroy([DataSourceRequest]DataSourceRequest request, ZATTRIBCATEGORY zATTRIBCATEGORY)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZATTRIBCATEGORY
                {
                    NB = zATTRIBCATEGORY.NB,
                    NAME = zATTRIBCATEGORY.NAME,
                    ORDR = zATTRIBCATEGORY.ORDR,
                };

                db.ZATTRIBCATEGORYS.Attach(entity);
                db.ZATTRIBCATEGORYS.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zATTRIBCATEGORY }.ToDataSourceResult(request, ModelState));
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
