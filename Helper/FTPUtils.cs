using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace ProcedBase.Helper
{
    public class FTPUtils
    {
        public const int RequestTimeOut = 5000;
        public const char PathSeparator = '/';
        public static string DateFormat = "yyyy" + FTPUtils.PathSeparator + "MM" + FTPUtils.PathSeparator + "dd";
        #region Public Attributes
        public static string FtpURL
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpURL"];
            }
        }

        public static string FtpHomeDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpHomeDirectory"];
            }
        }
        public static string FtpHomeDirectoryNativeDocs
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpHomeDirectoryNativeDocs"];
            }
        }
        public static string FtpHomeDirectoryArchive
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpHomeDirectoryArchive"];
            }
        }

        public static string FTPFullPath
        {
            get
            {
                return string.Format("{0}{1}{2}", FtpURL, PathSeparator, FtpHomeDirectory);
            }
        }

        public static string FTPFullPath_NativeDocs
        {
            get
            {
                return string.Format("{0}{1}{2}", FtpURL, PathSeparator, FtpHomeDirectoryNativeDocs);
            }
        }

        public static string FTPFullPath_Archive
        {
            get
            {
                return string.Format("{0}{1}{2}", FtpURL, PathSeparator, FtpHomeDirectoryArchive);
            }
        }


        public static string FtpUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpUsername"];
            }
        }

        public static string FtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpPassword"];
            }
        }
        #endregion

        #region Public Methods

        private static string MakeArchiveFTPDir(string pathToCreate)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;

            string currentDir = pathToCreate.Contains(FtpHomeDirectoryArchive) ? FtpURL : FTPFullPath_Archive;

            string[] subDirs = pathToCreate.Split(PathSeparator);

            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + PathSeparator + subDir;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.Timeout = RequestTimeOut;
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
        private static string MakeFTPDir(string pathToCreate)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;

            string currentDir = pathToCreate.Contains(FtpHomeDirectory) ? FtpURL : FTPFullPath;

            string[] subDirs = pathToCreate.Split(PathSeparator);

            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + PathSeparator + subDir;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.Timeout = RequestTimeOut;
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

        public static string UploadArchiveFile(byte[] file, string pathToUploadTo=null, string fileName=null)
        {


            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".pdf";
            }
            //FtpURL + PathSeparator + FtpHomeDirectoryArchive + PathSeparator +
            pathToUploadTo = DateTime.Now.ToString(DateFormat);
            string uploadedPath = PathSeparator + pathToUploadTo + PathSeparator + fileName;
            string uploadedFullPath = FtpURL + PathSeparator + FtpHomeDirectoryArchive + PathSeparator + pathToUploadTo + PathSeparator + fileName;
             try
            {
                MakeFTPDir(FtpHomeDirectoryArchive);
            }
            catch (Exception)
            {
            }
            try
            {
                MakeArchiveFTPDir(pathToUploadTo);
            }
            catch (Exception)
            {
            }
            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadedFullPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Timeout = RequestTimeOut;

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
            catch (   WebException e)
            {
                uploadedFullPath = null;
                //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
            return uploadedPath;
        }

        public static string UploadFile(byte[] file, string pathToUploadTo, string fileName)
        {
            string uploadedFullPath = FTPFullPath + PathSeparator + pathToUploadTo + PathSeparator + fileName;
            string uploadedRelativePath = ((FTPFullPath != null && FTPFullPath.EndsWith("" + PathSeparator)) ? "" : "" + PathSeparator) + pathToUploadTo + PathSeparator + fileName;
            try
            {
                MakeFTPDir(FtpHomeDirectory);
            }
            catch (Exception)
            {
            }
            try
            {
                MakeFTPDir(pathToUploadTo);
            }
            catch (Exception)
            {
            }
            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadedFullPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Timeout = RequestTimeOut;

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
                //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
            return uploadedRelativePath;
        }

        private static string MakeFTPDirNativeDocs(string pathToCreate)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;

            string currentDir = pathToCreate.Contains(FtpHomeDirectoryNativeDocs) ? FtpURL : FTPFullPath_NativeDocs;

            string[] subDirs = pathToCreate.Split(PathSeparator);

            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + PathSeparator + subDir;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.Timeout = RequestTimeOut;
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

        public static string SaveNativeDocFile(string textToSave, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".txt";
            }
            if (string.IsNullOrEmpty(textToSave))
            {
                return null;
            }
            byte[] fileContent = System.Text.Encoding.UTF8.GetBytes(textToSave);
            string pathToUploadTo = DateTime.Now.ToString(DateFormat);
            string uploadedFullPath = FTPFullPath_NativeDocs + PathSeparator + pathToUploadTo + PathSeparator + fileName;
            string uploadedRelativePath = ((FTPFullPath_NativeDocs != null && FTPFullPath_NativeDocs.EndsWith("" + PathSeparator)) ? "" : "" + PathSeparator) + pathToUploadTo + PathSeparator + fileName;
            try
            {
                MakeFTPDirNativeDocs(FtpHomeDirectoryNativeDocs);
            }
            catch (Exception)
            {
            }
            try
            {
                MakeFTPDirNativeDocs(pathToUploadTo);
            }
            catch (Exception)
            {
            }
            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadedFullPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Timeout = RequestTimeOut;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                request.ContentLength = fileContent.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = fileContent.Length;
                //request.EnableSsl = false;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContent, 0, fileContent.Length);
                    requestStream.Close();
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (WebException)
            {
                uploadedFullPath = null;
                //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
            return uploadedRelativePath;
        }
        public static bool DeleteNativeDocFile(string textTodelete)
        {
            bool result = false;
            var path = FTPFullPath_NativeDocs + textTodelete;
            try
            {
                result = DeleteFTPFile(path);
            }
            catch (Exception ex)
            { throw ex; }
            return result;
        }
        public static string GetNativeFileContent(string nativeDoce_FTP_PATH)
        {
            string result = "";
            try
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                if (!string.IsNullOrEmpty(nativeDoce_FTP_PATH))
                {
                    if (nativeDoce_FTP_PATH[0] !='/')
                    {
                        nativeDoce_FTP_PATH = nativeDoce_FTP_PATH.Insert(0, "/");
                    }

                }
                var path = FTPFullPath_NativeDocs + nativeDoce_FTP_PATH;
                byte[] fileData = request.DownloadData(path);
                result = System.Text.Encoding.UTF8.GetString(fileData);
            }
            catch (Exception EE)
            {
                return null;
            }
            return result;
        }

        public static bool DeleteFTPFile(string fullPathToFileToDelete)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;
            bool success = false;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(fullPathToFileToDelete);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Timeout = RequestTimeOut;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
                success = true;
            }
            catch
            {

            }
            return success;
        }


        public static byte[] GetFileContent(string absolutePathToFile)
        {
            try
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);

                var path = FTPFullPath + absolutePathToFile;
                byte[] fileData = request.DownloadData(path);

                return fileData;
            }
            catch (Exception EE)
            {
                return null;
            }
        }

        public static byte[] GetFileContent(string absolutePathToFile, string root)
        {
            try
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(FtpUsername, FtpPassword);

                var path = FTPFullPath_Archive + absolutePathToFile;
                byte[] fileData = request.DownloadData(path);

                return fileData;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}