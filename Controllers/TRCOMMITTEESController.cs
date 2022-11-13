using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;

namespace Passengers.Controllers
{
    public class TRCOMMITTEESController : Controller
    {
        private ProcedContext db = new ProcedContext();
        // GET: TRCOMMITTEES
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            
            var data = db.Database.SqlQuery<TRCOMMITTEES>("select * from TRCOMMITTEES").ToList();/*db.TRCOMMITTEES.ToList();*/
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }

   
}