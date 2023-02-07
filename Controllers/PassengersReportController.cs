using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Proced.DataAccess.Models.CF;
using Rotativa;
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



        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }

        public FileResult CommittesAndSessions_ex()
        {

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("CustomerId"),
                                            new DataColumn("ContactName"),
                                            new DataColumn("City"),
                                            new DataColumn("Country") });
            string query = "SELECT ZC.NAME AS CityName,"
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
            + " JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB "
            + " JOIN TRSESSIONS TCS ON TCS.COMMITTEENB = TM.NB "
            + " JOIN TRSTATUS TS2 ON TS2.NB = TCS.STATUS "
            + " WHERE 1 = 1 AND TMM.MEMBERSHIPNB = 1 ";
            var customers = db.Database.SqlQuery<ViewModel.CommittesAndSessions>(query);


            foreach (var customer in customers)
            {
                dt.Rows.Add(customer.SessNo, customer.SessDate, customer.CityName, customer.BossName);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }

            //       OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ModelCF"].ConnectionString);
            //       conn.Open();
            //       DataTable Table_hor = new DataTable();
            //       DataTable table_ver = new DataTable();
            //       string query = "SELECT ZC.NAME AS CityName,"
            //       + " TM.NB AS CommNb, "
            //       + " TM.COMNO AS CommNo, "
            //+ " TM.COMDATE AS CommDate, "
            //+ " TS.NAME AS CommStatus, "
            //+ " TCS.NB AS SessNb, "
            //+ " TCS.SESNO AS SessNo, "
            //+ " TCS.SESDATE AS SessDate, "
            //+ " TS2.NAME AS SessStatus, "
            //+ " TMM.MEMBERNAME AS BossName, "
            //+ " ZP.NAME AS BossPostion "
            //+ " FROM TRCOMMITTEES TM "
            //+ " JOIN TRCOMMITTEES_MEMBERS TMM ON TMM.COMMITTEENB = TM.NB "
            //+ " JOIN ZCITYS ZC ON ZC.NB = TM.COMCITYNB "
            //+ " JOIN TRSTATUS TS ON TS.NB = TM.STATUS "
            //+ "  JOIN TRZMEMBERPOSITION ZP ON ZP.NB = TMM.MEMBERPOSITIONNB "
            //+ " JOIN TRSESSIONS TCS ON TCS.COMMITTEENB = TM.NB "
            //+ " JOIN TRSTATUS TS2 ON TS2.NB = TCS.STATUS "
            //+ " WHERE 1 = 1 AND TMM.MEMBERSHIPNB = 1 ";
            //       //var query = "SELECT GROUP_NB, SS.SSVAL('TGROUPS', 'NAME', GROUP_NB, 'NB') TGROUPS FROM TAXES WHERE GROUP_NB IS NOT NULL GROUP BY GROUP_NB ";
            //       OracleCommand cmd_hor = new OracleCommand(query, conn);
            //       OracleDataReader dr_hor = cmd_hor.ExecuteReader();
            //       Table_hor.Load(dr_hor);
            //       double count = 1;


            //       //Table_hor.Load(dr_hor);


            //       for (int i = 0; i < Table_hor.Rows.Count; i++)
            //       {
            //           string hutt1 = Table_hor.Rows[i][Table_hor.Columns[1]].ToString();

            //           hutt1 = hutt1.Replace("(", "_");
            //           hutt1 = hutt1.Replace(")", "_");
            //           hutt1 = hutt1.Replace(".", "_");
            //           hutt1 = hutt1.Replace("+", "_");
            //           hutt1 = hutt1.Replace(",", "_");
            //           hutt1 = hutt1.Replace(";", "_");
            //           hutt1 = hutt1.Replace("-", "_");
            //           hutt1 = hutt1.Replace("&", "_");
            //           hutt1 = hutt1.Replace("?", "_");
            //           hutt1 = hutt1.Replace("!", "_");
            //           hutt1 = hutt1.Replace("/", "_");
            //           hutt1 = hutt1.Replace("%", "_");
            //           hutt1.Replace(" ", "_");
            //           hutt1 = hutt1.Trim();
            //           string hutt = Table_hor.Rows[i][Table_hor.Columns[0]].ToString();

            //       }



            //       OracleCommand cmd = new OracleCommand(query, conn);

            //       OracleDataReader dr = cmd.ExecuteReader();

            //       table_ver.Load(dr);
            //       cmd.Dispose();
            //       conn.Close(); conn.Dispose();


            //       ViewData["PageCnt"] = count;
            //       ViewData["dt"] = Table_hor;
            //       ViewData["dt2"] = table_ver;

            //       //var v = View(table_ver);
            //       XLWorkbook wb = new XLWorkbook();
            //       wb.Worksheets.Add(table_ver, "الرسوم");
            //       wb.Worksheets.FirstOrDefault()?.Row(1).InsertRowsAbove(1);
            //       wb.Worksheets.FirstOrDefault()?.Row(1).InsertRowsAbove(1);
            //       wb.Worksheets.FirstOrDefault()?.Row(1).InsertRowsAbove(1);
            //       wb.Worksheets.FirstOrDefault()?.Cell(1, 1)?.SetValue("الجمهورية العربية السورية");
            //       wb.Worksheets.FirstOrDefault()?.Cell(2, 1)?.SetValue("وزارة النقل");
            //       if (ViewBag.FromDateText != null)
            //       {
            //           wb.Worksheets.FirstOrDefault()?.Cell(3, 1)?.SetValue("من تاريخ");
            //           wb.Worksheets.FirstOrDefault()?.Cell(3, 2)?.SetValue(ViewBag.FromDateText);
            //       }
            //       if (ViewBag.ToDateText != null)
            //       {
            //           wb.Worksheets.FirstOrDefault()?.Cell(3, 5)?.SetValue("إلى تاريخ");
            //           wb.Worksheets.FirstOrDefault()?.Cell(2, 6)?.SetValue(ViewBag.ToDateText);
            //       }
            //       MemoryStream stream = GetStream(wb);

            //       string myName = "التقرير_التفصيلي_للرسوم.xlsx";
            //       Response.Clear();
            //       Response.Buffer = true;
            //       Response.AddHeader("content-disposition", "attachment; filename=" + myName);
            //       Response.ContentType = "application/vnd.ms-excel";
            //       Response.BinaryWrite(stream.ToArray());
            //       Response.End();
            //       return null;

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
       +" TSE.NB AS SessNb, "
       +" TSE.SESNO AS SessNo, "
       +" TSE.SESDATE AS SessDate, "
       +" TS.NAME AS SessStatus, "
       +" PT.NAME AS ProcedName, "
       +" TSS.NAME AS ProcedRes "
      + " FROM TRSESSIONS_PROCEDS TSP "
      + " JOIN TRSESSIONS TSE ON TSE.NB = TSP.SESSIONNB "
      + " JOIN ZCITYS ZC ON ZC.NB = TSE.SESCITYNB "
      + " JOIN TRSTATUS TS ON TS.NB = TSE.STATUS "
      + " JOIN CARPROCEDS CP ON CP.NB = TSP.CARPROCEDNB "
      + " JOIN ZPROCEDTYPS PT ON PT.NB = CP.PROCEDNB "
      + " JOIN TRSESSIONSPROCEDSTATUS TSS ON TSS.NB = TSP.PSTATUS "
     + " WHERE 1 = 1 ";



            //var NB = Request.Form["NB"].Trim();
            //var sNB = Request.Form["sNB"].Trim();
            //var sNO = Request.Form["sNO"].Trim();
            //var sDATESTART = Request.Form["sDATESTART"].Trim();
            //var sDATEEND = Request.Form["sDATEEND"].Trim();
            //var STATUS = Request.Form["sSTATUS"].Trim();

            //var MemberSH = Request.Form["MemberSH"].Trim();
            //var COMCITYNB = Request.Form["COMCITYNB"].Trim();


            //if (NB != "")
            //{
            //    sql += " and TSE.COMMITTEENB = " + NB;
            //}
            //if (sNB != "")
            //{
            //    sql += " and TSE.NB =" + sNB;
            //}

            //if (sNO != "")
            //{
            //    sql += " and  TSE.SESNO =" + sNO;
            //}

            //if (STATUS != "")
            //{
            //    sql += " and  TSE.STATUS =" + STATUS;
            //}

            //if (sDATESTART != "")
            //{
            //    sql += " and TSE.SESDATE >= TO_DATE('" + sDATESTART + "','DD/MM/YYYY') ";
            //}

            //if (sDATEEND != "")
            //{
            //    sql += " and TSE.SESDATE <= TO_DATE('" + sDATEEND + "','DD/MM/YYYY') ";
            //}
            //if (MemberSH != "")
            //{
            //    sql += " and TSP.MEMBERNB =" + MemberSH;
            //}


            //CodesController bb = new CodesController();

            //var ci = bb.GetCityForRead();

            //if (ci == "0")
            //{
            //    if (COMCITYNB != "")
            //    {
            //        sql += " and TSE.SESCITYNB =" + COMCITYNB;
            //    }

            //}
            //else
            //{
            //    sql += " and TSE.SESCITYNB =" + ci;
            //}

            //sql += " ORDER BY TSE.NB, TSP.MEMBERNB ";

            var data = db.Database.SqlQuery<ViewModel.SessionsAndProcedsVM>(sql);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult SessionsAndProceds_PDF()
        {
            return View();
        }


    }
}