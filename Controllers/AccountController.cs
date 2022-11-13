using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using Proced.DataAccess.Models.CF;
using AppsManagerDataAccess.DAL;
using Passengers.Models.AccountModels;

namespace Passengers.Controllers

{
    //[Authorize]
    public class AccountController : Controller
    {
        public ProcedContext db = new ProcedContext();

        private const string Default_Print_Option_Name = "Default_Print_Option_Name";
        private const string Default_Records_Per_Page_Name = "Default_Records_Per_Page_Name";

        // GET: Helper
        #region User Settings
        #region User Default Print Option
        public int ChangeDefaultPrintOption(int optionId)
        {
            int result = 1;
            if (User.Identity.IsAuthenticated)
            {
                Repository<USER> usersRepo = new Repository<USER>(db);
                try
                {
                    var user = usersRepo.GetList(u => u.USERNAME == User.Identity.Name).First();
                    if (user.SETTINGS.Any(setting => setting.NAME == Default_Print_Option_Name))
                    {
                        user.SETTINGS.SingleOrDefault(setting => setting.NAME == Default_Print_Option_Name).VAL = optionId;
                        user.SETTINGS.SingleOrDefault(setting => setting.NAME == Default_Print_Option_Name).DAT = DateTime.Now;
                    }
                    else
                    {
                        db.SETTINGS.Add(new SETTINGS
                        {
                            USERNB = user.NB,
                            NAME = Default_Print_Option_Name,
                            DAT = DateTime.Now,
                            VAL = optionId,
                            DATATYP = "int"
                        });
                    }
                    db.SaveChanges();
                    result = optionId;
                }
                catch (Exception) { }
            }
            return result;
        }

        public int GetDefaultPrintOption()
        {
            int result = 1;
            if (User.Identity.IsAuthenticated)
            {
                Repository<USER> usersRepo = new Repository<USER>(db);
                try
                {
                    var user = usersRepo.GetList(u => u.USERNAME == User.Identity.Name).First();
                    if (user.SETTINGS.Any(setting => setting.NAME == Default_Print_Option_Name))
                    {
                        result = (int)user.SETTINGS.SingleOrDefault(setting => setting.NAME == Default_Print_Option_Name).VAL;
                    }
                }
                catch (Exception) { }
            }
            return result;
        }
        #endregion
        #region User Default Records Per Page Options
        public int ChangeDefaultRecordsPerPageOption(int optionId)
        {
            int result = 1;
            if (User.Identity.IsAuthenticated)
            {
                Repository<USER> usersRepo = new Repository<USER>(db);
                try
                {
                    var user = usersRepo.GetList(u => u.USERNAME == User.Identity.Name).First();
                    if (user.SETTINGS.Any(setting => setting.NAME == Default_Records_Per_Page_Name))
                    {
                        user.SETTINGS.SingleOrDefault(setting => setting.NAME == Default_Records_Per_Page_Name).VAL = optionId;
                        user.SETTINGS.SingleOrDefault(setting => setting.NAME == Default_Records_Per_Page_Name).DAT = DateTime.Now;
                    }
                    else
                    {
                        user.SETTINGS.Add(new SETTINGS
                        {
                            USERNB = user.NB,
                            NAME = Default_Records_Per_Page_Name,
                            DAT = DateTime.Now,
                            VAL = optionId,
                            DATATYP = "int"
                        });
                    }
                    db.SaveChanges();
                    result = optionId;
                }
                catch (Exception e)
                {

                }
            }
            return result;
        }

        public int GetDefaultRecordsPerPageOption()
        {
            int result = 1;
            if (User.Identity.IsAuthenticated)
            {
                Repository<USER> usersRepo = new Repository<USER>(db);
                try
                {
                    var user = usersRepo.GetList(u => u.USERNAME == User.Identity.Name).First();
                    if (user.SETTINGS.Any(setting => setting.NAME == Default_Records_Per_Page_Name))
                    {
                        result = (int)user.SETTINGS.SingleOrDefault(setting => setting.NAME == Default_Records_Per_Page_Name).VAL;
                    }
                }
                catch (Exception) { }
            }
            return result;
        }
        #endregion
        #endregion

