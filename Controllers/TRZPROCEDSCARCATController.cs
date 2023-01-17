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
    public class TRZPROCEDSCARCATController : Controller
    {
        private ProcedContext db = new ProcedContext();
        // GET: TRZPROCEDSCARCAT
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
            ViewData["CARCAT"] = db.ZCARCATEGORYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "SELECT * FROM TRZPROCEDS_CARCAT TC JOIN ZPROCEDTYPS ZP ON ZP.NB = TC.PROCEDNB JOIN ZCARCATEGORYS CT ON CT.NB = TC.CARCATNB WHERE 1 = 1 ";

            var SPROCEDNB = Request.Form["SPROCEDNB"].Trim();
            var SCARCATNB = Request.Form["SCARCATNB"].Trim();
            if (SPROCEDNB != "")
            {
                sql += " and ZP.NAME like '%" + SPROCEDNB + "%'";
            }
            if (SCARCATNB != "")
            {
                sql += " and CT.NAME like '%" + SCARCATNB + "%'";
            }

            var data = db.Database.SqlQuery<TRZPROCEDS_CARCAT>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                PROCEDNB = commm.PROCEDNB,
                CARCATNB = commm.CARCATNB,
                STATUS = commm.STATUS,


                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
        }

        public ActionResult Create(List<int> PROCEDNB, List<int> CARCATNB)
        {

            foreach (var baseitem in PROCEDNB)
            {
                foreach (var secitem in CARCATNB)
                {
                    TRZPROCEDS_CARCAT data = new TRZPROCEDS_CARCAT();
                    data.PROCEDNB = baseitem;
                    data.CARCATNB = secitem;
                    data.STATUS = 1;
                    db.TRZPROCEDS_CARCAT.Add(data);
                   

                }

            }
            
            db.SaveChanges();
            return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(TRZPROCEDS_CARCAT model)
        {
           try
            {

                var dd = db.TRZPROCEDS_CARCAT.Find(model.NB);
               
                    if (dd.CARCATNB != model.CARCATNB)
                    {
                        dd.CARCATNB = model.CARCATNB;
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
              
                return Json(new { success = false, responseText = ex }, JsonRequestBehavior.AllowGet);
            }
           
        }
    }
}