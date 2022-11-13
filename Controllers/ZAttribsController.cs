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
        [checksession, Authorize, RedirectOnError,CanDoIt]
    public class ZAttribsController : Controller
    {
        //private IndexesEntities db = new IndexesEntities();
        private ProcedContext db = new ProcedContext();

        public ActionResult Index()
        {
            var zAttribCategories = db.ZATTRIBCATEGORYS.OrderBy(x => x.ORDR);
            ViewBag.zAttribCategories = zAttribCategories.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NB + " - " + pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            ViewBag.Typs = new List<SelectListItem>()
                 {
                 new SelectListItem{Text="منطقي", Value="منطقي",Selected=true},
                  new SelectListItem{Text="محرف", Value="محرف",Selected=true},
                 new SelectListItem{Text="رقم", Value="رقم",Selected=true},
                 new SelectListItem{Text="اختر نوع المواصفة", Value="",Selected=true}
                 };
            return View();
        }
        private void initControls(ZATTRIB model)
        {
            var zAttribCategories = db.ZATTRIBCATEGORYS.OrderBy(x => x.ORDR);
            ViewBag.zAttribCategories = zAttribCategories.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NB + " - " + pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            ViewBag.Typs = new List<SelectListItem>()
                 {
                 new SelectListItem{Text="رقم", Value="رقم",Selected=true},
                     new SelectListItem{Text="محرف", Value="محرف",Selected=true},
                     new SelectListItem{Text="منطقي", Value="منطقي",Selected=true}
                 };
        }
        public ActionResult Create()
        {
            var model = new ZATTRIB();
            if (db.ZATTRIBS.Any())
            {
                model.ORDR = db.ZATTRIBS.Max(m => m.ORDR) + 1;
            }
            else
            {
                model.ORDR = 1;
            }
            initControls(model);
            return PartialView(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(ZATTRIB model)
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
                    var Zattrib = db.ZATTRIBS.Where(m => m.NAME == model.NAME).FirstOrDefault();
                    if (Zattrib != null)
                    {
                        ModelState.AddModelError("", "لايمكن الاضافة,الاسم مكرر");
                    }
                }
                if (model.ORDR == null || model.ORDR < 0)
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الترتيب");
                }
                if (db.ZATTRIBS.Any(m => m.ORDR == model.ORDR))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,قيمة الترتيب مكررة");
                }
                if (ModelState.IsValid)
                {
                    db.ZATTRIBS.Add(model);
                    try
                    {
                        db.SaveChanges();
                        success = true;
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "فشلت عملية الإضافة");
                    }
                }
            }
            ViewBag.Success = success;
            initControls(model);
            return PartialView("_CreatPartial", model);
        }
        public ActionResult Edit(long? id)
        {
            var zAttrib = db.ZATTRIBS.Find(id);
            if (zAttrib == null)
            {
                return Content("لايوجد ");
            }
            initControls(zAttrib);
            return PartialView(zAttrib);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(ZATTRIB model)
        {
            bool success = false;
            if (model != null)
            {
                if (string.IsNullOrWhiteSpace(model.NAME))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الاسم");
                }
                else
                {
                    var Zattrib = db.ZATTRIBS.Where(m => m.NAME == model.NAME && m.NB != model.NB).FirstOrDefault();
                    if (Zattrib != null)
                    {
                        ModelState.AddModelError("", "لايمكن التعديل,الاسم مكرر");
                    }
                }
                if (model.ORDR == null || model.ORDR < 0)
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الترتيب");
                }
                //if (db.ZATTRIBS.Any(m => m.ORDR == model.ORDR && m.NB != model.NB))
                //{
                //    ModelState.AddModelError("", "لايمكن التعديل,قيمة الترتيب مكررة");
                //}
                if (ModelState.IsValid)
                {
                    var dbZattrib = db.ZATTRIBS.Find(model.NB);
                    if (model.NAME != dbZattrib.NAME)
                    {
                        dbZattrib.NAME = model.NAME;
                    }
                    if (model.ORDR != model.ORDR)
                    {
                        dbZattrib.ORDR = model.ORDR;
                    }
                    if (model.DEFVALUE != dbZattrib.DEFVALUE)
                    {
                        dbZattrib.DEFVALUE = model.DEFVALUE;
                    }
                    if (model.REQUIRED != dbZattrib.REQUIRED)
                    {
                        dbZattrib.REQUIRED = model.REQUIRED;
                    }
                    if (model.TYP != dbZattrib.TYP)
                    {
                        dbZattrib.TYP = model.TYP;
                    }
                    try
                    {
                        db.Entry(dbZattrib).State = EntityState.Modified;
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
            initControls(model);
            return PartialView("_CreatPartial", model);
        }
        public ActionResult ZATTRIBS_Read([DataSourceRequest]DataSourceRequest request)
        {
            var zattribs = db.ZATTRIBS.SqlQuery("select * from ZATTRIBS CC ORDER BY CC.ORDR").AsQueryable();
            DataSourceResult result = zattribs.ToDataSourceResult(request, zATTRIB => new
            {
                zATTRIB.NB,
                zATTRIB.ORDR,
                zATTRIB.NAME,
                ZcarCategoryName = db.ZCARCATEGORYS.Find(zATTRIB.CATEGORYNB) != null ? db.ZCARCATEGORYS.Find(zATTRIB.CATEGORYNB).NAME : " ", //zATTRIB.CATEGORYNB,
                zATTRIB.TYP,
                zATTRIB.SIZ,
                zATTRIB.UNITNB,
                zATTRIB.REQUIRED,
                zATTRIB.MASTERINDX,
                zATTRIB.DEFVALUE,
                ZATTRIBCATEGORY = new { 
                    NAME = zATTRIB.ZATTRIBCATEGORY != null? zATTRIB.ZATTRIBCATEGORY.NAME : db.ZATTRIBCATEGORYS.Find(zATTRIB.CATEGORYNB)?.NAME,
                },
            });
            return Json(result);
        }
        public ActionResult Delete(long? id)
        {
            var Zattribs = db.ZATTRIBS.Find(id);
            if (Zattribs == null)
            {
                return Content("لا يوجد مواصفة فنية");
            }
            ViewBag.ZattribName = Zattribs.NAME;
            return PartialView(Zattribs);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(ZATTRIB model)
        {
            bool success = false;
            var ZAttrib = db.ZATTRIBS.Find(model.NB);
            ViewBag.ZattribName = ZAttrib.NAME;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ZAttrib != null)
                    {

                        db.ZATTRIBS.Attach(ZAttrib);
                        db.ZATTRIBS.Remove(ZAttrib);
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
            return PartialView("_ZattribDeletePartial", model);
        }

        //    Repository<ZATTRIBS> repo = new Repository<ZATTRIBS>(db);
        //    var list = repo.GetList(z => z.NAME == "");
        //    //IQueryable<ZATTRIBS> zattribs = list.AsEnumerable();
        //    DataSourceResult result = list.ToDataSourceResult(request, zATTRIB => new {
        //        NB = zATTRIB.NB,
        //        ORDR = zATTRIB.ORDR,
        //        NAME = zATTRIB.NAME,
        //        CATEGORYNB = zATTRIB.CATEGORYNB,
        //        TYP = zATTRIB.TYP,
        //        SIZ = zATTRIB.SIZ,
        //        UNITNB = zATTRIB.UNITNB,
        //        REQUIRED = zATTRIB.REQUIRED,
        //        MASTERINDX = zATTRIB.MASTERINDX,
        //        DEFVALUE = zATTRIB.DEFVALUE,
        //    });

        //    return Json(result);
        //}




        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBS_Create([DataSourceRequest]DataSourceRequest request, ZATTRIB zATTRIB)
        {
            int? ord = 0;
            if (!db.ZCITYS.Any())
            { ord = 0; }
            else { ord = db.ZATTRIBS.Max(p => p.ORDR); }
            if (ModelState.IsValid)
            {
                var entity = new ZATTRIB
                {
                    ORDR = ord + 1,
                    NAME = zATTRIB.NAME,
                    CATEGORYNB = zATTRIB.CATEGORYNB,
                    TYP = zATTRIB.TYP,
                    SIZ = zATTRIB.SIZ,
                    UNITNB = zATTRIB.UNITNB,
                    REQUIRED = zATTRIB.REQUIRED,
                    MASTERINDX = zATTRIB.MASTERINDX,
                    DEFVALUE = zATTRIB.DEFVALUE,
                };

                db.ZATTRIBS.Add(entity);
                db.SaveChanges();
                zATTRIB.NB = entity.NB;
            }

            return Json(new[] { zATTRIB }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBS_Update([DataSourceRequest]DataSourceRequest request, ZATTRIB zATTRIB)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZATTRIB
                {
                    NB = zATTRIB.NB,
                    ORDR = zATTRIB.ORDR,
                    NAME = zATTRIB.NAME,
                    CATEGORYNB = zATTRIB.CATEGORYNB,
                    TYP = zATTRIB.TYP,
                    SIZ = zATTRIB.SIZ,
                    UNITNB = zATTRIB.UNITNB,
                    REQUIRED = zATTRIB.REQUIRED,
                    MASTERINDX = zATTRIB.MASTERINDX,
                    DEFVALUE = zATTRIB.DEFVALUE,
                };

                db.ZATTRIBS.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { zATTRIB }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBS_Destroy([DataSourceRequest]DataSourceRequest request, ZATTRIB zATTRIB)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZATTRIB
                {
                    NB = zATTRIB.NB,
                    ORDR = zATTRIB.ORDR,
                    NAME = zATTRIB.NAME,
                    CATEGORYNB = zATTRIB.CATEGORYNB,
                    TYP = zATTRIB.TYP,
                    SIZ = zATTRIB.SIZ,
                    UNITNB = zATTRIB.UNITNB,
                    REQUIRED = zATTRIB.REQUIRED,
                    MASTERINDX = zATTRIB.MASTERINDX,
                    DEFVALUE = zATTRIB.DEFVALUE,
                };

                db.ZATTRIBS.Attach(entity);
                db.ZATTRIBS.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zATTRIB }.ToDataSourceResult(request, ModelState));
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