        #region Account Actions
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult SignIn(LoginViewModel model, string ReturnUrl)
        {
            USER loggedUser2;

            try
            {
                loggedUser2 = db.USERS.Where(u => u.USERNAME == model.UserName).FirstOrDefault();
                if (loggedUser2.LOCKED == true)
                {
                    ModelState.AddModelError("", " الحساب معطل  !!      ");

                    ViewData["ERR"] = " الحساب معطل !!  ";
                    return View();
                }
            }
            catch
            {
                ViewData["ERR"] = "يوجد خطأ في كلمة السر أو اسم المستخدم";
            }
             Security.UserAccountManager uam = new Security.UserAccountManager(db);
            if (uam.Login(model))
            {

                Session["UserName"] = model.UserName;
                var loggedUser = db.USERS.Where(u => u.USERNAME == model.UserName).FirstOrDefault();

                Session["UserNB"] = loggedUser.NB;




                Session["UserCity"] = loggedUser.CITYNB;
                Session["UserCityNb"] = loggedUser.CITYNB;
                Session["CityNb"] = loggedUser.CITYNB;
                Session["CityName"] = loggedUser.CITY_NAME;


                if (!loggedUser.LASTLOGINDATE.HasValue)
                {


                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ee)
                    {


                    }
                    return RedirectToAction("ChangePassword", "Account");
                }
                loggedUser.LASTLOGINDATE = System.DateTime.Now;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ee)
                {


                }
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "VehiclesSSO");
                }
            }
            else
            {
                ViewData["ERR"] = "يوجد خطأ في كلمة السر أو اسم المستخدم";
                return View();
            }




        }
        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = DateTime.Now.AddDays(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult ChangePassword()
        {
            int t = GetDefaultRecordsPerPageOption();
            List<SelectListItem> list1 = new List<SelectListItem>();
            SelectListItem sle1 = new SelectListItem
            {
                Text = "5",
                Value = "5",
                Selected = (t == 5)
            };
            list1.Add(sle1);

            SelectListItem sle2 = new SelectListItem
            {
                Text = "10",
                Value = "10",
                Selected = (t == 10)
            };
            list1.Add(sle1);

            SelectListItem sle3 = new SelectListItem
            {
                Text = "20",
                Value = "20",
                Selected = (t == 20)
            };
            list1.Add(sle3);
            ViewBag.DefaultRecordsPerPage = list1;

            List<SelectListItem> list2 = new List<SelectListItem>();
            int tt = GetDefaultPrintOption();
            SelectListItem s1 = new SelectListItem
            {
                Text = "Excel",
                Value = "1",
                Selected = (tt == 1)
            };
            list2.Add(s1);
            SelectListItem s2 = new SelectListItem
            {
                Text = "Word",
                Value = "2",
                Selected = (tt == 2)
            };
            list2.Add(s2);
            SelectListItem s3 = new SelectListItem
            {
                Text = "PDF",
                Value = "3",
                Selected = (tt == 3)
            };
            list2.Add(s3);

            SelectListItem s4 = new SelectListItem
            {
                Text = "HTML",
                Value = "4",
                Selected = (tt == 4)
            };
            list2.Add(s4);
            ViewBag.DefaultPrintOption = list2;
            ChangeDefaults model = new ChangeDefaults
            {
                DefaultPrintOption = tt,
                DefaultRecordsPerPage = t,
            };
            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public ActionResult ChangePWD(ChangeDefaults model)
        {
            var ph = new PasswordHasher();
            //string pwd = ph.HashPassword(loginModel.Password);
            string uName = Session["UserName"].ToString();
            var user = db.USERS.SingleOrDefault(u => u.USERNAME == uName);
            var res = ph.VerifyHashedPassword(user.PASSWORD, model.OldPassword);
            if (user == null || res != PasswordVerificationResult.Success)
            {
                return Content("تأكد من كلمة المرور القديمة");

            }
            if (model.NewPassword != model.NewPasswordConfirmed)
            {

                return Content("تأكد من تطابق كلمة المرور الجديدة مع التأكيد");

            }
            else
            {
                var query = (from q in db.USERS
                             where q.NB == user.NB
                             select q).First();
                query.PASSWORD = ph.HashPassword(model.NewPassword);
                query.LASTLOGINDATE = DateTime.Now;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {


                }
                // return result;
                return Logout();
            }
            //  return View();

        }
        #endregion

        protected override void Dispose(bool disposing)
        {


            db.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}