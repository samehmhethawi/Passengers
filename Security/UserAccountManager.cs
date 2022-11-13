//using AppsManager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Proced.DataAccess.Models.CF;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Passengers.Models.AccountModels;

namespace Passengers.Security
{
    public class UserAccountManager
    {
        Claim[] rolesClaims = null;

        public ProcedContext db = new ProcedContext();

        public UserAccountManager(ProcedContext dbContext)
        {
            this.db = dbContext;
        }

        private bool IsValidCredintials(LoginViewModel loginModel)
        {
            if (loginModel.Password == null)
                loginModel.Password = "";
            if (loginModel.UserName == null)
                loginModel.UserName = "";
            var ph = new PasswordHasher();
            string pwd = ph.HashPassword(loginModel.Password);
            var user = db.USERS.SingleOrDefault(u => u.USERNAME == loginModel.UserName);

            if (user != null)

            {
                var res = ph.VerifyHashedPassword(user.PASSWORD, loginModel.Password);
                if (res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded)
                {

                    var Roles = user.USER_ROLES.ToList();
                    int length = Roles.Count;
                    if (rolesClaims == null)
                    {
                        rolesClaims = new Claim[length];
                    }
                    for (int i = 0; i < length; i++)
                    {
                        rolesClaims[i] = new Claim(ClaimTypes.Role, Roles[i].ROLES.NAME);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool Login(LoginViewModel loginModel)
        {
            if (this.IsValidCredintials(loginModel))
            {
                var ident = new ClaimsIdentity(
                  new[]
                  { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, loginModel.UserName),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

              new Claim(ClaimTypes.Name, loginModel.UserName),


          },
                  DefaultAuthenticationTypes.ApplicationCookie);
                ident.AddClaims(rolesClaims);
                HttpContext.Current.GetOwinContext().Authentication.SignIn(
                   new AuthenticationProperties { IsPersistent = false }, ident);
                return true; // auth succeed 
            }

            return false;
        }
    }
}