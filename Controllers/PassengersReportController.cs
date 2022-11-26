using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using Proced.DataAccess.Models.CF;
using Rotativa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ClosedXML.Excel;
using System.IO;

namespace Passengers.Controllers
{
    [checksession, Authorize, RedirectOnError, CanDoIt]
    public class PassengersReportController : Controller
    {

        private ProcedContext db = new ProcedContext();
        // GET: PassengersReport
        public ActionResult CommittesAndMembers()
        {
            return View();
        }
        public ActionResult CommittesAndMembers_Read([DataSourceRequest] DataSourceRequest request)
        {
            string sql = "SELECT ZC.NAME AS CityName, TM.NB AS CommNb, TM.COMNO AS CommNo , TM.COMDATE AS CommDate, TS.NAME AS CommStatus, TMM.MEMBERNAME AS MemberName, ZP.NAME  AS MemberPostion, ZSH.NAME  AS MemberShip  FROM TRCOMMITTEES  TM JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB  JOIN TRSTATUS TS ON TS.NB = TM.STATUS  JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB JOIN TRZMEMBERSHIP ZSH ON ZSH.NB = TMM.MEMBERSHIPNB WHERE 1 = 1 ";

            var NB = Request.Form["NB"].Trim();
            var COMNO = Request.Form["COMNO"].Trim();
            var COMDATESTART = Request.Form["COMDATESTART"].Trim();
            var COMDATEEND = Request.Form["COMDATEEND"].Trim();
            var STATUS = Request.Form["STATUS"].Trim();
            var MEMBERNAME = Request.Form["MEMBERNAME"].Trim();
            var MEMBERPOSITIONNB = Request.Form["MEMBERPOSITIONNB"].Trim();
            var MEMBERSHIPNB = Request.Form["MEMBERSHIPNB"].Trim();
            var COMCITYNB = Request.Form["COMCITYNB"].Trim();


            if (NB != "")
            {
                sql += " and TM.NB = " + COMNO ;
            }
            if (COMNO != "")
            {
                sql += " and TM.COMNO like '%" + COMNO + "%'";
            }
            if (STATUS != "")
            {
                sql += " and TM.STATUS =" + STATUS;
            }
            if (COMDATESTART != "")
            {
                sql += " and TM.COMDATE >= TO_DATE('" + COMDATESTART + "','DD/MM/YYYY') ";
            }

            if (COMDATEEND != "")
            {
                sql += " and TM.COMDATE <= TO_DATE('" + COMDATEEND + "','DD/MM/YYYY') ";
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and TM.COMCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and TM.COMCITYNB =" + ci;
            }
            if (MEMBERPOSITIONNB != "")
            {
                sql += " and TMM.MEMBERPOSITIONNB =" + MEMBERPOSITIONNB;
            }

            if (MEMBERSHIPNB != "")
            {
                sql += " and TMM.MEMBERSHIPNB =" + MEMBERSHIPNB;
            }
            if (MEMBERNAME != "")
            {
                sql += " and TMM.MEMBERNAME LIKE '%" + MEMBERNAME+"%' ";
            }
            var data = db.Database.SqlQuery<ViewModel.CommittesAndMembersVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult CommittesAndMembers_PDF (string sNB ,string sCOMNO, string sCOMDATESTART, string sCOMDATEEND, string sSTATUS, string sMEMBERNAME, string sMEMBERPOSITIONNB,  string sMEMBERSHIPNB, string sCOMCITYNB)
        {
            string sql = "SELECT ZC.NAME AS CityName, TM.NB AS CommNb, TM.COMNO AS CommNo , TM.COMDATE AS CommDate, TS.NAME AS CommStatus, TMM.MEMBERNAME AS MemberName, ZP.NAME  AS MemberPostion, ZSH.NAME  AS MemberShip  FROM TRCOMMITTEES  TM JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB  JOIN TRSTATUS TS ON TS.NB = TM.STATUS  JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB JOIN TRZMEMBERSHIP ZSH ON ZSH.NB = TMM.MEMBERSHIPNB WHERE 1 = 1 ";

            var NB = sNB;
            var COMNO = sCOMNO;
            var COMDATESTART = sCOMDATESTART;
            var COMDATEEND = sCOMDATEEND;
            var STATUS = sSTATUS;
            var MEMBERNAME = sMEMBERNAME;
            var MEMBERPOSITIONNB = sMEMBERPOSITIONNB;
            var MEMBERSHIPNB = sMEMBERSHIPNB;
            var COMCITYNB = sCOMCITYNB;

            
           if (NB != "")
            {
                sql += " and TM.NB = " + COMNO;
            }
            if (COMNO != "")
            {
                sql += " and TM.COMNO like '%" + COMNO + "%'";
            }
            if (STATUS != "")
            {
                sql += " and TM.STATUS =" + STATUS;
            }
            if (COMDATESTART != "")
            {
                sql += " and TM.COMDATE >= TO_DATE('" + COMDATESTART + "','DD/MM/YYYY') ";
            }

            if (COMDATEEND != "")
            {
                sql += " and TM.COMDATE <= TO_DATE('" + COMDATEEND + "','DD/MM/YYYY') ";
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and TM.COMCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and TM.COMCITYNB =" + ci;
            }
            if (MEMBERPOSITIONNB != "")
            {
                sql += " and TMM.MEMBERPOSITIONNB =" + MEMBERPOSITIONNB;
            }

            if (MEMBERSHIPNB != "")
            {
                sql += " and TMM.MEMBERSHIPNB =" + MEMBERSHIPNB;
            }
            if (MEMBERNAME != "")
            {
                sql += " and TMM.MEMBERNAME LIKE '%" + MEMBERNAME + "%' ";
            }
            var data = db.Database.SqlQuery<ViewModel.CommittesAndMembersVM>(sql).ToList();
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };
           
        }


    }
}