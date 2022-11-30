using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Passengers.Controllers
{
    public  class CodesController : Controller
    {
        public ProcedContext db = new ProcedContext();
        // GET: Codes

        public bool IsAdmin()
        {

            var usernb = Utility.MyNB();
            var admin = db.Database.SqlQuery<int>("select 1 from APPMGR.USER_ROLES where ROLENB = 0 and USERNB = " + usernb).FirstOrDefault();
            if (admin == 0)
            { return false; }
            else
            { return true; }

        }
        public bool IsAdminCity()
        {
            var usernb = Utility.MyNB();
            var admin = db.Database.SqlQuery<int>("select 1 from APPMGR.USER_ROLES where ROLENB = 1000 and USERNB = " + usernb).FirstOrDefault();
            if (admin == 0)
            { return false; }
            else
            { return true; }

        }

        public  string GetCityForRead()
        {

            if (IsAdmin() || IsAdminCity())
            { return "0"; }
            else
            {
                return Utility.MyCityNb().ToString();
            }
        }
        public ActionResult GetStatus()
        {

            var status = db.TRSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCity()
        {
            var _isadmin = IsAdmin();
            var _isadmincity = IsAdminCity();
            if (_isadmin || _isadmincity)
            {
                var city = db.ZCITYS.Select(x => new
                {
                    ID = x.NB,
                    NAME = x.NAME,
                });
                return Json(city, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var mycitynb = Utility.MyCityNb();
                var city = db.ZCITYS.Where(y => y.NB == mycitynb).Select(x => new
                {
                    ID = x.NB,
                    NAME = x.NAME,
                });
                return Json(city, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetMemberShip()
        {

            var status = db.TRZMEMBERSHIP.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMemberPostion()
        {

            var status = db.TRZMEMBERPOSITION.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }


        public int GetMaxOrdr(string TableName)
        {
           var sql = "SELECT NVL(MAX(ORDR),0) FROM " + TableName;
            var data = db.Database.SqlQuery<int>(sql).FirstOrDefault();  
            return data +1;
        }

        public ActionResult GetProced()
        {

            var status = db.ZPROCEDTYPS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCarCat()
        {

            var status = db.ZCARCATEGORYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }


        
    }
}

