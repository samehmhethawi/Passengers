using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;
using static Passengers.Helper.ZPRICES;

namespace Passengers.Controllers
{
    [checksession, Authorize, CanDoIt, Audit, RedirectOnError]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintCarQrCode(string carNb, string carProcedNb)
        {
            ViewBag.CarNb = carNb;
            ViewBag.CarProcedNb = carProcedNb;
            if (!string.IsNullOrEmpty(carNb))
            {

                ViewBag.CarQRCode = BarCodeHelper.GenerateBarcode("" + carNb, 120, 120, BarcodeFormat.QR_CODE);
            }
            if (!string.IsNullOrEmpty(carProcedNb))
            {
                ViewBag.CarProcedQRCode = BarCodeHelper.GenerateBarcode("" + carProcedNb, 120, 120, BarcodeFormat.QR_CODE);
            }
            return PartialView("PrintQR");
        }
    }
}