using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class TrLinesController : Controller
    {
        private ProcedContext db = new ProcedContext();
        private ValidationController validation = new ValidationController();
        public ActionResult Index()
        {
            ViewData["ZTRLINETYPES"] = db.ZTRLINETYPES.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
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

        public ActionResult TrLines_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "select * from TRLINES where 1 = 1 ";
            var STrline = Request.Form["STrline"].Trim();
            var SlineCity = Request.Form["SlineCity"].Trim();
            var StrlineStatus = Request.Form["StrlineStatus"].Trim();
            var StrlineCANCELD = Request.Form["StrlineCANCELD"].Trim();
            var STrnb = Request.Form["STrnb"].Trim();

            var STrlinepath = Request.Form["STrlinepath"].Trim();


            if (STrnb != "")
            {
                sql += " and nb = " + STrnb;
            }

            if (STrline != "")
            {
                sql += " and name like '" + STrline + "'";
            }

            if (STrlinepath != "")
            {
                sql += " and LINEPATH like '" + STrlinepath + "'";
            }
            if (StrlineStatus != "")
            {
                sql += " and STATUS = " + StrlineStatus;
            }

            if (StrlineCANCELD != "")
            {
                sql += " and ISCANCELD = " + StrlineCANCELD;
            }
            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();
            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql += " and CITYNB =" + SlineCity;
                }
            }
            else
            {
                sql += " and CITYNB =" + ci;
            }

            var data = db.Database.SqlQuery<TRLINE>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                NAME = commm.NAME,
                TYP = commm.TYP,
                ORDR = commm.ORDR,
                STATUS = commm.STATUS,
                CITYNB = commm.CITYNB,
                ISCANCELD = commm.ISCANCELD,
                MINCARS = commm.MINCARS,
                MAXCARS = commm.MAXCARS,
                SESNB = commm.SESNB,
                LINEPATH = commm.LINEPATH,
                CANEDIT = commm.CANEDIT,


                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);

        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string Name, int? city, int? typ, long? min, long? max, List<int> allcity)
        {
            try
            {
                if (Name == "")
                {
                    return Json(new { success = false, responseText = "الاسم فارغ" });
                }
                if (!city.HasValue)
                {
                    return Json(new { success = false, responseText = "يجب تحديد محافظة الخط" });
                }
                if (!typ.HasValue)
                {
                    return Json(new { success = false, responseText = "يجب تحديد نوع الخط" });
                }

                var Isexis = db.Database.SqlQuery<int>("select count(*) from TRLINES where Name ='" + Name + "'").FirstOrDefault();
                if (Isexis > 0)
                {
                    return Json(new { success = false, responseText = "اسم الخط موجود مسبقاً" });
                }
                //if (!min.HasValue || !max.HasValue)
                //{
                //    return Json(new { success = false, responseText = "يجب تحديد العدد الادنى و الاعلى" });
                //}
                TRLINE tRLINE = new TRLINE();
                tRLINE.NAME = Name;
                tRLINE.TYP = typ;
                tRLINE.MAXCARS = max;
                tRLINE.MINCARS = min;
                tRLINE.CITYNB = city;
                tRLINE.STATUS = true;
                tRLINE.ISCANCELD = 0;
                db.TRLINES.Add(tRLINE);

                db.SaveChanges();

                TRLINE_CITY cc = new TRLINE_CITY();
                cc.LINENB = tRLINE.NB;
                cc.CITYNB = (int)city;
                cc.ORDR = 1;
                db.TRLINE_CITY.Add(cc);
                db.SaveChanges();
                if (allcity != null)
                {
                    foreach (var item in allcity)
                    {
                        if (item != city)
                        {
                            cc.LINENB = tRLINE.NB;
                            cc.CITYNB = item;
                            cc.ORDR = db.Database.SqlQuery<long?>("select nvl(Max(ordr),0)+1 from TRLINE_CITY where LINENB = " + tRLINE.NB).FirstOrDefault();
                            db.TRLINE_CITY.Add(cc);
                        }
                        db.SaveChanges();
                    }
                }


                db.SaveChanges();
                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var ss = validation.OracleExceptionValidation(ex);
                return Json(new { success = false, responseText = ss });
            }

        }

        public ActionResult Read_city([DataSourceRequest] DataSourceRequest request, long? Nb)
        {
            var sql = "select * from TRLINE_CITY where 1 = 1 and LINENB = " + Nb + " order by ordr";

            //var STrline = Request.Form["STrline"].Trim();
            //var SlineCity = Request.Form["SlineCity"].Trim();
            //var StrlineStatus = Request.Form["StrlineStatus"].Trim();
            //var StrlineCANCELD = Request.Form["StrlineCANCELD"].Trim();

            var data = db.Database.SqlQuery<TRLINE_CITY>(sql).ToList();

            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                ORDR = commm.ORDR,
                CITYNB = commm.CITYNB,
                LINENB = commm.LINENB

            });
            return Json(result);
        }


        public ActionResult Deletecity(long? nb)
        {
            try
            {
                var dd = db.TRLINE_CITY.Find(nb);
                if (dd == null)
                {
                    return Json(new { success = false, responseText = " لا يوجد سجل" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (dd != null)
                            {
                                db.TRLINE_CITY.Attach(dd);
                                db.TRLINE_CITY.Remove(dd);
                                db.SaveChanges();
                                transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                        }
                    }
                }


                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult addcitytoline(long? linb, long? city)
        {
            try
            {
                var isexis = db.Database.SqlQuery<int>("select count(*) from TRLINE_CITY where LINENB = " + linb + " and CITYNB = " + city).FirstOrDefault();
                if (isexis > 0)
                {
                    return Json(new { success = false, responseText = "المحافظة المختارة موجودة على الخط" });
                }
                TRLINE_CITY cc = new TRLINE_CITY();
                cc.LINENB = (long)linb;
                cc.CITYNB = (int)city;
                cc.ORDR = db.Database.SqlQuery<long?>("select nvl(Max(ordr),0)+1 from TRLINE_CITY where LINENB = " + linb).FirstOrDefault();
                db.TRLINE_CITY.Add(cc);
                db.SaveChanges();

                return Json(new { success = true, responseText = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, responseText = e.Message });
            }
        }
        public ActionResult SetCanEditTrue(long? linb)
        {
            try
            {
                var data = db.TRLINES.Find(linb);

                data.CANEDIT = true;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();


                return Json(new { success = true, responseText = "تم التعديل بنجاح" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, responseText = e.Message });
            }
        }

        public ActionResult update(long? Nb, string Name, int? city, int? typ, long? min, long? max, int? STATUS, int? ISCANCELD, string eLINEPATH, string mod)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (!Nb.HasValue)
                    {
                        return Json(new { success = false, responseText = "الرمز فارغ" });
                    }
                    if (!STATUS.HasValue)
                    {
                        return Json(new { success = false, responseText = "يجب اختيار هل هو فعال او لا" });
                    }
                    if (!ISCANCELD.HasValue)
                    {
                        return Json(new { success = false, responseText = "يجب اختيار هل هو ملغى او لا" });
                    }
                    if (Name == "")
                    {
                        return Json(new { success = false, responseText = "الاسم فارغ" });
                    }
                    if (eLINEPATH == "")
                    {
                        return Json(new { success = false, responseText = "المسار فارغ" });
                    }


                    if (!city.HasValue)
                    {
                        return Json(new { success = false, responseText = "يجب تحديد محافظة الخط" });
                    }
                    if (!typ.HasValue)
                    {
                        return Json(new { success = false, responseText = "يجب تحديد نوع الخط" });
                    }
                    //if (!min.HasValue || !max.HasValue)
                    //{
                    //    return Json(new { success = false, responseText = "يجب تحديد العدد الادنى و الاعلى" });
                    //}



                    var subjects = JsonConvert.DeserializeObject<List<TRLINE_CITY>>(mod);
                    if (subjects.Count() == 0)
                    {
                        return Json(new { success = false, responseText = "يجب اضافة محافظة على الاقل" });
                    }



                    var data = db.TRLINES.Find(Nb);

                    if (data == null)
                    {

                        return Json(new { success = false, responseText = "لا يوجد سجل موافق" });

                    }
                    if (data.CANEDIT == false)
                    {
                        return Json(new { success = false, responseText = "عذراً لا يمكن تعديل هذا الخط " });
                    }

                    if (data.NAME != Name)
                    {
                        data.NAME = Name;
                    }

                    if (data.LINEPATH != eLINEPATH)
                    {
                        data.LINEPATH = eLINEPATH;
                    }


                    if (data.CITYNB != city)
                    {
                        data.CITYNB = city;
                    }
                    if (data.TYP != typ)
                    {
                        data.TYP = typ;
                    }
                    if (data.MINCARS != min)
                    {
                        data.MINCARS = min;
                    }
                    if (data.MAXCARS != max)
                    {
                        data.MAXCARS = max;
                    }
                    if (data.ISCANCELD != ISCANCELD)
                    {
                        data.ISCANCELD = ISCANCELD;
                    }
                    bool? dSTATUS = true;
                    if (STATUS == 1)
                    {
                        dSTATUS = true;
                    }
                    if (STATUS == 0)
                    {
                        dSTATUS = false;
                    }
                    if (data.STATUS != dSTATUS)
                    {
                        data.STATUS = dSTATUS;
                    }



                    var all_city_line = db.Database.SqlQuery<TRLINE_CITY>("select * from TRLINE_CITY where LINENB = " + Nb).ToList();
                    foreach (var item in all_city_line)
                    {

                        var ddd = db.TRLINE_CITY.Find(item.NB);


                        try
                        {
                            if (ddd != null)
                            {
                                db.TRLINE_CITY.Attach(ddd);
                                db.TRLINE_CITY.Remove(ddd);

                                db.SaveChanges();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                        }



                    }


                    foreach (var item in subjects)
                    {

                        db.TRLINE_CITY.Add(item);


                    }

                    data.CANEDIT = false;

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true, responseText = "ok" });


                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    var ss = validation.OracleExceptionValidation(ex);
                    return Json(new { success = false, responseText = ss });

                }
            }

        }



        public ActionResult MergeTrLines()
        {
            ViewData["ZTRLINETYPES"] = db.ZTRLINETYPES.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            return View();
        }

        public ActionResult TrLines_Read_toMerge([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "select * from TRLINES where 1 = 1 ";
            var STrline = Request.Form["STrline"].Trim();
            var SlineCity = Request.Form["SlineCity"].Trim();
            var StrlineStatus = Request.Form["StrlineStatus"].Trim();
            var StrlineCANCELD = Request.Form["StrlineCANCELD"].Trim();
            var STrnb = Request.Form["STrnb"].Trim();

            var STrlinepath = Request.Form["STrlinepath"].Trim();

            var mod = Request.Form["sbjstosc"].Trim();

            var subjects = JsonConvert.DeserializeObject<List<TRLINE>>(mod);

            if (STrnb != "")
            {
                sql += " and nb = " + STrnb;
            }

            if (subjects.Count != 0)
            {
                sql += " and nb not in (";
                foreach (var subject in subjects)
                {
                    sql += " " + subject.NB + " , ";
                }
                sql += " 0 )";
            }

            if (STrline != "")
            {
                sql += " and name like '" + STrline + "'";
            }

            if (STrlinepath != "")
            {
                sql += " and LINEPATH like '" + STrlinepath + "'";
            }
            if (StrlineStatus != "")
            {
                sql += " and STATUS = " + StrlineStatus;
            }

            if (StrlineCANCELD != "")
            {
                sql += " and ISCANCELD = " + StrlineCANCELD;
            }
            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();
            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql += " and CITYNB =" + SlineCity;
                }
            }
            else
            {
                sql += " and CITYNB =" + ci;
            }

            sql += " and nb not in (SELECT TRLINENB FROM TRLINES_TO_MERGE) ";

            var data = db.Database.SqlQuery<TRLINE>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                NAME = commm.NAME,
                TYP = commm.TYP,
                ORDR = commm.ORDR,
                STATUS = commm.STATUS,
                CITYNB = commm.CITYNB,
                ISCANCELD = commm.ISCANCELD,
                MINCARS = commm.MINCARS,
                MAXCARS = commm.MAXCARS,
                SESNB = commm.SESNB,
                LINEPATH = commm.LINEPATH,
                CANEDIT = commm.CANEDIT,


                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);

        }


        public ActionResult AddNbToMerge(string mod, long? ParentNb, string ParentName)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var subjects = JsonConvert.DeserializeObject<List<TRLINE>>(mod);
                    var username = Utility.MyName();

                    foreach (var item in subjects)
                    {
                        if (item.NB != ParentNb)
                        {
                            var sql = "INSERT INTO TRLINES_TO_MERGE (NB,"
                                                                + " TRLINENB, "
                                                              + " PARENT_TRLINENB, "
                                                              + " PARENT_TRLINENAME, "
                                                              + " TRLINENAME, "
                                                              + " IUSER, "
                                                              + " IDATE," 
                                                              + "ISMERGE) "
                                                              + " VALUES"
                                                              + "(1, "
                                                              + "  " + item.NB + ", "
                                                              + "  " + ParentNb + ", "
                                                              + "  '" + ParentName + "', "
                                                              + "  '" + item.NAME + "', "
                                                              + "  '"+ username + "', "
                                                              + "  SYSDATE,"
                                                              + " 0) ";
                            db.Database.ExecuteSqlCommand(sql);
                        }


                    }
                    transaction.Commit();
                    return Json(new { success = true, responseText = "ok" });

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var ss = validation.OracleExceptionValidation(ex);
                    return Json(new { success = false, responseText = ss });

                }

            }

        }




        public ActionResult TRLINES_TO_MERGE()
        {

            return View();
        }

        public ActionResult TRLINES_TO_MERGE_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = "select * from TRLINES_TO_MERGE where 1 = 1 ";
            var STrline = Request.Form["STrline"].Trim();
            var STrnb = Request.Form["STrnb"].Trim();
            var STrlineParent = Request.Form["STrlineParent"].Trim();
            var STrnbParent = Request.Form["STrnbParent"].Trim();


            if (STrnb != "")
            {
                sql += " and TRLINENB = " + STrnb;
            }
            if (STrnbParent != "")
            {
                sql += " and PARENT_TRLINENB = " + STrnbParent;
            }


            if (STrline != "")
            {
                sql += " and TRLINENAME like '" + STrline + "'";
            }

            if (STrlineParent != "")
            {
                sql += " and PARENT_TRLINENAME like '" + STrlineParent + "'";
            }
            //if (STrlinepath != "")
            //{
            //    sql += " and LINEPATH like '" + STrlinepath + "'";
            //}
            //if (StrlineStatus != "")
            //{
            //    sql += " and STATUS = " + StrlineStatus;
            //}

            //if (StrlineCANCELD != "")
            //{
            //    sql += " and ISCANCELD = " + StrlineCANCELD;
            //}
            //CodesController bb = new CodesController();

            //var ci = bb.GetCityForRead();
            //if (ci == "0")
            //{
            //    if (SlineCity != "")
            //    {
            //        sql += " and CITYNB =" + SlineCity;
            //    }
            //}
            //else
            //{
            //    sql += " and CITYNB =" + ci;
            //}

            var data = db.Database.SqlQuery<TRLINES_TO_MERGE>(sql).ToList();
            int index = 0;
            DataSourceResult result = data.ToDataSourceResult(request, commm => new
            {
                NB = commm.NB,
                TRLINENB = commm.TRLINENB,
                PARENT_TRLINENB = commm.PARENT_TRLINENB,
                PARENT_TRLINENAME = commm.PARENT_TRLINENAME,
                TRLINENAME = commm.TRLINENAME,
                IUSER = commm.IUSER,
                IDATE = commm.IDATE,



                Seq = (request.Page - 1) * request.PageSize + (++index)
            });
            return Json(result);
        }
        public ActionResult DeleteNbFromMerge(long NB)
        {
            try
            {
                var dd = db.TRLINES_TO_MERGE.Find(NB);
                if (dd == null)
                {
                    return Json(new { success = false, responseText = " لا يوجد سجل" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (dd.ISMERGE == true)
                    {
                        return Json(new { success = false, responseText = "لا يمكن الحذف لانه تم الدمج" }, JsonRequestBehavior.AllowGet);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (dd != null)
                            {
                                db.TRLINES_TO_MERGE.Attach(dd);
                                db.TRLINES_TO_MERGE.Remove(dd);
                                db.SaveChanges();
                                transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                        }
                    }
                }


                return Json(new { success = true, responseText = "تم الحذف" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = ex }, JsonRequestBehavior.AllowGet);
            }
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
        public ActionResult ZCARREGS_Create([DataSourceRequest] DataSourceRequest request, ZCARREG zCARREG)
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
        public ActionResult ZCARREGS_Update([DataSourceRequest] DataSourceRequest request, ZCARREG zCARREG)
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
        public ActionResult ZCARREGS_Destroy([DataSourceRequest] DataSourceRequest request, ZCARREG zCARREG)
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
