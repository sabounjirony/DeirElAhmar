using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web;

namespace Tools
{
    public static class Constants
    {
        public const long SystemUser = 1;
        public const long FullAdminRole = 1;

        #region Connection
        
        public static string GetConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["Default"].ToString();
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.Password = Utilities.Cryptography.Decrypt(builder.Password, true);
            return builder.ConnectionString;
        }

        public static string GetStateConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["Default"].ToString();
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.Password = Utilities.Cryptography.Decrypt(builder.Password, true);
            builder.InitialCatalog = ConfigurationManager.AppSettings["StateMgtDbName"];
            return builder.ConnectionString;
        }

        #endregion
        
        #region Path

        public static string GetWebAppRootUrl()
        {
            return "";
            //var url = HttpContext.Current.Request.Url;
            //var strUrl = url.AbsoluteUri.Replace(url.AbsolutePath, "") + "//";
            //if (strUrl.Contains("?")) strUrl = Utilities.StringUtilities.Left(strUrl, strUrl.IndexOf("?", StringComparison.CurrentCulture));
            //return strUrl + GetWebAppUrlPath();
        }

        public static string GetWebAppUrlPath()
        {
            return HttpContext.Current.Request.ApplicationPath;
        }

        public static string GetWebAppPhysicalPath()
        {
            var retval = string.Empty;

            var appMode = ConfigurationManager.AppSettings["APP_MODE"];
            var pathInstall = ConfigurationManager.AppSettings["PathInstall"];

            if (appMode == null)
            {
                //retval = HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath);
                retval = HttpContext.Current.Server.MapPath("~");
                retval += (!retval.EndsWith("\\")) ? "\\" : "";
                return retval;
            }

            switch (appMode.ToUpper())
            {
                case "WIN":
                    if (pathInstall != null) retval = pathInstall;
                    break;
                case "WEB":
                    retval = HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath);
                    break;
                case "DEBUG":
                    retval = ConfigurationManager.AppSettings["PathInstall"];
                    break;
            }

            retval += (!retval.EndsWith("\\")) ? "\\" : "";
            return retval;

        }

        public static string GetAppRootDirectory(string fullRoot)
        {
            fullRoot = fullRoot.ToUpper();
            var rootDir = fullRoot;
            if (rootDir.Contains("\\BIN\\"))
            {
                var unusedPart = fullRoot.Substring(fullRoot.IndexOf("\\BIN\\", StringComparison.CurrentCulture) + 1, fullRoot.Length - fullRoot.IndexOf("\\BIN\\", StringComparison.CurrentCulture) - 1);
                rootDir = fullRoot.Replace(unusedPart, "\\");
            }
            return rootDir.Replace("\\\\", "\\");
        }

        public static string GetDocumentsPath()
        {
            return GetWebAppPhysicalPath() + "Documents";
        }

        public static void VerifyDirectoryExist(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static string GetCallingAssemblyLocalPath()
        {
            return (new Uri(Assembly.GetCallingAssembly().CodeBase)).LocalPath;
        }

        #endregion Path

        public static IEnumerable<T> SingleItemAsEnumerable<T>(this T item)
        {
            yield return item;
        }
        public static List<T> SingleItemAsList<T>(this T item)
        {
            return new List<T> {item};
        }
    }
}