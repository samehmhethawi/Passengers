using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;

namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class TRCOMMITTEESController : Controller
    {
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        // GET: TRCOMMITTEES
        public ActionResult Index()
        {
            ViewData["STATUS"] = db.TRSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["TRZMEMBERSHIP"] = db.TRZMEMBERSHIP.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["TRZMEMBERPOSITION"] = db.TRZMEMBERPOSITION.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "select * from TRCOMMITTEES where 1 = 1 ";
            var COMNO = Request.Form["COMNO"].Trim();
            var COMDATESTART = Request.Form["COMDATESTART"].Trim();
            var COMDATEEND = Request.Form["COMDATEEND"].Trim();
            var STATUS = Request.Form["STATUS"].Trim();
            var COMCITYNB = Request.Form["COMCITYNB"].Trim();




            if (COMNO != "")
            {
                sql += " and COMNO like '%" + COMNO + "%'";
            }
            if (STATUS != "")
            {
                sql += " and STATUS =" + STATUS ;
            }

            if (COMDATESTART != "")
            {
                sql += " and COMDATE >= TO_DATE('"+ COMDATESTART+"','DD/MM/YYYY') ";
            }

            if (COMDATEEND != "")
            {
                sql += " and COMDATE <= TO_DATE('" + COMDATEEND + "','DD/MM/YYYY') ";
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and COMCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and COMCITYNB =" + ci;
            }

            sql += " order by nb desc";
            var data = db.Database.SqlQuery<TRCOMMITTEES>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                COMNO = commm.COMNO,
                COMDATE = commm.COMDATE,
                COMCITYNB = commm.COMCITYNB,
                STATUS = commm.STATUS,
                ORDR = commm.ORDR,
                IUSER = commm.IUSER,
                IDATE = commm.IDATE,
                Seq = (request.Page -1) * request.PageSize + (++index)
            });
            return Json(result);
           
        }
  
        public ActionResult Create(TRCOMMITTEES model)
        {
            try
            {
                var is_com_exists = db.Database.SqlQuery<int>("select 1 from TRCOMMITTEES where COMCITYNB = " + model.COMCITYNB + " and STATUS = 1").FirstOrDefault();
                if (is_com_exists == 1)
                {
                    return Json(new { success = false, responseText ="يوجد لجنة فعالة لهذه المحافظة " });
                }
                else
                {
                    model.IUSER = Utility.MyName();
                    model.IDATE = DateTime.Now;
                    db.TRCOMMITTEES.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
             
            }
            catch (Exception ex)
            {
                var SS = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText =  SS });
            }
           
        }

        public ActionResult Update(TRCOMMITTEES model)
        {
            try
            {
                var dd = db.TRCOMMITTEES.Find(model.NB);
                if (model.STATUS != 1)
                {
                    if (dd.COMNO != model.COMNO)
                    {
                        dd.COMNO = model.COMNO;
                    }
                    if (dd.COMDATE != model.COMDATE)
                    {
                        dd.COMDATE = model.COMDATE;
                    }

                    if (dd.STATUS != model.STATUS)
                    {
                        dd.STATUS = model.STATUS;
                    }
                    db.Entry(dd).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }

                else if (dd.STATUS == model.STATUS)
                {
                    if (dd.COMNO != model.COMNO)
                    {
                        dd.COMNO = model.COMNO;
                    }
                    if (dd.COMDATE != model.COMDATE)
                    {
                        dd.COMDATE = model.COMDATE;
                    }

                    db.Entry(dd).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }else
                {
                    var is_com_exists = db.Database.SqlQuery<int>("select 1 from TRCOMMITTEES where COMCITYNB = " + dd.COMCITYNB + " and STATUS = 1").FirstOrDefault();

                    if (is_com_exists == 1)
                    {
                        return Json(new { success = false, responseText = "يوجد لجنة فعالة لهذه المحافظة " });
                    }
                    else
                    {

                        if (dd.COMNO != model.COMNO)
                        {
                            dd.COMNO = model.COMNO;
                        }
                        if (dd.COMDATE != model.COMDATE)
                        {
                            dd.COMDATE = model.COMDATE;
                        }

                        if (dd.STATUS != model.STATUS)
                        {
                            dd.STATUS = model.STATUS;
                        }
                        db.Entry(dd).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                }
   
             
            }
            catch (Exception ex)
            {
                var SS = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = SS });
            }

        }

        public ActionResult Read_MEMBERS([DataSourceRequest] DataSourceRequest request ,int Nb)
        {
            var sql = "select * from TRCOMMITTEES_MEMBERS where 1 = 1 and STATUS = 1 and  COMMITTEENB = " + Nb ;
           

            sql += " order by ORDR";
            var data = db.Database.SqlQuery<TRCOMMITTEES_MEMBERS>(sql).ToList();

            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                COMMITTEENB = commm.COMMITTEENB,
                MEMBERSHIPNB = commm.MEMBERSHIPNB,
                MEMBERNAME = commm.MEMBERNAME,
                ORDR = commm.ORDR,
                NOTES = commm.NOTES,
                MEMBERPOSITIONNB = commm.MEMBERPOSITIONNB,
                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);

        }


        public ActionResult Create_Member(TRCOMMITTEES_MEMBERS model)
        {
            try
            {
                var ssss = "select zh.ORDR from TRZMEMBERSHIP zh where zh.nb = " + model.MEMBERSHIPNB;
                if (model.MEMBERSHIPNB == 1 || model.MEMBERSHIPNB == 2 || model.MEMBERSHIPNB == 3) 
                {
                    var countship = db.Database.SqlQuery<int>("select count(*) from TRCOMMITTEES_MEMBERS where STATUS = 1 and COMMITTEENB =" + model .COMMITTEENB + " and MEMBERSHIPNB ="+ model.MEMBERSHIPNB).FirstOrDefault();
                    if (countship > 0) 
                    {
                        return Json(new { success = false, responseText = "لا يمكن اضافة هذه العضوية مرتين الى اللجنة" });

                    }
                    else
                    {
                        model.ORDR = db.Database.SqlQuery<long?>(ssss).FirstOrDefault();
                        db.TRCOMMITTEES_MEMBERS.Add(model);
                        db.SaveChanges();
                        return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                    }

                }
              

                  model.ORDR = db.Database.SqlQuery<long?>(ssss).FirstOrDefault();
                    db.TRCOMMITTEES_MEMBERS.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                

            }
            catch (Exception ex)
            {
                var SS = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = SS });
            }

        }
        
        public ActionResult Read_Carowners([DataSourceRequest] DataSourceRequest request)
        {
            string sql = "select * from carowners where  1 = 1 ";


       
           var NB = Request.Form["SOWNERNB"].Trim();
            var NAME = Request.Form["NAME"].Trim();
            var LASTNAME = Request.Form["LASTNAME"].Trim();
            var FATHER = Request.Form["FATHER"].Trim();
            var MOTHER = Request.Form["MOTHER"].Trim();
            var NATIONNO = Request.Form["NATIONNO"].Trim();
           

            if (NB != "")
            {
               long NNB = long.Parse(NB); 
                sql += " and nb = " + NNB;
            }
            if (NATIONNO != "")
            {
                
                sql += " and NATIONNO  ='" + NATIONNO + "'";
            }
            if (NAME != "")
            {
                sql += " and NAME like '%" + NAME + "%' ";
            }
            if (LASTNAME != "")
            {
                sql += " and LASTNAME like '%" + LASTNAME + "%' ";
            }
            if (FATHER != "")
            {
                sql += " and FATHER like '%" + FATHER + "%' ";
            }
            if (MOTHER != "")
            {
                sql += " and MOTHER like '%" + MOTHER + "%' ";
            }


            var data = db.Database.SqlQuery<CAROWNER>(sql).ToList();
           // int index = 0;
            //DataSourceResult result = data.ToDataSourceResult(request, commm => new
            //{
            //    NB = commm.NB,
            //    NAME = commm.NAME,
            //    LASTNAME = commm.LASTNAME,
            //    FATHER = commm.FATHER,
            //    MOTHER = commm.MOTHER,
            //    NATIONNO = commm.NATIONNO,
              
            //    Seq = (request.Page - 1) * request.PageSize + (++index)
            //});
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet );

        }


        public ActionResult UpdateMemberStatus( long nb )
        {
            try 
            {


                var data = db.TRCOMMITTEES_MEMBERS.Find(nb);

                var comdate = db.TRCOMMITTEES.Find(data.COMMITTEENB);
                if (comdate.STATUS == 1)
                {
                    data.STATUS = 3;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, responseText = "لا يمكن تعديل الاعضاء لان اللجنة غير فعالة" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                var SS = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = SS });
            }
            
        }

    }


}