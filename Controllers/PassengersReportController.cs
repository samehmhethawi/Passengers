using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

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
                sql += " and TM.NB = " + NB;
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
            var data = db.Database.SqlQuery<ViewModel.CommittesAndMembersVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CommittesAndMembers_PDF(string sNB, string sCOMNO, string sCOMDATESTART, string sCOMDATEEND, string sSTATUS, string sMEMBERNAME, string sMEMBERPOSITIONNB, string sMEMBERSHIPNB, string sCOMCITYNB)
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
                sql += " and TM.NB = " + NB;
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



        public ActionResult CommittesAndSessions()
        {
            return View();
        }

        public ActionResult CommittesAndSessions_Read([DataSourceRequest] DataSourceRequest request)
        {

            string sql = "SELECT ZC.NAME AS CityName,"
            + " TM.NB AS CommNb, "
            + " TM.COMNO AS CommNo, "
     + " TM.COMDATE AS CommDate, "
     + " TS.NAME AS CommStatus, "
     + " TCS.NB AS SessNb, "
     + " TCS.SESNO AS SessNo, "
     + " TCS.SESDATE AS SessDate, "
     + " TS2.NAME AS SessStatus, "
     + " TMM.MEMBERNAME AS BossName, "
     + " ZP.NAME AS BossPostion "
     + " FROM TRCOMMITTEES TM "
     + " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB "
     + " JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB "
     + " JOIN TRSTATUS TS ON TS.NB = TM.STATUS "
     + "  JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB "
     + " JOIN TRSESSIONS TCS ON TCS.COMMITTEENB = TM.NB "
     + " JOIN TRSTATUS TS2 ON TS2.NB = TCS.STATUS "
     + " WHERE 1 = 1 AND TMM.MEMBERSHIPNB = 1 ";

            var NB = Request.Form["NB"].Trim();
            var COMNO = Request.Form["COMNO"].Trim();
            var COMDATESTART = Request.Form["COMDATESTART"].Trim();
            var COMDATEEND = Request.Form["COMDATEEND"].Trim();
            var STATUS = Request.Form["COMSTATUS"].Trim();

            var sNB = Request.Form["sNB"].Trim();
            var sNO = Request.Form["sNO"].Trim();
            var sDATESTART = Request.Form["sDATESTART"].Trim();
            var sDATEEND = Request.Form["sDATEEND"].Trim();
            var sSTATUS = Request.Form["sSTATUS"].Trim();

            var COMCITYNB = Request.Form["COMCITYNB"].Trim();




            if (NB != "")
            {
                sql += " and TM.NB = " + NB;
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



            if (sNB != "")
            {
                sql += " and TCS.NB = " + sNB;
            }
            if (sNO != "")
            {
                sql += " and TCS.SESNO like '%" + sNO + "%'";
            }
            if (sSTATUS != "")
            {
                sql += " and TCS.STATUS =" + sSTATUS;
            }
            if (sDATESTART != "")
            {
                sql += " and TCS.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            }

            if (sDATEEND != "")
            {
                sql += " and TCS.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
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


            var data = db.Database.SqlQuery<ViewModel.CommittesAndSessions>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommittesAndSessions_PDF(string pNB, string pCOMNO, string pCOMDATESTART, string pCOMDATEEND, string pSTATUS, string psNB, string psNO, string psDATESTART, string psDATEEND, string psSTATUS, string pCOMCITYNB)
        {
            string sql = "SELECT ZC.NAME AS CityName,"
   + " TM.NB AS CommNb, "
   + " TM.COMNO AS CommNo, "
   + " TM.COMDATE AS CommDate, "
   + " TS.NAME AS CommStatus, "
   + " TCS.NB AS SessNb, "
   + " TCS.SESNO AS SessNo, "
   + " TCS.SESDATE AS SessDate, "
   + " TS2.NAME AS SessStatus, "
   + " TMM.MEMBERNAME AS BossName, "
   + " ZP.NAME AS BossPostion "
   + " FROM TRCOMMITTEES TM "
   + " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB "
   + " JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB "
   + " JOIN TRSTATUS TS ON TS.NB = TM.STATUS "
   + "  JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB "
   + " JOIN TRSESSIONS TCS ON TCS.COMMITTEENB = TM.NB "
   + " JOIN TRSTATUS TS2 ON TS2.NB = TCS.STATUS "
   + " WHERE 1 = 1 AND TMM.MEMBERSHIPNB = 1 ";


            var NB = pNB;
            var COMNO = pCOMNO;
            var COMDATESTART = pCOMDATESTART;
            var COMDATEEND = pCOMDATEEND;
            var STATUS = pSTATUS;
            var sNB = psNB;
            var sNO = psNO;
            var sDATESTART = psDATESTART;
            var sDATEEND = psDATEEND;
            var sSTATUS = psSTATUS;
            var COMCITYNB = pCOMCITYNB;




            if (NB != "")
            {
                sql += " and TM.NB = " + NB;
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



            if (sNB != "")
            {
                sql += " and TCS.NB = " + sNB;
            }
            if (sNO != "")
            {
                sql += " and TCS.SESNO like '%" + sNO + "%'";
            }
            if (sSTATUS != "")
            {
                sql += " and TCS.STATUS =" + sSTATUS;
            }
            if (sDATESTART != "")
            {
                sql += " and TCS.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            }

            if (sDATEEND != "")
            {
                sql += " and TCS.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
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

            var data = db.Database.SqlQuery<ViewModel.CommittesAndSessions>(sql);
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };

        }
        public ActionResult CommittesAndSessions_ex()
        {


            string query = "SELECT ZC.NAME AS CityName,"
                   + " TM.NB AS CommNb, "
                   + " TM.COMNO AS CommNo, "
            + " TO_DATE(TM.COMDATE,'DD/MM/YYYY') AS CommDate, "
            + " TS.NAME AS CommStatus, "
            + " TCS.NB AS SessNb, "
            + " TCS.SESNO AS SessNo, "
            + " TO_DATE(TCS.SESDATE,'DD/MM/YYYY') AS SessDate, "
            + " TS2.NAME AS SessStatus, "
            + " TMM.MEMBERNAME AS BossName, "
            + " ZP.NAME AS BossPostion "
            + " FROM TRCOMMITTEES TM "
            + " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB "
            + " JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB "
            + " JOIN TRSTATUS TS ON TS.NB = TM.STATUS "
            + " JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB "
            + " JOIN TRSESSIONS TCS ON TCS.COMMITTEENB = TM.NB "
            + " JOIN TRSTATUS TS2 ON TS2.NB = TCS.STATUS "
            + " WHERE 1 = 1 AND TMM.MEMBERSHIPNB = 1 ";

            var data = db.Database.SqlQuery<ViewModel.CommittesAndSessions>(query);

            var data2 = from e in data
                        select new
                        {
                            المحافظة = e.CityName,
                            رمز_اللجنة = e.CommNb,
                            رقم_اللجنة = e.CommNo,
                            تاريخ_اللجنة = e.CommDate,
                            حالة_اللجنة = e.CommStatus,
                            رمز_الجلسة = e.SessNb,
                            رقم_الجلسة = e.SessNo,
                            تاريخ_الجلسة = e.SessDate,
                            حالة_الجلسة = e.SessStatus,
                            اسم_رئيس_الجلسة = e.BossName,
                            منصب_رئيس_الجلسة = e.BossPostion,
                        };

            string sheetName = "تقرير اللجان وجلساتها";
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            ws.Cell(1, 1).InsertTable(data2);

            ws.RightToLeft = true;
            ws.ColumnWidth = 30;


            //ws.Style.Font.Bold = true;
            ws.Style.Font.FontSize = 12;
            for (int i = 1; i <= 11; i++)
            {
                ws.Column(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", sheetName.Replace(" ", "_")));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();

            return View();
        }



        public ActionResult SessionsAndMembers()
        {
            return View();
        }
        public ActionResult SessionsAndMembers_Read([DataSourceRequest] DataSourceRequest request)
        {
            string sql = "SELECT ZC.NAME   AS CityName,"
       + " TSE.COMMITTEENB AS CommNb, "
       + " TSE.NB AS SessNb, "
       + " TSE.SESNO AS SessNo, "
       + " TSE.SESDATE AS SessDate, "
       + " TS.NAME AS SessStatus, "
       + " TMM.MEMBERNAME AS MemberName, "
       + " TSH.NAME AS MemberShip, "
       + "  TP.NAME AS MemberPostion, "
       + "  CASE WHEN TSP.ISPRESENT = 1 THEN 'حاضر' ELSE 'غائب' END AS ISPRESENT "
       + " FROM TRSESSIONS_MEMBERS_PRESENT TSP "
       + " JOIN TRSESSIONS TSE ON TSE.NB = TSP.SESSIONNB "
       + "  JOIN TRSTATUS TS ON TS.NB = TSE.STATUS "
       + " JOIN ZCITYS ZC ON ZC.NB = TSE.SESCITYNB "
       + " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.NB = TSP.MEMBERNB "
       + " JOIN TRZMEMBERSHIP TSH ON TSH.NB = TSP.MEMBERSHIPNB "
       + " JOIN TRZMEMBERPOSITION TP ON TP.NB = TMM.MEMBERPOSITIONNB "
       + " WHERE 1 = 1 ";



            var NB = Request.Form["NB"].Trim();
            var sNB = Request.Form["sNB"].Trim();
            var sNO = Request.Form["sNO"].Trim();
            var sDATESTART = Request.Form["sDATESTART"].Trim();
            var sDATEEND = Request.Form["sDATEEND"].Trim();
            var STATUS = Request.Form["sSTATUS"].Trim();

            var MemberSH = Request.Form["MemberSH"].Trim();
            var COMCITYNB = Request.Form["COMCITYNB"].Trim();


            if (NB != "")
            {
                sql += " and TSE.COMMITTEENB = " + NB;
            }
            if (sNB != "")
            {
                sql += " and TSE.NB =" + sNB;
            }

            if (sNO != "")
            {
                sql += " and  TSE.SESNO =" + sNO;
            }

            if (STATUS != "")
            {
                sql += " and  TSE.STATUS =" + STATUS;
            }

            if (sDATESTART != "")
            {
                sql += " and TSE.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            }

            if (sDATEEND != "")
            {
                sql += " and TSE.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
            }
            if (MemberSH != "")
            {
                sql += " and TSP.MEMBERNB =" + MemberSH;
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and TSE.SESCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and TSE.SESCITYNB =" + ci;
            }

            sql += " ORDER BY TSE.NB, TSP.MEMBERNB ";
            var data = db.Database.SqlQuery<ViewModel.SessionsAndMembersVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SessionsAndMembers_PDF(string pNB, string pCOMNO, string pCOMDATESTART, string pCOMDATEEND, string pSTATUS, string psNB, string psNO, string psDATESTART, string psDATEEND, string psSTATUS, string pMemberSH, string pCOMCITYNB)
        {



            string sql = "SELECT ZC.NAME   AS CityName,"
      + " TSE.COMMITTEENB AS CommNb, "
      + " TSE.NB AS SessNb, "
      + " TSE.SESNO AS SessNo, "
      + " TSE.SESDATE AS SessDate, "
      + " TS.NAME AS SessStatus, "
      + " TMM.MEMBERNAME AS MemberName, "
      + " TSH.NAME AS MemberShip, "
      + "  TP.NAME AS MemberPostion, "
      + "  CASE WHEN TSP.ISPRESENT = 1 THEN 'حاضر' ELSE 'غائب' END AS ISPRESENT "
      + " FROM TRSESSIONS_MEMBERS_PRESENT TSP "
      + " JOIN TRSESSIONS TSE ON TSE.NB = TSP.SESSIONNB "
      + "  JOIN TRSTATUS TS ON TS.NB = TSE.STATUS "
      + " JOIN ZCITYS ZC ON ZC.NB = TSE.SESCITYNB "
      + " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.NB = TSP.MEMBERNB "
      + " JOIN TRZMEMBERSHIP TSH ON TSH.NB = TSP.MEMBERSHIPNB "
      + " JOIN TRZMEMBERPOSITION TP ON TP.NB = TMM.MEMBERPOSITIONNB "
      + " WHERE 1 = 1 ";


            var NB = pNB;
            var sNB = psNB;
            var sNO = psNO;
            var sDATESTART = psDATESTART;
            var sDATEEND = psDATEEND;
            var STATUS = psSTATUS;
            var MemberSH = pMemberSH;
            var COMCITYNB = pCOMCITYNB;





            if (NB != "")
            {
                sql += " and TSE.COMMITTEENB = " + NB;
            }
            if (sNB != "")
            {
                sql += " and TSE.NB =" + sNB;
            }

            if (sNO != "")
            {
                sql += " and  TSE.SESNO =" + sNO;
            }
            if (STATUS != "")
            {
                sql += " and  TSE.STATUS =" + STATUS;
            }


            if (sDATESTART != "")
            {
                sql += " and TSE.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            }

            if (sDATEEND != "")
            {
                sql += " and TSE.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
            }
            if (MemberSH != "")
            {
                sql += " and TSP.MEMBERNB =" + MemberSH;
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and TSE.SESCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and TSE.SESCITYNB =" + ci;
            }

            sql += " ORDER BY TSE.NB, TSP.MEMBERNB ";
            var data = db.Database.SqlQuery<ViewModel.SessionsAndMembersVM>(sql);
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };

        }



        public ActionResult SessionsAndProceds()
        {
            return View();
        }
        public ActionResult SessionsAndProceds_Read([DataSourceRequest] DataSourceRequest request)
        {
            string sql = "SELECT ZC.NAME  AS CityName, "
      + " TSE.COMMITTEENB AS CommNb, "
       + " TSE.NB AS SessNb, "
       + " TSE.SESNO AS SessNo, "
       + " TSE.SESDATE AS SessDate, "
       + " TS.NAME AS SessStatus, "
       + " PT.NAME AS ProcedName, "
       + " TSS.NAME AS ProcedRes "
      + " FROM TRSESSIONS_PROCEDS TSP "
      + " JOIN TRSESSIONS TSE ON TSE.NB = TSP.SESSIONNB "
      + " JOIN ZCITYS ZC ON ZC.NB = TSE.SESCITYNB "
      + " JOIN TRSTATUS TS ON TS.NB = TSE.STATUS "
      + " JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB "
      + " JOIN ZPROCEDTYPS PT ON PT.NB = CP.PROCEDNB "
      + " JOIN TRSESSIONSPROCEDSTATUS TSS ON TSS.NB = TSP.PSTATUS "
     + " WHERE 1 = 1 ";



            var NB = Request.Form["NB"].Trim();
            var sNB = Request.Form["sNB"].Trim();
            var sNO = Request.Form["sNO"].Trim();
            var sDATESTART = Request.Form["sDATESTART"].Trim();
            var sDATEEND = Request.Form["sDATEEND"].Trim();
            var SesStatus = Request.Form["sSTATUS"].Trim();
            var SPstatus = Request.Form["SPstatus"].Trim();
            var SProcedtyps = Request.Form["SProcedtyps"].Trim();
            var COMCITYNB = Request.Form["COMCITYNB"].Trim();


            if (NB != "")
            {
                sql += " and TSE.COMMITTEENB = " + NB;
            }
            if (sNB != "")
            {
                sql += " and TSE.NB =" + sNB;
            }

            if (sNO != "")
            {
                sql += " and  TSE.SESNO =" + sNO;
            }

            if (SesStatus != "")
            {
                sql += " and  TSE.STATUS =" + SesStatus;
            }

            if (sDATESTART != "")
            {
                sql += " and TSE.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            }

            if (sDATEEND != "")
            {
                sql += " and TSE.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
            }

            if (SPstatus != "")
            {
                sql += " and TSP.PSTATUS =" + SPstatus;
            }

            if (SProcedtyps != "")
            {
                sql += " and CP.PROCEDNB =" + SProcedtyps;
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and TSE.SESCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and TSE.SESCITYNB =" + ci;
            }

            sql += " ORDER BY TSE.NB";

            var data = db.Database.SqlQuery<ViewModel.SessionsAndProcedsVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult SessionsAndProceds_PDF(string pNB, string psNB, string psNO, string psDATESTART, string psDATEEND, string psSTATUS, string SPstatus, string SProcedtyps, string pCOMCITYNB)
        {
            string sql = "SELECT ZC.NAME  AS CityName, "
     + " TSE.COMMITTEENB AS CommNb, "
      + " TSE.NB AS SessNb, "
      + " TSE.SESNO AS SessNo, "
      + " TSE.SESDATE AS SessDate, "
      + " TS.NAME AS SessStatus, "
      + " PT.NAME AS ProcedName, "
      + " TSS.NAME AS ProcedRes "
     + " FROM TRSESSIONS_PROCEDS TSP "
     + " JOIN TRSESSIONS TSE ON TSE.NB = TSP.SESSIONNB "
     + " JOIN ZCITYS ZC ON ZC.NB = TSE.SESCITYNB "
     + " JOIN TRSTATUS TS ON TS.NB = TSE.STATUS "
     + " JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB "
     + " JOIN ZPROCEDTYPS PT ON PT.NB = CP.PROCEDNB "
     + " JOIN TRSESSIONSPROCEDSTATUS TSS ON TSS.NB = TSP.PSTATUS "
    + " WHERE 1 = 1 ";


            var NB = pNB;
            var sNB = psNB;
            var sNO = psNO;
            var sDATESTART = psDATESTART;
            var sDATEEND = psDATEEND;
            var SesStatus = psSTATUS;

            var COMCITYNB = pCOMCITYNB;
            var Pstatus = SPstatus;
            var Procedtyps = SProcedtyps;



            if (NB != "")
            {
                sql += " and TSE.COMMITTEENB = " + NB;
            }
            if (sNB != "")
            {
                sql += " and TSE.NB =" + sNB;
            }

            if (sNO != "")
            {
                sql += " and  TSE.SESNO =" + sNO;
            }

            if (SesStatus != "")
            {
                sql += " and  TSE.STATUS =" + SesStatus;
            }

            if (sDATESTART != "")
            {
                sql += " and TSE.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            }

            if (sDATEEND != "")
            {
                sql += " and TSE.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
            }

            if (Pstatus != "")
            {
                sql += " and TSP.PSTATUS =" + Pstatus;
            }

            if (Procedtyps != "")
            {
                sql += " and CP.PROCEDNB =" + Procedtyps;
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (COMCITYNB != "")
                {
                    sql += " and TSE.SESCITYNB =" + COMCITYNB;
                }

            }
            else
            {
                sql += " and TSE.SESCITYNB =" + ci;
            }

            sql += " ORDER BY TSE.NB";

            var data = db.Database.SqlQuery<ViewModel.SessionsAndProcedsVM>(sql);
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };
        }



        public ActionResult CityAndLines()
        {
            return View();
        }
        public ActionResult CityAndLines_Read([DataSourceRequest] DataSourceRequest request)
        {
            var sql = " SELECT ZC.NAME AS CITYNAME, "
                 + " TR.NAME AS LINENAME, "
                 + "   TRT.NAME AS TPYS, "
                 + "   CASE WHEN TR.STATUS = 1 THEN 'فعال' ELSE 'غير فعال' END AS STATUS, "
                 + "   CASE WHEN TR.ISCANCELD = 1 THEN 'ملغى' ELSE 'غير ملغى' END AS ISCANCELD, "
                 + "   TR.MINCARS AS MINCARS, "
                 + "  TR.MAXCARS AS MAXCARS "
                 + "  FROM TRLINES TR "
                 + "    LEFT JOIN ZCITYS ZC ON ZC.NB = TR.CITYNB "
                 + "  JOIN ZTRLINETYPES TRT ON TRT.NB = TR.TYP where 1 = 1 ";

            var STrName = Request.Form["STrline"].Trim();
            var StrlineStatus = Request.Form["StrlineStatus"].Trim();
            var StrlineCANCELD = Request.Form["StrlineCANCELD"].Trim();
            var Slinetyps = Request.Form["Slinetyps"].Trim();
            var SlineCity = Request.Form["SlineCity"].Trim();


            if (STrName != "")
            {
                sql += " and TR.NAME like '%" + STrName + "%' ";
            }
            if (StrlineStatus != "")
            {
                sql += " and TR.STATUS =" + StrlineStatus;
            }
            if (StrlineCANCELD != "")
            {
                sql += " and nvl(TR.ISCANCELD,0) =" + StrlineCANCELD;
            }
            if (Slinetyps != "")
            {
                sql += " and TR.TYP =" + Slinetyps;
            }
            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql += " and TR.CITYNB =" + SlineCity;
                }

            }
            else
            {
                sql += " and TR.CITYNB =" + ci;
            }
            var data = db.Database.SqlQuery<ViewModel.CityAndLinesVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CityAndLines_PDF(string pSTrName, string pStrlineStatus, string pStrlineCANCELD, string pSlinetyps, string pSlineCity)
        {
            var sql = " SELECT rownum as rr,  ZC.NAME AS CITYNAME, "
                 + " TR.NAME AS LINENAME, "
                 + "   TRT.NAME AS TPYS, "
                 + "   CASE WHEN TR.STATUS = 1 THEN 'فعال' ELSE 'غير فعال' END AS STATUS, "
                 + "   CASE WHEN TR.ISCANCELD = 1 THEN 'ملغى' ELSE 'غير ملغى' END AS ISCANCELD, "
                 + "   TR.MINCARS AS MINCARS, "
                 + "  TR.MAXCARS AS MAXCARS "
                 + "  FROM TRLINES TR "
                 + "    LEFT JOIN ZCITYS ZC ON ZC.NB = TR.CITYNB "
                 + "  JOIN ZTRLINETYPES TRT ON TRT.NB = TR.TYP where 1 = 1 ";

            var STrName = pSTrName;
            var StrlineStatus = pStrlineStatus;
            var StrlineCANCELD = pStrlineCANCELD;
            var Slinetyps = pSlinetyps;
            var SlineCity = pSlineCity;


            if (STrName != "")
            {
                sql += " and TR.NAME like '%" + STrName + "%' ";
            }
            if (StrlineStatus != "")
            {
                sql += " and TR.STATUS =" + StrlineStatus;
            }
            if (StrlineCANCELD != "")
            {
                sql += " and nvl(TR.ISCANCELD,0) =" + StrlineCANCELD;
            }
            if (Slinetyps != "")
            {
                sql += " and TR.TYP =" + Slinetyps;
            }
            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql += " and TR.CITYNB =" + SlineCity;
                }

            }
            else
            {
                sql += " and TR.CITYNB =" + ci;
            }
            var data = db.Database.SqlQuery<ViewModel.CityAndLinesVM>(sql);
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };
        }

        public ActionResult CityAndLinesChange()
        {
            return View();
        }


        public ActionResult LinesAndCarsGroup()
        {
            return View();
        }
        public ActionResult LinesAndCarsGroup_Read([DataSourceRequest] DataSourceRequest request)
        {

            var SlineCity_checkbox = Request.Form["SlineCity_checkbox"].Trim();
            var Slinetyps_checkbox = Request.Form["Slinetyps_checkbox"].Trim();
            var StrlineStatus_checkbox = Request.Form["StrlineStatus_checkbox"].Trim();
            var Scarkind_checkbox = Request.Form["Scarkind_checkbox"].Trim();
            var Scarreg_checkbox = Request.Form["Scarreg_checkbox"].Trim();
            var Strname_checkbox = Request.Form["Strname_checkbox"].Trim();

            var Slinetyps = Request.Form["Slinetyps"].Trim();
            var StrlineStatus = Request.Form["StrlineStatus"].Trim();
            var Scarkind = Request.Form["Scarkind"].Trim();
            var Scarreg = Request.Form["Scarreg"].Trim();

            var STrline = Request.Form["STrline"].Trim();
            var SlineCity = Request.Form["SlineCity"].Trim();




            List<string> grossupby = new List<string>();

            var sql1 = " select ";

            if (SlineCity_checkbox == "true")
            {
                sql1 += " ZC.NAME AS CityName ,";
                grossupby.Add("ZC.NAME");
            }
            if (Slinetyps_checkbox == "true")
            {
                sql1 += " try.name AS TYPS ,";
                grossupby.Add("try.name");

            }
            if (StrlineStatus_checkbox == "true")
            {
                sql1 += " CASE WHEN TR.STATUS = 1 THEN 'فعال' ELSE 'غير فعال' END AS  STATUS , ";
                grossupby.Add("TR.STATUS ");

            }
            if (Scarkind_checkbox == "true")
            {
                sql1 += " zk.name as kind ,";
                grossupby.Add("zk.name");

            }
            if (Scarreg_checkbox == "true")
            {
                sql1 += " zg.name as reg ,";
                grossupby.Add("zg.name");

            }
            if (Strname_checkbox == "true")
            {
                sql1 += " TR.name as trname ,";
                grossupby.Add("TR.NAME");

            }


            sql1 += " COUNT (CA.NB) AS CoutnCar ";

            sql1 += "  FROM TRLINES TR  "
             + "  left JOIN ZCITYS ZC ON ZC.NB = TR.CITYNB "
             + "  left join ZTRLINETYPES try on try.nb = tr.TYP "
             + "  left JOIN cars ca ON tr.nb = ca.lin  "
             + "  left join zcarkinds zk on zk.nb = ca.reg "
             + "  left join ZCARREGS zg on ca.CARREGNB = zg.nb where 1=1 ";

            if (Slinetyps != "")
            {
                sql1 += " and tr.TYP = " + Slinetyps;
            }
            if (StrlineStatus != "")
            {
                sql1 += " and TR.STATUS = " + StrlineStatus;
            }


            if (Scarkind != "")
            {
                sql1 += " and ca.reg = " + Scarkind;
            }

            if (Scarreg != "")
            {
                sql1 += " and ca.CARREGNB = " + Scarreg;
            }

            if (STrline != "")
            {
                sql1 += " and tr.name like  '%" + STrline + "%' ";
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql1 += " and TR.CITYNB =" + SlineCity;
                }

            }
            else
            {
                sql1 += " and TR.CITYNB =" + ci;
            }

            if (SlineCity_checkbox == "true" ||
                Slinetyps_checkbox == "true" ||
                StrlineStatus_checkbox == "true" ||
                Scarkind_checkbox == "true" ||
                Scarreg_checkbox == "true" ||
                Strname_checkbox == "true"
                )
            {
                sql1 += " GROUP BY ";
            }

            for (var i = 0; i <= grossupby.Count() - 1; i++)
            {
                sql1 += grossupby[i];
                if (i != grossupby.Count() - 1)
                {
                    sql1 += " , ";
                }
            }

            if (SlineCity_checkbox == "true" ||
               Slinetyps_checkbox == "true" ||
               StrlineStatus_checkbox == "true" ||
               Scarkind_checkbox == "true" ||
               Scarreg_checkbox == "true" ||
               Strname_checkbox == "true"
               )
            {
                sql1 += " ORDER BY ";
            }
            for (var i = 0; i <= grossupby.Count() - 1; i++)
            {
                sql1 += grossupby[i];
                if (i != grossupby.Count() - 1)
                {
                    sql1 += " , ";
                }
            }


            var data = db.Database.SqlQuery<ViewModel.LinesAndCarsGroupVM>(sql1);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult LinesAndCarsGroup_PDF
            (string SlineCitycheckbox, string Slinetypscheckbox, string StrlineStatuscheckbox, string Scarkindcheckbox, string Scarregcheckbox, string Strnamecheckbox, string pSlinetyps, string pStrlineStatus, string pScarkind, string pScarreg, string pSTrline, string pSlineCity)
        {
            var SlineCity_checkbox = SlineCitycheckbox;
            var Slinetyps_checkbox = Slinetypscheckbox;
            var StrlineStatus_checkbox = StrlineStatuscheckbox;

            var Scarkind_checkbox = Scarkindcheckbox;
            var Scarreg_checkbox = Scarregcheckbox;
            var Strname_checkbox = Strnamecheckbox;

            var Slinetyps = pSlinetyps;
            var StrlineStatus = pStrlineStatus;
            var Scarkind = pScarkind;
            var Scarreg = pScarreg;

            var STrline = pSTrline;
            var SlineCity = pSlineCity;




            List<string> grossupby = new List<string>();

            var sql1 = " select ";

            if (SlineCity_checkbox == "true")
            {
                sql1 += " ZC.NAME AS CityName ,";
                grossupby.Add("ZC.NAME");
            }
            if (Slinetyps_checkbox == "true")
            {
                sql1 += " try.name AS TYPS ,";
                grossupby.Add("try.name");

            }
            if (StrlineStatus_checkbox == "true")
            {
                sql1 += " CASE WHEN TR.STATUS = 1 THEN 'فعال' ELSE 'غير فعال' END AS  STATUS , ";
                grossupby.Add("TR.STATUS ");

            }
            if (Scarkind_checkbox == "true")
            {
                sql1 += " zk.name as kind ,";
                grossupby.Add("zk.name");

            }
            if (Scarreg_checkbox == "true")
            {
                sql1 += " zg.name as reg ,";
                grossupby.Add("zg.name");

            }
            if (Strname_checkbox == "true")
            {
                sql1 += " TR.name as trname ,";
                grossupby.Add("TR.NAME");

            }


            sql1 += " COUNT (CA.NB) AS CoutnCar ";

            sql1 += "  FROM TRLINES TR  "
             + "  left JOIN ZCITYS ZC ON ZC.NB = TR.CITYNB "
             + "  left join ZTRLINETYPES try on try.nb = tr.TYP "
             + "  left JOIN cars ca ON tr.nb = ca.lin  "
             + "  left join zcarkinds zk on zk.nb = ca.reg "
             + "  left join ZCARREGS zg on ca.CARREGNB = zg.nb where 1=1 ";

            if (Slinetyps != "")
            {
                sql1 += " and tr.TYP = " + Slinetyps;
            }
            if (StrlineStatus != "")
            {
                sql1 += " and TR.STATUS = " + StrlineStatus;
            }


            if (Scarkind != "")
            {
                sql1 += " and ca.reg = " + Scarkind;
            }

            if (Scarreg != "")
            {
                sql1 += " and ca.CARREGNB = " + Scarreg;
            }

            if (STrline != "")
            {
                sql1 += " and tr.name like  '%" + STrline + "%' ";
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql1 += " and TR.CITYNB =" + SlineCity;
                }

            }
            else
            {
                sql1 += " and TR.CITYNB =" + ci;
            }

            if (SlineCity_checkbox == "true" ||
                Slinetyps_checkbox == "true" ||
                StrlineStatus_checkbox == "true" ||
                Scarkind_checkbox == "true" ||
                Scarreg_checkbox == "true" ||
                Strname_checkbox == "true"
                )
            {
                sql1 += " GROUP BY ";
            }

            for (var i = 0; i <= grossupby.Count() - 1; i++)
            {
                sql1 += grossupby[i];
                if (i != grossupby.Count() - 1)
                {
                    sql1 += " , ";
                }
            }

            if (SlineCity_checkbox == "true" ||
               Slinetyps_checkbox == "true" ||
               StrlineStatus_checkbox == "true" ||
               Scarkind_checkbox == "true" ||
               Scarreg_checkbox == "true" ||
               Strname_checkbox == "true"
               )
            {
                sql1 += " ORDER BY ";
            }
            for (var i = 0; i <= grossupby.Count() - 1; i++)
            {
                sql1 += grossupby[i];
                if (i != grossupby.Count() - 1)
                {
                    sql1 += " , ";
                }
            }

            var data = db.Database.SqlQuery<ViewModel.LinesAndCarsGroupVM>(sql1).ToList();
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };
        }

        public ActionResult LinesAndCarsDetalis()
        {
            ViewData["ZCARREGS"] = db.ZCARREGS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["ZCARKINDS"] = db.ZCARKINDS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            return View();
        }
        public ActionResult LinesAndCarsDetalis_Read([DataSourceRequest] DataSourceRequest request)
        {


            var Slinetyps = Request.Form["Slinetyps"].Trim();
            var StrlineStatus = Request.Form["StrlineStatus"].Trim();
            var Scarkind = Request.Form["Scarkind"].Trim();
            var Scarreg = Request.Form["Scarreg"].Trim();

            var STrline = Request.Form["STrline"].Trim();
            var SlineCity = Request.Form["SlineCity"].Trim();






            var sql1 = " SELECT ZC.NAME AS CityName,"
      + " TR.NAME AS trname,"
      + " try.NAME AS TYPS,"
      + " CASE WHEN TR.STATUS = 1 THEN 'فعال' ELSE 'غير فعال'  END     AS STATUS,"
      + " CA.NB AS carnb,"
      + " CA.TABNU AS tabnu, "
      + " ca.reg AS kind,"
      + " ca.CARREGNB AS reg,"
      + " CA.CITYNB AS carcity"
      + " FROM TRLINES TR"
      + " LEFT JOIN ZCITYS ZC ON ZC.NB = TR.CITYNB"
      + " LEFT JOIN ZTRLINETYPES try ON try.nb = tr.TYP"
      + "  LEFT JOIN cars ca ON tr.nb = ca.lin"
      + "  LEFT JOIN zcarkinds zk ON zk.nb = ca.reg"
      + "  LEFT JOIN ZCARREGS zg ON ca.CARREGNB = zg.nb"
      + " WHERE 1 = 1 ";

            if (Slinetyps != "")
            {
                sql1 += " and tr.TYP = " + Slinetyps;
            }
            if (StrlineStatus != "")
            {
                sql1 += " and TR.STATUS = " + StrlineStatus;
            }


            if (Scarkind != "")
            {
                sql1 += " and ca.reg = " + Scarkind;
            }

            if (Scarreg != "")
            {
                sql1 += " and ca.CARREGNB = " + Scarreg;
            }

            if (STrline != "")
            {
                sql1 += " and tr.name like  '%" + STrline + "%' ";
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql1 += " and TR.CITYNB =" + SlineCity;
                }

            }
            else
            {
                sql1 += " and TR.CITYNB =" + ci;
            }


            var data = db.Database.SqlQuery<ViewModel.LinesAndCarsDetalisVM>(sql1);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult LinesAndCarsDetalis_PDF(string pSlinetyps, string pStrlineStatus, string pScarkind, string pScarreg, string pSTrline, string pSlineCity)
        {
            var Slinetyps = pSlinetyps;
            var StrlineStatus = pStrlineStatus;
            var Scarkind = pScarkind;
            var Scarreg = pScarreg;
            var STrline = pSTrline;
            var SlineCity = pSlineCity;

            var sql1 = " SELECT ZC.NAME AS CityName,"
      + " TR.NAME AS trname,"
      + " try.NAME AS TYPS,"
      + " CASE WHEN TR.STATUS = 1 THEN 'فعال' ELSE 'غير فعال'  END     AS STATUS,"
      + " CA.NB AS carnb,"
      + " CA.TABNU AS tabnu, "
      + " zk.NAME  AS kind,"
      + " zg.NAME  AS reg,"
      + " ZC2.NAME AS carcity"
      + " FROM TRLINES TR"
      + " LEFT JOIN ZCITYS ZC ON ZC.NB = TR.CITYNB"
      + " LEFT JOIN ZTRLINETYPES try ON try.nb = tr.TYP"
      + "   JOIN cars ca ON tr.nb = ca.lin"
      + "  LEFT JOIN zcarkinds zk ON zk.nb = ca.reg"
      + "  LEFT JOIN ZCARREGS zg ON ca.CARREGNB = zg.nb"
       + " LEFT JOIN ZCITYS ZC2 ON ZC2.NB = CA.CITYNB"
      + " WHERE 1 = 1 ";

            if (Slinetyps != "")
            {
                sql1 += " and tr.TYP = " + Slinetyps;
            }
            if (StrlineStatus != "")
            {
                sql1 += " and TR.STATUS = " + StrlineStatus;
            }


            if (Scarkind != "")
            {
                sql1 += " and ca.reg = " + Scarkind;
            }

            if (Scarreg != "")
            {
                sql1 += " and ca.CARREGNB = " + Scarreg;
            }

            if (STrline != "")
            {
                sql1 += " and tr.name like  '%" + STrline + "%' ";
            }

            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
                if (SlineCity != "")
                {
                    sql1 += " and TR.CITYNB =" + SlineCity;
                }

            }
            else
            {
                sql1 += " and TR.CITYNB =" + ci;
            }


            var data = db.Database.SqlQuery<ViewModel.LinesAndCarsDetalisPDFVM>(sql1);
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };

        }

        public ActionResult CarsAndProceds()
        {
            ViewData["ZCARREGS"] = db.ZCARREGS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });
            ViewData["ZCARKINDS"] = db.ZCARKINDS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            return View();
        }


        public ActionResult CarsAndProceds_Read([DataSourceRequest] DataSourceRequest request)
        { 
            var Carnb = Request.Form["Carnb"].Trim();
            var Tabnu = Request.Form["Tabnu"].Trim();
            var Carcity = Request.Form["Carcity"].Trim();
            var Scarkind = Request.Form["Scarkind"].Trim();
            var Scarreg = Request.Form["Scarreg"].Trim();
            var SProcedtyps = Request.Form["SProcedtyps"].Trim();
            var Carprocednb = Request.Form["Carprocednb"].Trim();
            var SCarprocedDateStart = Request.Form["SCarprocedDateStart"].Trim();
            var SCarprocedDateEnd = Request.Form["SCarprocedDateEnd"].Trim();

            var sql1 = " SELECT CA.NB AS carnb, "
      + "  CA.TABNU AS tabnu, "
        + "  zk.NAME AS kind, "
        + "  zg.NAME AS reg, "
        + "  ZC2.NAME AS carcity, "
        + "  CP.NB AS CARPROCEDNB, "
        + "  ZTP.NAME AS PROCEDNAME, "
        + "  CP.RECDAT AS PROCEDDATE, "
       + "   CP.RESULT AS RESULT,  "
       + "   ZC.NAME AS STEPCITY, "
        + "  CPS.NB, "
        + "  CASE WHEN STEPNB = 454 THEN 'نهائية' WHEN STEPNB = 453 THEN 'مبدئية' END AS TYPSNAMEAGR, "
      + "    (SELECT 'نعم' FROM TRAGREEMENTS WHERE CARPROCEDNB = CP.NB) AS PROPSTATUS "
  + "   FROM CARPROCEDS  CP "
     + "     JOIN ZPROCEDTYPS ZTP ON ZTP.NB = CP.PROCEDNB "
      + "    JOIN CARS CA ON CP.CARNB = CA.NB "
      + "    LEFT JOIN zcarkinds zk ON zk.nb = ca.reg "
      + "    LEFT JOIN ZCARREGS zg ON ca.CARREGNB = zg.nb "
      + "    LEFT JOIN ZCITYS ZC2 ON ZC2.NB = CA.CITYNB "
      + "    LEFT JOIN CARPROCEDSTEPS CPS ON CPS.CARPROCEDNB = CP.NB "
      + "    LEFT JOIN ZCITYS ZC ON ZC.NB = CPS.CITYNB "
  + "  WHERE STEPNB IN(454, 453) ";

            if (Carnb != "")
            {
                sql1 += " and CA.NB = " + Carnb;
            }

            if (Tabnu != "")
            {
                sql1 += " and  CA.TABNU = " + Tabnu;
            }
         

            if (Scarkind != "")
            {
                sql1 += " and  ca.reg = " + Scarkind;
            }

            if (Scarreg != "")
            {
                sql1 += " and  ca.CARREGNB = " + Scarreg;
            }

            if (SProcedtyps != "")
            {
                sql1 += " and  CP.PROCEDNB = " + SProcedtyps;
            }

            if (Carprocednb != "")
            {
                sql1 += " and  cp.nb = " + Carprocednb;
            }

            if (SCarprocedDateStart != "")
            {
                sql1 += " and CP.RECDAT >= TO_DATE('" + SCarprocedDateStart + "','DD/MM/YYYY') ";
            }

            if (SCarprocedDateEnd != "")
            {
                sql1 += " and CP.RECDAT <= TO_DATE('" + SCarprocedDateEnd + "','DD/MM/YYYY') ";
            }

            if (Carcity != "")
            {
                sql1 += " and  CA.CITYNB = " + Carcity;
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {
            
            }
            else
            {
                sql1 += " and  CPS.CITYNB =" + ci;
            }

            sql1 += " ORDER BY CARPROCEDNB , CPS.NB ";
            var data = db.Database.SqlQuery<ViewModel.CarsAndProcedsVM>(sql1);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult CarsAndProceds_PDF(
            string pCarnb,
            string pTabnu,
            string pCarcity,
            string pScarkind,
            string pScarreg,
            string pSProcedtyps,
            string pCarprocednb,
            string pSCarprocedDateStart,
            string pSCarprocedDateEnd

            )
        {
            var Carnb = pCarnb;
            var Tabnu = pTabnu;
            var Carcity = pCarcity;
            var Scarkind = pScarkind;
            var Scarreg = pScarreg;
            var SProcedtyps = pSProcedtyps;
            var Carprocednb = pCarprocednb;
            var SCarprocedDateStart = pSCarprocedDateStart;
            var SCarprocedDateEnd = pSCarprocedDateEnd;

            var sql1 = " SELECT CA.NB AS carnb, "
       + "  CA.TABNU AS tabnu, "
         + "  zk.NAME AS kind, "
         + "  zg.NAME AS reg, "
         + "  ZC2.NAME AS carcity, "
         + "  CP.NB AS CARPROCEDNB, "
         + "  ZTP.NAME AS PROCEDNAME, "
         + "  CP.RECDAT AS PROCEDDATE, "
        + "   CP.RESULT AS RESULT,  "
        + "   ZC.NAME AS STEPCITY, "
         + "  CPS.NB, "
         + "  CASE WHEN STEPNB = 454 THEN 'نهائية' WHEN STEPNB = 453 THEN 'مبدئية' END AS TYPSNAMEAGR, "
       + "    (SELECT 'نعم' FROM TRAGREEMENTS WHERE CARPROCEDNB = CP.NB) AS PROPSTATUS "
   + "   FROM CARPROCEDS  CP "
      + "     JOIN ZPROCEDTYPS ZTP ON ZTP.NB = CP.PROCEDNB "
       + "    JOIN CARS CA ON CP.CARNB = CA.NB "
       + "    LEFT JOIN zcarkinds zk ON zk.nb = ca.reg "
       + "    LEFT JOIN ZCARREGS zg ON ca.CARREGNB = zg.nb "
       + "    LEFT JOIN ZCITYS ZC2 ON ZC2.NB = CA.CITYNB "
       + "    LEFT JOIN CARPROCEDSTEPS CPS ON CPS.CARPROCEDNB = CP.NB "
       + "    LEFT JOIN ZCITYS ZC ON ZC.NB = CPS.CITYNB "
   + "  WHERE STEPNB IN(454, 453) ";

            if (Carnb != "")
            {
                sql1 += " and CA.NB = " + Carnb;
            }

            if (Tabnu != "")
            {
                sql1 += " and  CA.TABNU = " + Tabnu;
            }


            if (Scarkind != "")
            {
                sql1 += " and  ca.reg = " + Scarkind;
            }

            if (Scarreg != "")
            {
                sql1 += " and  ca.CARREGNB = " + Scarreg;
            }

            if (SProcedtyps != "")
            {
                sql1 += " and  CP.PROCEDNB = " + SProcedtyps;
            }

            if (Carprocednb != "")
            {
                sql1 += " and  cp.nb = " + Carprocednb;
            }

            if (SCarprocedDateStart != "")
            {
                sql1 += " and CP.RECDAT >= TO_DATE('" + SCarprocedDateStart + "','DD/MM/YYYY') ";
            }

            if (SCarprocedDateEnd != "")
            {
                sql1 += " and CP.RECDAT <= TO_DATE('" + SCarprocedDateEnd + "','DD/MM/YYYY') ";
            }

            if (Carcity != "")
            {
                sql1 += " and  CA.CITYNB = " + Carcity;
            }


            CodesController bb = new CodesController();

            var ci = bb.GetCityForRead();

            if (ci == "0")
            {

            }
            else
            {
                sql1 += " and  CPS.CITYNB =" + ci;
            }


            sql1 += " ORDER BY CARPROCEDNB , CPS.NB ";

            var data = db.Database.SqlQuery<ViewModel.CarsAndProcedsVM>(sql1);
            return new ViewAsPdf(data)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8"
            };


        }



        public ActionResult TRCHANGECARLINES()
        {
            ViewData["ZCARREGS"] = db.ZCARREGS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["zcities"] = db.ZCITYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

            ViewData["ZCARCATEGORYS"] = db.ZCARCATEGORYS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME
            });

          
            return View();
        }

        public ActionResult TRCHANGECARLINES_Read([DataSourceRequest] DataSourceRequest request)
        {
            string sql = "select * from TRCHANGE_CAR_LINES where 1= 1 ";

            //var NB = Request.Form["NB"].Trim();
            //var COMNO = Request.Form["COMNO"].Trim();
            //var COMDATESTART = Request.Form["COMDATESTART"].Trim();
            //var COMDATEEND = Request.Form["COMDATEEND"].Trim();
            //var STATUS = Request.Form["STATUS"].Trim();
            //var MEMBERNAME = Request.Form["MEMBERNAME"].Trim();
            //var MEMBERPOSITIONNB = Request.Form["MEMBERPOSITIONNB"].Trim();
            //var MEMBERSHIPNB = Request.Form["MEMBERSHIPNB"].Trim();
            //var COMCITYNB = Request.Form["COMCITYNB"].Trim();


            //if (NB != "")
            //{
            //    sql += " and TM.NB = " + NB;
            //}
            //if (COMNO != "")
            //{
            //    sql += " and TM.COMNO like '%" + COMNO + "%'";
            //}
            //if (STATUS != "")
            //{
            //    sql += " and TM.STATUS =" + STATUS;
            //}
            //if (COMDATESTART != "")
            //{
            //    sql += " and TM.COMDATE >= TO_DATE('" + COMDATESTART + "','DD/MM/YYYY') ";
            //}

            //if (COMDATEEND != "")
            //{
            //    sql += " and TM.COMDATE <= TO_DATE('" + COMDATEEND + "','DD/MM/YYYY') ";
            //}


            //CodesController bb = new CodesController();

            //var ci = bb.GetCityForRead();

            //if (ci == "0")
            //{
            //    if (COMCITYNB != "")
            //    {
            //        sql += " and TM.COMCITYNB =" + COMCITYNB;
            //    }

            //}
            //else
            //{
            //    sql += " and TM.COMCITYNB =" + ci;
            //}
            //if (MEMBERPOSITIONNB != "")
            //{
            //    sql += " and TMM.MEMBERPOSITIONNB =" + MEMBERPOSITIONNB;
            //}

            //if (MEMBERSHIPNB != "")
            //{
            //    sql += " and TMM.MEMBERSHIPNB =" + MEMBERSHIPNB;
            //}
            //if (MEMBERNAME != "")
            //{
            //    sql += " and TMM.MEMBERNAME LIKE '%" + MEMBERNAME + "%' ";
            //}
            var data = db.Database.SqlQuery<ViewModel.TRCHANGE_CAR_LINESVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
