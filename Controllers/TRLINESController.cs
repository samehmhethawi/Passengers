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
    public class TrLinesController : Controller
    {
        private ProcedContext db = new ProcedContext();
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetZOUTACT(string text)
        {
            // int userCity = int.Parse(Session["UserCity"].ToString());

            // var u_c = db.USER_CITIES.Where(us => us.CITYNB == userCity).ToList();
            ///List<USER> ulist = new List<USER>();

            var zcarregs = db.ZCARREGS.Select(zCARREG => new
            {
                NB = zCARREG.NB,
                NAME = zCARREG.NAME,
            });


            if (!string.IsNullOrEmpty(text))
            {
                zcarregs = zcarregs.Where(p => p.NAME.Contains(text)); ;

            }
            return Json(zcarregs, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TrLines_Read([DataSourceRequest]DataSourceRequest request)
        {
            //IQueryable<TRLINE> zcarregs = db.TRLINES.OrderBy(c => c.ORDR);
            IQueryable<TRLINE> zcarregs = db.TRLINES.SqlQuery("select * from TRLINES order by ORDR").AsQueryable();
            int index = 0;
            DataSourceResult result = zcarregs.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                NAME = commm.NAME,
                ORDR = commm.ORDR,
                STATUS = commm.STATUS,
                CITYNB = commm.CITYNB,
            
                //MINCARS = commm.MINCARS,
                //MAXCARS = commm.MAXCARS,
              

                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
            //DataSourceResult result = zcarregs.ToDataSourceResult(request, trLine => new
            //{
            //    trLine.NB,
            //    trLine.NAME,
            //    trLine.ORDR,
            //    trLine.STATUS,
            //    ZCITY = new {
            //        NAME= trLine.ZCITY != null? trLine.ZCITY.NAME :db.ZCITYS.Find(trLine.CITYNB) != null ? db.ZCITYS.Find(trLine.CITYNB).NAME:"",
            //    },
            //    ZTRLINETYPE = new
            //    {
            //        NAME = trLine.ZTRLINETYPE != null ? trLine.ZTRLINETYPE.NAME : db.ZTRLINETYPES.Find(trLine.TYP) != null ?  db.ZTRLINETYPES.Find(trLine.TYP).NAME: "",
            //    },
            //});
            //return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            var model = new TRLINE();
            var zTrlinesTypes = db.ZTRLINETYPES.ToList();
            ViewBag.zTrlinesTypes = zTrlinesTypes.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            var zCitys = db.ZCITYS.OrderBy(c => c.ORDR).ToList();
            ViewBag.zCitys = zCitys.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NB+"-"+ pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            if (db.TRLINES.Any())
            {
                model.ORDR = db.TRLINES.Max(m => m.ORDR) + 1;
            }
            else
            {
                model.ORDR = 1;
            }
            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(TRLINE model)
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
                    if (db.TRLINES.Any(c => c.NAME == model.NAME))
                    {
                        ModelState.AddModelError("", "لايمكن الاضافة,الاسم مكرر");
                    }
                }
                if (model.ORDR == null || model.ORDR < 0)
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد الترتيب");
                }
                if (db.TRLINES.Any(m => m.ORDR == model.ORDR))
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,قيمة الترتيب مكررة");
                }
                if (model.TYP == null )
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد نوع الخط");
                }
                if (model.CITYNB == null)
                {
                    ModelState.AddModelError("", "لايمكن الاضافة,يجب تحديد المحافظة");
                }
                if (ModelState.IsValid)
                {
                    db.TRLINES.Add(model);
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
            var zTrlinesTypes = db.ZTRLINETYPES.ToList();
            ViewBag.zTrlinesTypes = zTrlinesTypes.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            var zCitys = db.ZCITYS.OrderBy(c => c.ORDR).ToList();
            ViewBag.zCitys = zCitys.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NB + "-" + pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            ViewBag.Success = success;
            return PartialView("_CreatPartial", model);
        }
        public ActionResult Edit(long? id)
        {
            var trLine = db.TRLINES.Find(id);
            if (trLine == null)
            {
                return Content("لا يوجد خط سير ");
            }
            var zTrlinesTypes = db.ZTRLINETYPES.ToList();
            ViewBag.zTrlinesTypes = zTrlinesTypes.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            var zCitys = db.ZCITYS.OrderBy(c => c.ORDR).ToList();
            ViewBag.zCitys = zCitys.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NB + "-" + pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            return PartialView(trLine);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(TRLINE model)
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
                    if (db.TRLINES.Any(c => c.NAME == model.NAME && c.NB != model.NB))
                    {
                        ModelState.AddModelError("", "لايمكن التعديل,الاسم مكرر");
                    }
                    //if (db.TRLINES.Any(c => c.ORDR == model.ORDR && c.NB != model.NB))
                    //{
                    //    ModelState.AddModelError("", "لايمكن التعديل,الترتيب مكرر");
                    //}
                     if (model.TYP == null)
                    {
                        ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد نوع الخط");
                        ModelState.AddModelError("TYP", "لايمكن التعديل,يجب تحديد نوع الخط");
                    }
                    if (model.CITYNB == null)
                    {
                        ModelState.AddModelError("", "لايمكن التعديل,يجب تحديد المحافظة");
                        ModelState.AddModelError("CITYNB", "لايمكن التعديل,يجب تحديد المحافظة");
                    }
                }
                if (ModelState.IsValid)
                {
                    var trLine = db.TRLINES.Find(model.NB);
                    if (model.NAME != trLine.NAME)
                    {
                        trLine.NAME = model.NAME;
                    }
                    if (model.ORDR != trLine.ORDR)
                    {
                        trLine.ORDR = model.ORDR;
                    }
                    if (model.CITYNB != trLine.CITYNB)
                    {
                        trLine.CITYNB = model.CITYNB;
                    }
                    if (model.TYP != trLine.TYP)
                    {
                        trLine.TYP = model.TYP;
                    }
                    if (model.STATUS != trLine.STATUS)
                    {
                        trLine.STATUS = model.STATUS;
                    }
                    db.Entry(trLine).State = EntityState.Modified;
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
            var zTrlinesTypes = db.ZTRLINETYPES.ToList();
            ViewBag.zTrlinesTypes = zTrlinesTypes.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            var zCitys = db.ZCITYS.OrderBy(c => c.ORDR).ToList();
            ViewBag.zCitys = zCitys.ToList().Select(pt => new SelectListItem()
            {
                Text = pt.NB + "-" + pt.NAME,
                Value = "" + pt.NB
            }).ToList();
            return PartialView("_EditePartial", model);
        }
        public ActionResult Delete(long? id)
        {
            var trLine = db.TRLINES.Find(id);
            if (trLine == null)
            {
                return Content("لا يوجد خط سير");
            }
            return PartialView(trLine);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(TRLINE model)
        {
            bool success = false;
            var trLine = db.TRLINES.Find(model.NB);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (trLine != null)
                    {
                        db.TRLINES.Attach(trLine);
                        db.TRLINES.Remove(trLine);
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
            return PartialView("_TrLineDeletePartial", model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZCARREGS_Create([DataSourceRequest]DataSourceRequest request, ZCARREG zCARREG)
        {
            using (var dbx = new ProcedContext())
            {
                List<ZCARREG> tccm = dbx.ZCARREGS.ToList();
                foreach (var m in tccm)
                {
                    if (m.NAME == zCARREG.NAME && m.NB != zCARREG.NB)
                    {
                        return this.Json(new DataSourceResult
                        {
                            Errors = "لا يمكنك الإضافة،الاسم  مكرر"
                        });
                    }
                }
            }

            int? ord = 0;
            if (!db.ZCARREGS.Any())
            { ord = 0; }
            else { ord = db.ZCARREGS.Max(p => p.ORDR); }

            if (ModelState.IsValid)
            {
                var entity = new ZCARREG
                {
                    NAME = zCARREG.NAME,
                    ORDR = ord + 1,
                    ZREGNB = zCARREG.ZREGNB,
                };

                db.ZCARREGS.Add(entity);
                db.SaveChanges();
                zCARREG.NB = entity.NB;
            }

            return Json(new[] { zCARREG }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZCARREGS_Update([DataSourceRequest]DataSourceRequest request, ZCARREG zCARREG)
        {
            if (ModelState.IsValid)
            {

                using (var dbx = new ProcedContext())
                {
                    List<ZCARREG> tccm = dbx.ZCARREGS.ToList();
                    foreach (var m in tccm)
                    {
                        if (m.NAME == zCARREG.NAME && m.NB != zCARREG.NB)
                        {
                            return this.Json(new DataSourceResult
                            {
                                Errors = "لا يمكنك التعديل،الاسم مكرر"
                            });
                        }
                    }
                }
                var entity = new ZCARREG
                {
                    NB = zCARREG.NB,
                    NAME = zCARREG.NAME,
                    ORDR = zCARREG.ORDR,
                    ZREGNB = zCARREG.ZREGNB,
                };

                db.ZCARREGS.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { zCARREG }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZCARREGS_Destroy([DataSourceRequest]DataSourceRequest request, ZCARREG zCARREG)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZCARREG
                {
                    NB = zCARREG.NB,
                    NAME = zCARREG.NAME,
                    ORDR = zCARREG.ORDR,
                    ZREGNB = zCARREG.ZREGNB,
                };

                db.ZCARREGS.Attach(entity);
                db.ZCARREGS.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zCARREG }.ToDataSourceResult(request, ModelState));
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
