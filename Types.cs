//using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Passengers
{
    public static class MyResource
    {






    }
    public static class MyOwnData
    {
        public static List<SelectListItem> GetCities()
        {
            int? currentCityNb = MyOwnData.MyCityNb();
            string username = MyOwnData.MyName();
            List<SelectListItem> cities = new List<SelectListItem>
            {
                new SelectListItem() { Text = "دمشق", Value="1" },
                new SelectListItem() { Text = "ريف دمشق", Value="2" },
                new SelectListItem() { Text ="حلب", Value="3" },
                new SelectListItem() { Text = "طرطوس", Value="4"},
                new SelectListItem() { Text ="اللاذقية", Value="5"  },
                new SelectListItem() { Text = "حمص", Value="6"},
                new SelectListItem() { Text = "حماه", Value="7"},
                new SelectListItem() { Text = "ادلب", Value="8"},
                new SelectListItem() { Text = "الرقة", Value="9"},
                new SelectListItem() { Text = "الحسكة", Value="10"},
                new SelectListItem() { Text = "دير الزور", Value="11"},
                new SelectListItem() { Text = "درعا", Value="12"},
                new SelectListItem() { Text = "السويداء", Value="13"},
                new SelectListItem() { Text = "القنيطرة", Value="14"}
            };
            if (MyOwnData.IsAdmin())
            {
                var city = cities.ToList().Select(x => new SelectListItem()
                {
                    Text = x.Value + "- " + x.Text,
                    Value = x.Value + ""

                }).ToList();
                return city;
            }
            else
            {
                var city = cities.ToList().Select(x => new SelectListItem()
                {
                    Text = x.Value + "- " + x.Text,
                    Value = x.Value + "",
                    Selected = x.Value == currentCityNb.ToString()
                }).ToList();
                return city;
            }


        }


        public static List<SelectListItem> GetCities_forcars()
        {
            int? currentCityNb = MyOwnData.MyCityNb();
            string username = MyOwnData.MyName();
            List<SelectListItem> cities = new List<SelectListItem>
            { new SelectListItem() { Text = "الكل", Value="" },
                new SelectListItem() { Text = "دمشق", Value="1" },
                new SelectListItem() { Text = "ريف دمشق", Value="2" },
                new SelectListItem() { Text ="حلب", Value="3" },
                new SelectListItem() { Text = "طرطوس", Value="4"},
                new SelectListItem() { Text ="اللاذقية", Value="5"  },
                new SelectListItem() { Text = "حمص", Value="6"},
                new SelectListItem() { Text = "حماه", Value="7"},
                new SelectListItem() { Text = "ادلب", Value="8"},
                new SelectListItem() { Text = "الرقة", Value="9"},
                new SelectListItem() { Text = "الحسكة", Value="10"},
                new SelectListItem() { Text = "دير الزور", Value="11"},
                new SelectListItem() { Text = "درعا", Value="12"},
                new SelectListItem() { Text = "السويداء", Value="13"},
                new SelectListItem() { Text = "القنيطرة", Value="14"}
            };
            if (MyOwnData.IsAdmin())
            {
                var city = cities.ToList().Select(x => new SelectListItem()
                {
                    Text = x.Value + "- " + x.Text,
                    Value = x.Value + ""

                }).ToList();
                return city;
            }
            else
            {
                var city = cities.ToList().Select(x => new SelectListItem()
                {
                    Text = x.Value + "- " + x.Text,
                    Value = x.Value + "",
                    Selected = x.Value == currentCityNb.ToString()
                }).ToList();
                return city;
            }


        }













        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = DateTime.Now.AddDays(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static class Cypher
        {
            public static byte[] Hashing(string plainText)
            {
                Encoding u8 = Encoding.UTF8;
                byte[] dataArray = u8.GetBytes(plainText);
                HashAlgorithm sha = new SHA1CryptoServiceProvider();
                byte[] result = sha.ComputeHash(dataArray);
                return result;
            }
        }
        public static double MyNB()
        {
            return Utility.MyNB();
        }
        public static int MyCityNb()
        {
            return Utility.MyCityNb();
        }
        public static string MyCityName()
        {
            return Utility.MyCityName();
        }
        public static string MyFullName()
        {
            return Utility.MyFullName();
        }
        public static bool IsAdmin()
        {
            return Utility.IsAdminUser();
        }
        public static string MyName()
        {
            return Utility.MyName();
        }
        public static string ConvertToEasternArabicNumerals(string input)
        {
            if (String.IsNullOrEmpty(input)) { return ""; }
            System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
            System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
            System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
            char[] convertedChar = new char[1];
            byte[] bytes = new byte[] { 217, 160 };
            char[] inputCharArray = input.ToCharArray();
            foreach (char c in inputCharArray)
            {
                if (char.IsDigit(c))
                {
                    bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                    utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                    convertedChars.Append(convertedChar[0]);
                }
                else
                {
                    convertedChars.Append(c);
                }
            }
            return convertedChars.ToString();
        }
    }
    public class checksession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    using (var db = new ProcedContext())
                    {

                        USER loggedUser = db.USERS.Where(u => u.USERNAME == filterContext.RequestContext.HttpContext.User.Identity.Name).FirstOrDefault();
                        if (loggedUser != null)
                        {
                            filterContext.HttpContext.Session["UserNB"] = loggedUser.NB + "";
                            filterContext.HttpContext.Session["UserName"] = loggedUser.USERNAME + "";

                            filterContext.HttpContext.Session["UserCity"] = loggedUser.CITYNB + "";
                            filterContext.HttpContext.Session["UserCityNb"] = loggedUser.CITYNB + "";
                            filterContext.HttpContext.Session["CityNb"] = loggedUser.CITYNB + "";
                            filterContext.HttpContext.Session["CityName"] = loggedUser.CITY_NAME + "";
                        }
                        else
                        {
                            filterContext.HttpContext.Session["UserNB"] = "";
                            filterContext.HttpContext.Session["UserName"] = "";

                            filterContext.HttpContext.Session["UserCity"] = "";
                            filterContext.HttpContext.Session["UserCityNb"] = "";
                            filterContext.HttpContext.Session["CityNb"] = "";
                            filterContext.HttpContext.Session["CityName"] = "";
                        }

                    }
                }
                else
                {
                    var session = filterContext.HttpContext.Session["UserNB"];

                    if (session == null)
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.HttpContext.GetOwinContext().Authentication.SignOut();


                            var x = filterContext.HttpContext.Request.Url;

                            filterContext.HttpContext.GetOwinContext().Authentication.SignOut();

                            // Indicate to the remote caller that the session has expired and where to redirect
                            filterContext.HttpContext.Response.Headers.Add("Location", new UrlHelper(filterContext.RequestContext).Action("Login", "Account", new { returnUrl = filterContext.HttpContext.Request.Url.AbsolutePath }));
                            //filterContext.Result = new HttpUnauthorizedResult("Session expired");

                            base.OnActionExecuting(filterContext);
                        }
                        else
                        {
                            filterContext.HttpContext.GetOwinContext().Authentication.SignOut();
                            //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login", area = "", returnUrl = filterContext.HttpContext.Request.Url.AbsolutePath }));

                        }
                }
            }
            catch (Exception e)
            {
                filterContext.Controller.TempData["Error"] = "حدثت مشكلة أثناء الاتصال مع قاعدة البيانات : ";
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "HandleException", action = "Index", area = "", returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery }));


                base.OnActionExecuting(filterContext);
            }
        }

    }


    [checksession, Authorize]
    public class CanDoIt : ActionFilterAttribute
    {

        //public ProcedContext db = new ProcedContext();

        public bool UserCanDoAction(string usernb, string controller, string action, string area)
        {
            using (ProcedContext db = new ProcedContext())
            {

                //return true; //to active filter remove it

                string username = HttpContext.Current.Session["UserName"].ToString();


                if (username == "Admin")
                {
                    return true;
                }
                if (username == "دعم فني")
                {
                    return true;
                }
                USER User = db.USERS.Find(int.Parse(usernb));


                if (User.LOCKED == true)
                {


                    return false;
                }
                int i = 0;
                try
                {
                    i = db.Database.SqlQuery<int>("select count(*) from appmgr.user_roles  ur,appmgr.ROLE_PROGRAMS rp , appmgr.programs pr where     pr.nb = rp.program_nb and  pr.name='"/* + Resources.Prog_id +*/+ "Passengers"+ "' and UR.ROLENB = RP.ROLE_NB and ur.usernb = " + usernb).FirstOrDefault();

                }
                catch
                {
                    return false;
                }
                if (i == 0)
                {

                    return false;

                }
                string ActionUrl = "";
                if (area == "")
                {
                    ActionUrl = controller + "/" + action;
                }
                else
                {
                    ActionUrl = area + "/" + controller + "/" + action;
                }
                ACTIONS aCTION = db.ACTIONS.Where(a => a.FULLNAME == ActionUrl).FirstOrDefault();
                if (aCTION == null)
                {
                    return true;
                }
                if (aCTION.IS_GRANT == false)
                {

                    return true;
                }
                IList<ROLE_ACTIONS> rOLE_ACTIONS = db.ROLE_ACTIONS.Where(d => d.ACTIONNB == aCTION.NB && d.ROLES.USER_ROLES.Any(x => x.USERNB == User.NB)).ToList();
                if (rOLE_ACTIONS.Count == 0)
                {

                    return false;
                }
                else
                { return true; }


            }

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ph = new PasswordHasher();
            var routingValues = filterContext.RouteData.Values;
            var area = (string)filterContext.RouteData.DataTokens["area"] ?? string.Empty;
            var controller = (string)routingValues["controller"] ?? string.Empty;
            var action = (string)routingValues["action"] ?? string.Empty;
            //string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString();
            // string action = filterContext.RouteData.Values["action"].ToString();
            filterContext.Controller.TempData["Controller"] = controller;
            filterContext.Controller.TempData["Action"] = action;
            string usernb = MyOwnData.MyNB().ToString();
            using (ProcedContext db = new ProcedContext())
            {
                USER User = db.USERS.Find(int.Parse(usernb));
                //string hashPassword = ph.HashPassword("123456");
                var res = ph.VerifyHashedPassword(User.PASSWORD, "123456");
                if (res == PasswordVerificationResult.Success)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "ChangePassword", area = "", returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery }));
                    base.OnActionExecuting(filterContext);
                }
            }
            if (!UserCanDoAction(usernb, controller, action, area))
            {
                filterContext.Controller.TempData["exception_type"] = "خطأ صلاحيات";
                filterContext.Controller.TempData["Action"] = action;
                filterContext.Controller.TempData["Error"] = "لا تملك الصلاحية اللازمة للعملية التي اخترتها";
                filterContext.Controller.TempData["StackTrace"] = "";


                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "HandleException", action = "Access_Denied", area = "", returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery }));

               
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class NoCache : ActionFilterAttribute
    {

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }
    public class RedirectOnErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            string controller = filterContext.Controller.GetType().Name;
            string action = filterContext.RouteData.Values["action"].ToString();
            string usernb = MyOwnData.MyNB().ToString();
            string username = MyOwnData.MyName();
            // Don't interfere if the exception is already handled
            if (filterContext.ExceptionHandled)
                return;
            //throw filterContext.Exception;
            // Let the next request know what went wrong
            filterContext.Controller.TempData["exception_type"] = filterContext.Exception.GetType().Name;
            filterContext.Controller.TempData["Controller"] = controller;
            filterContext.Controller.TempData["Action"] = action;
            filterContext.Controller.TempData["Error"] = filterContext.Exception.Message + " 111-INNER EXCEPTION ";
            if (filterContext.Exception.InnerException != null)
                filterContext.Controller.TempData["Error"] += "" + filterContext.Exception.InnerException.InnerException;
            filterContext.Controller.TempData["StackTrace"] = filterContext.Exception.StackTrace;

            //Set up a redirection to my global error handler
            //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
            //new { controller = "Exception", action = "HandleError" }
            //));
            //Set up a redirection to my global error handler

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "HandleException", action = "Index", area = "", returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery }));

            //Advise subsequent exception filters not to interfere
            //and stop ASP.NET from producing a "yellow screen of death"
            filterContext.ExceptionHandled = true;
            //Erase any output already generated
            filterContext.HttpContext.Response.Clear();
        }
    }
    public class RequireSSL : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase req = filterContext.HttpContext.Request;
            HttpResponseBase res = filterContext.HttpContext.Response;
            if (!req.IsSecureConnection && !req.IsLocal)
            {
                var builder = new UriBuilder(req.Url)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = 443
                };
                res.Redirect(builder.Uri.ToString());
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public static class Cypher
    {
        public static byte[] Hashing(string plainText)
        {
            Encoding u8 = Encoding.UTF8;
            byte[] dataArray = u8.GetBytes(plainText);
            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(dataArray);
            return result;
        }
    }
    public static class ErrorString
    {
        public static string GetErrorString(int errCode)
        {
            switch (errCode)
            {
                case 00001: return "هذا السجل   موجود يرجى التدقيق وإعادة المحاولة";
                case 01400: return "هناك حقول واجبة الإدخال";
                case 01407: return "هناك حقول واجبة الإدخال";
                case 01722: return "يوجد خطأ بإدخال رقمي";
                case 01830: return "النص المدخل للتاريخ غير سليمة";
                case 02292: return "عملية الحذف غير ممكنة لوجود سجلات مرتبطة";
                case 01033: return "قاعدة المعطيات )أوراكل( قيد التشغيل. الرجاء الانتظار ثم إعادة المحاولة.";
                case 20001: return "لا يمكن إجراء العملية المطلوبة! تم قفل العام الحالي.";
                default: return "خطأ غير محدد يحمل الرقم /" + errCode.ToString() + "/";
            }
        }

        public static string GetErrorString(string errCode)
        {
            return errCode;
        }
    }
    public static class MvcHelper
    {
        public static List<string> GetActions(this Type controller)
        {
            return controller.GetMethods()
                .Where(x => (!x.Name.Equals("ToString") && (x.ReturnType == typeof(ActionResult) || x.ReturnType == typeof(string))))
                .OrderBy(x => x.Name)
                .Select(x => x.Name)
                .Distinct()
                .ToList();
        }

        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(T)))
                .ToList();
        }

        public static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>()
                .ForEach(type => controllerNames.Add(type.Name));
            return controllerNames;
        }








    }
}

