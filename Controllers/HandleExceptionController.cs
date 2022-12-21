using Oracle.ManagedDataAccess.Client;
using System;
using System.Web.Mvc;


namespace Passengers.Controllers
{
    [NoCache, RedirectOnErrorAttribute]
    public class HandleExceptionController : Controller
    {
        public void AddLog(string usernb, string username, string controller, string action, string message, string trace)
        {
           
        }
        public ActionResult Index(string returnUrl)
        {
            try
            {
                string controller = TempData["Controller"].ToString();
                string action = TempData["Action"].ToString();
                //string exception_type= TempData["exception_type"];
                string usernb = MyOwnData.MyNB().ToString();
                string username = MyOwnData.MyFullName();
                // ToDo
                AddLog(MyOwnData.MyNB().ToString(), MyOwnData.MyFullName(), controller, action, TempData["Error"].ToString(), TempData["StackTrace"].ToString());
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    ViewBag.ReturnUrl = returnUrl;
                }
            }
            catch
            {
                return View();
            }
            return View();
        }

        public ActionResult Access_Denied(string returnUrl)
        {
            try
            {
                string controller = TempData["Controller"].ToString();
                string action = TempData["Action"].ToString();
                //string exception_type= TempData["exception_type"];
                string usernb = MyOwnData.MyNB().ToString();
                string username = MyOwnData.MyFullName();
                // ToDo
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    ViewBag.ReturnUrl = returnUrl;
                }
            }
            catch
            {
                return View();
            }
            return View();
        }
        public ActionResult MailTo()
        {
            return RedirectToAction("Index");
        }
    }
}
