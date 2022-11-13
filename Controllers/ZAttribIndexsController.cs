using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace Passengers.Controllers
{
        [checksession, Authorize, RedirectOnError,CanDoIt]
    public class ZAttribIndexsController : Controller
    {
        //private IndexesEntities db = new IndexesEntities();
        private ProcedContext db = new ProcedContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ZATTRIBINDXS_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ZATTRIBINDX> zattribindxs = db.ZATTRIBINDXS;
            DataSourceResult result = zattribindxs.ToDataSourceResult(request, zATTRIBINDX => new
            {
                NB = zATTRIBINDX.NB,
                ORDR = zATTRIBINDX.ORDR,
                NAME = zATTRIBINDX.NAME,
                TABNAME = zATTRIBINDX.TABNAME
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBINDXS_Create([DataSourceRequest]DataSourceRequest request, ZATTRIBINDX zATTRIBINDX)

        {
            int? ord = 0;



            ord = (int?)db.ZATTRIBINDXS.Max(p => p.ORDR);
            if (ord == null)
            {
                ord = 0;
            }

            if (ModelState.IsValid)
            {
                var entity = new ZATTRIBINDX
                {
                    ORDR = ord + 1,
                    NAME = zATTRIBINDX.NAME,
                    TABNAME = zATTRIBINDX.TABNAME
                };

                db.ZATTRIBINDXS.Add(entity);
                db.SaveChanges();
                zATTRIBINDX.NB = entity.NB;
            }

            return Json(new[] { zATTRIBINDX }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBINDXS_Update([DataSourceRequest]DataSourceRequest request, ZATTRIBINDX zATTRIBINDX)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZATTRIBINDX
                {
                    NB = zATTRIBINDX.NB,
                    ORDR = zATTRIBINDX.ORDR,
                    NAME = zATTRIBINDX.NAME,
                    TABNAME = zATTRIBINDX.TABNAME
                };

                db.ZATTRIBINDXS.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { zATTRIBINDX }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ZATTRIBINDXS_Destroy([DataSourceRequest]DataSourceRequest request, ZATTRIBINDX zATTRIBINDX)
        {
            if (ModelState.IsValid)
            {
                var entity = new ZATTRIBINDX
                {
                    NB = zATTRIBINDX.NB,
                    ORDR = zATTRIBINDX.ORDR,
                    NAME = zATTRIBINDX.NAME,
                    TABNAME = zATTRIBINDX.TABNAME
                };

                db.ZATTRIBINDXS.Attach(entity);
                db.ZATTRIBINDXS.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { zATTRIBINDX }.ToDataSourceResult(request, ModelState));
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
