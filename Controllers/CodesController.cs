using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Passengers.Controllers
{
    public  class CodesController : Controller
    {
        public ProcedContext db = new ProcedContext();
        // GET: Codes

        public  bool IsAdmin()
        {

            var usernb = Utility.MyNB();
            var admin = db.Database.SqlQuery<int>("select 1 from APPMGR.USER_ROLES where ROLENB = 0 and USERNB = " + usernb).FirstOrDefault();
            if (admin == 0)
            { return false; }
            else
            { return true; }

        }
        public  bool IsAdminCity()
        {
            var usernb = Utility.MyNB();
            var admin = db.Database.SqlQuery<int>("select 1 from APPMGR.USER_ROLES where ROLENB = 1000 and USERNB = " + usernb).FirstOrDefault();
            if (admin == 0)
            { return false; }
            else
            { return true; }

        }

        public  string GetCityForRead()
        {

            if (IsAdmin() || IsAdminCity())
            { return "0"; }
            else
            {
                return Utility.MyCityNb().ToString();
            }
        }
        public ActionResult GetStatus()
        {

            var status = db.TRSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStatusOfComm()
        {

            var status = db.TRSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            }).Where(x=>x.ID == 0 || x.ID == 1);
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCity()
        {
            var _isadmin = IsAdmin();
            var _isadmincity = IsAdminCity();
            if (_isadmin || _isadmincity)
            {
                var city = db.ZCITYS.Select(x => new
                {
                    ID = x.NB,
                    NAME = x.NAME,
                });
                return Json(city, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var mycitynb = Utility.MyCityNb();
                var city = db.ZCITYS.Where(y => y.NB == mycitynb).Select(x => new
                {
                    ID = x.NB,
                    NAME = x.NAME,
                });
                return Json(city, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetMemberShip()
        {

            var status = db.TRZMEMBERSHIP.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMemberPostion()
        {

            var status = db.TRZMEMBERPOSITION.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }


        public int GetMaxOrdr(string TableName)
        {
           var sql = "SELECT NVL(MAX(ORDR),0) FROM " + TableName;
            var data = db.Database.SqlQuery<int>(sql).FirstOrDefault();  
            return data +1;
        }

        public ActionResult GetProced()
        {

            var status = db.ZPROCEDTYPS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCarCat()
        {

            var status = db.ZCARKINDS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Gettrlinetyps()
        {

            var status = db.ZTRLINETYPES.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetallCity()
        {            
                var city = db.ZCITYS.Select(x => new
                {
                    ID = x.NB,
                    NAME = x.NAME,
                });
                return Json(city, JsonRequestBehavior.AllowGet);  
            

        }

        public ActionResult GETTRSESSIONSPROCEDSTATUS()
        {
            var city = db.TRSESSIONSPROCEDSTATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(city, JsonRequestBehavior.AllowGet);
       
        }
        public ActionResult GETZPROCEDTYPS()
        {
            var city = db.ZPROCEDTYPS.Where(s =>s.TYP == "TRANSSESSION").Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(city, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GETZTRLINETYPES()
        {
            var city = db.ZLINETYPES.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(city, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GETCARKIND()
        {
            var kind = db.ZCARKINDS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(kind, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GETCARREG()
        {
            var reg = db.ZCARREGS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(reg, JsonRequestBehavior.AllowGet);

        }
        //public ActionResult GETTRLINES_TYPES_DURATIONS()
        //{
        //    var city = db.TRLINES_TYPES_DURATIONS.Select(x => new
        //    {
        //        ID = x.NB,
        //        NAME = x.NAME,
        //    });
        //    return Json(city, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult GetZOUTACTS()
        {
            var reg = db.ZOUTACTS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(reg, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAgrStatus()
        {
            var STATUS = db.TRAGREEMENTS_STATUS.Select(x => new
            {
                ID = x.NB,
                NAME = x.NAME,
            });
            return Json(STATUS, JsonRequestBehavior.AllowGet);

        }


        public static string UploadFile(byte[] file, string FTPFullPath, string fileName ,string FtpHomeDirectory,long nb)
        {
            var FtpUsername = ConfigurationManager.AppSettings["FtpUsername"];
            var FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
            var FTPURL = ConfigurationManager.AppSettings["FtpURL"];
            string uploadedFullPath = FTPURL + FTPFullPath + "(" + nb + ")"+fileName;
            string uploadedRelativePath = FTPFullPath + "(" + nb + ")" + fileName;// ((FTPFullPath != null && FTPFullPath.EndsWith("" + '/')) ? "" : "" + '/') + pathToUploadTo + PathSeparator + fileName;
            try
            {
                MakeFTPDir(FTPFullPath, FtpHomeDirectory);
            }
            catch (Exception)
            {
            }
            try
            {
              //  string uploadfilename = Guid.NewGuid().ToString() + "-" + Path.GetFileName(txtDocFile.FileName);
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadedFullPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Timeout = 5000;
                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                request.ContentLength = file.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = file.Length;
                //request.EnableSsl = false;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(file, 0, file.Length);
                    requestStream.Close();
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (WebException)
            {
                uploadedFullPath = null;
                // return Json(new { success = false, responseText = "حدث خطأ اثناء الارشفة" }, JsonRequestBehavior.AllowGet);

            }
            return uploadedRelativePath;
        }

        private static string MakeFTPDir(string pathToCreate,string FtpHomeDirectory)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;
            
            var FtpURL = ConfigurationManager.AppSettings["FtpURL"];
            var FTPFullPath = string.Format("{0}{1}{2}", FtpURL, "/", FtpHomeDirectory);
            var FtpUsername = ConfigurationManager.AppSettings["FtpUsername"];
            var FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
            string currentDir = pathToCreate.Contains(FtpHomeDirectory) ? FtpURL : FTPFullPath;

            string[] subDirs = pathToCreate.Split('/');

            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + '/' + subDir;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.Timeout = 5000;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                    ftpStream.Close();
                    response.Close();
                }
                catch
                {
                    //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
                }
            }
            return currentDir;
        }

        public static byte[] GetFileContent(string absolutePathToFile)
        {
            try
            {
                var FtpUsername = ConfigurationManager.AppSettings["FtpUsername"];
                var FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
                var FTPURL = ConfigurationManager.AppSettings["FtpURL"];
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                var path = FTPURL + absolutePathToFile;
                byte[] fileData = request.DownloadData(path);

                return fileData;
            }
            catch (Exception EE)
            {
                return null;
            }
        }

        public static bool DeleteFTPFile(string fullPathToFileToDelete)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;
            bool success = false;
            try
            {
                var FtpUsername = ConfigurationManager.AppSettings["FtpUsername"];
                var FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
                var FTPURL = ConfigurationManager.AppSettings["FtpURL"] + fullPathToFileToDelete;

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(FTPURL);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Timeout = 50000;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;

            }
            return success;
        }
    }
    
}

