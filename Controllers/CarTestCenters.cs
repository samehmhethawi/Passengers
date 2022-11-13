using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Proced.DataAccess.Models.CF;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class CarTestCentersController : Controller
    {
        //private IndexesEntities db = new IndexesEntities();
        private ProcedContext db = new ProcedContext();

        public ActionResult Index()
        {
            var zCarKinds = db.ZCARKINDS.OrderBy(x => x.ORDR);
            ViewBag.ZcarKinds = new SelectList(zCarKinds, "NB", "NAME", null);
            return View();
        }

        public ActionResult CarTestCenters_Read([DataSourceRequest] DataSourceRequest request)
        {
            //IQueryable<ZCARCATEGORY> zcarcategorys = db.ZCARCATEGORYS.OrderBy(c => c.ORDR).AsQueryable();
            var carTestCenters = db.CARTEST_CENTERS.SqlQuery("select * from CARTEST_CENTERS cc ORDER by cc.nb").AsQueryable();
            //if (Request.Form.HasKeys() && !string.IsNullOrEmpty(Request.Form["sName"]) && !string.IsNullOrWhiteSpace(Request.Form["sName"]))
            //{
            //    string name = Request.Form["sName"];
            //    zcarcategorys = zcarcategorys.Where(c => c.NAME.Contains(name));
            //}
            //if (Request.Form.HasKeys() && !string.IsNullOrEmpty(Request.Form["sKindNB"]) && !string.IsNullOrWhiteSpace(Request.Form["sKindNB"]))
            //{
            //    string sKindNb = Request.Form["sKindNB"];
            //    int kindNb;
            //    if (int.TryParse(sKindNb, out kindNb))
            //    {
            //        zcarcategorys = zcarcategorys.Where(c => c.KINDNB == kindNb);
            //    }
            //}
            DataSourceResult result = carTestCenters.ToDataSourceResult(request, carTestCenter => new
            {
                carTestCenter.NB,
                carTestCenter.ANAME,
                carTestCenter.ENAME,
                carTestCenter.ACTIVE,
                carTestCenter.START_WORK_DATE,
                carTestCenter.CODE,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            var model = new CARTEST_CENTERS();
            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(CARTEST_CENTERS model)
        {
            bool success = false;
            bool successPassword = false;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.ANAME))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الاسم العربي");
                    ModelState.AddModelError("ANAME", "لايمكن الاضافة,يجب تحديد الاسم العربي");
                }
                if (string.IsNullOrEmpty(model.ENAME))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الاسم الانجليزي");
                    ModelState.AddModelError("ENAME", "لايمكن الاضافة,يجب تحديد الاسم الانجليزي");
                }
                if (string.IsNullOrEmpty(model.CODE))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد رمز المركز");
                    ModelState.AddModelError("CODE", "لايمكن الاضافة,يجب تحديد الاسم رمز المركز");
                }
                else if (db.CARTEST_CENTERS.Any(c => c.CODE == model.CODE.Trim()))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,رمز المركز مكرر");
                    ModelState.AddModelError("CODE", "لايمكن الاضافة,يجب تحديد الاسم رمز المركز مكرر");
                }
                if (string.IsNullOrEmpty(model.PASSWORD))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد كلمة السر");
                    ModelState.AddModelError("PASSWORD", "لايمكن الاضافة,يجب تحديد كلمة السر");
                }
                else
                {
                    if (string.IsNullOrEmpty(model.ConfirmPassword))
                    {
                        ModelState.AddModelError("", "لايمكن الاضافة,يجب تأكيد كلمة السر");
                        ModelState.AddModelError("ConfirmPassword", "لايمكن الاضافة,يجب تحديد كلمة السر");
                    }
                    else if (model.PASSWORD.Trim() != model.ConfirmPassword.Trim())
                    {
                        ModelState.AddModelError("", "لايمكن الاضافة,خطأ في تأكيد كلمة السر");
                    }
                }
                if (ModelState.IsValid)
                {
                    var ph = new PasswordHasher();
                    model.PASSWORD = ph.HashPassword(model.PASSWORD);
                    model.ANAME = model.ANAME.Trim();
                    model.ENAME = model.ENAME.Trim();
                    model.CODE = model.CODE.Trim();
                    model.START_WORK_DATE = DateTime.Now;
                    db.CARTEST_CENTERS.Add(model);
                    try
                    {
                        db.SaveChanges();
                        success = true;
                    }
                    catch (Exception ee)
                    {
                        // throw ee;
                        ModelState.AddModelError("", "فشلت عملية الإضافة");
                    }
                }
            }
            ViewBag.Success = success;
            return PartialView("_CreatPartial", model);
        }
        public ActionResult Edit(long? id)
        {
            var carTestCenter = db.CARTEST_CENTERS.Find(id);
            if (carTestCenter == null)
            {
                return Content("لايوجد ");
            }
            return PartialView(carTestCenter);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(CARTEST_CENTERS model)
        {
            bool success = false;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.ANAME))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الاسم العربي");
                    ModelState.AddModelError("ANAME", "لايمكن التعديل,يجب تحديد الاسم العربي");
                }
                if (string.IsNullOrEmpty(model.ENAME))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد الاسم الانجليزي");
                    ModelState.AddModelError("ENAME", "لايمكن التعديل,يجب تحديد الاسم الانجليزي");
                }
                if (string.IsNullOrEmpty(model.CODE))
                {
                    ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد رمز المركز");
                    ModelState.AddModelError("CODE", "لايمكن التعديل,يجب تحديد الاسم رمز المركز");
                }
                else if (db.CARTEST_CENTERS.Any(c => c.CODE == model.CODE.Trim()
                && c.NB != model.NB))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,رمز المركز مكرر");
                    ModelState.AddModelError("CODE", "لايمكن الاضافة,يجب تحديد الاسم رمز المركز مكرر");
                }
                if (ModelState.IsValid)
                {
                    var dbcarTestCenter = db.CARTEST_CENTERS.Find(model.NB);
                    if (model.ANAME.Trim() != dbcarTestCenter.ANAME.Trim())
                    {
                        dbcarTestCenter.ANAME = model.ANAME.Trim();
                    }
                    if (model.ANAME.Trim() != dbcarTestCenter.ANAME.Trim())
                    {
                        dbcarTestCenter.ANAME = model.ANAME.Trim();
                    }
                    if (model.CODE.Trim() != dbcarTestCenter.CODE.Trim())
                    {
                        dbcarTestCenter.CODE = model.CODE.Trim();
                    }
                    if (model.ACTIVE != dbcarTestCenter.ACTIVE)
                    {
                        dbcarTestCenter.ACTIVE = model.ACTIVE;
                    }
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
            var carTestCenter = db.CARTEST_CENTERS.Find(id);
            if (carTestCenter == null)
            {
                return Content("لا يوجد مركز فحص");
            }
            ViewBag.Name = carTestCenter.ANAME;
            return PartialView(carTestCenter);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(CARTEST_CENTERS model)
        {
            bool success = false;
            var cartetCenter = db.CARTEST_CENTERS.Find(model.NB);
            ViewBag.Name = cartetCenter.ANAME;
            try
            {
                if (cartetCenter != null)
                {
                    db.CARTEST_CENTERS.Attach(cartetCenter);
                    db.CARTEST_CENTERS.Remove(cartetCenter);
                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }
            ViewBag.Success = success;
            return PartialView("_CarTestCenterDeletePartial", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZCARCATEGORYS_Create([DataSourceRequest] DataSourceRequest request, ZCARCATEGORY zCARCATEGORY)
        {
            int? ord = 0;
            if (!db.ZCARCATEGORYS.Any())
            { ord = 0; }
            else { ord = db.ZCARCATEGORYS.Max(p => p.ORDR); }
            if (ModelState.IsValid)
            {
                var entity = new ZCARCATEGORY
                {
                    NAME = zCARCATEGORY.NAME,
                    ORDR = (short)(ord + 1),
                    KINDNB = zCARCATEGORY.KINDNB,
                    //HT = zCARCATEGORY.HT,
                    //FONTSZ = zCARCATEGORY.FONTSZ,
                };

                db.ZCARCATEGORYS.Add(entity);
                db.SaveChanges();
                zCARCATEGORY.NB = entity.NB;
            }

            return Json(new[] { zCARCATEGORY }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZCARCATEGORYS_Update([DataSourceRequest] DataSourceRequest request, ZCARCATEGORY zCARCATEGORY)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZCARCATEGORY
                {
                    NB = zCARCATEGORY.NB,
                    NAME = zCARCATEGORY.NAME,
                    ORDR = zCARCATEGORY.ORDR,
                    KINDNB = zCARCATEGORY.KINDNB,
                    HT = zCARCATEGORY.HT,
                    FONTSZ = zCARCATEGORY.FONTSZ,
                };

                db.ZCARCATEGORYS.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { zCARCATEGORY }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZCARCATEGORYS_Destroy([DataSourceRequest] DataSourceRequest request, ZCARCATEGORY zCARCATEGORY)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZCARCATEGORY
                {
                    NB = zCARCATEGORY.NB,
                    NAME = zCARCATEGORY.NAME,
                    ORDR = zCARCATEGORY.ORDR,
                    KINDNB = zCARCATEGORY.KINDNB,
                    HT = zCARCATEGORY.HT,
                    FONTSZ = zCARCATEGORY.FONTSZ,
                };

                db.ZCARCATEGORYS.Attach(entity);
                db.ZCARCATEGORYS.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zCARCATEGORY }.ToDataSourceResult(request, ModelState));
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
