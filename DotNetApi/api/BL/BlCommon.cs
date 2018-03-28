using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using BL.Setup;
using BL.System;
using BL.System.MultiLanguage;
using BL.System.Security;
using Tools;
using Tools.Utilities;
using VM;
using VM.Ddl;
using VM.QuickSearch;

namespace BL
{
    public static class BlCommon
    {

        public static int DefaultTimeOut()
        {
            return Convert.ToInt32(BlCode.LoadSingle(Constants.SystemUser, "_System", "DefaultTimeOut").Value1);
        }

        public static bool IsDbConnected()
        {
            using (var connection = new SqlConnection(Constants.GetConnection()))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (Exception)
                { return false; }
            }
        }

        public static DateTime GetServerDateTime(bool withTime = true)
        { return DL.DbManager.GetServerDateTime(withTime); }

        public static void InitiateService()
        {
            //Initiate and create all multilanguage files/directories for all languages
            var blDescription = new BlDescription();
            blDescription.Initiate(Constants.SystemUser);

            //Make sure temp directory exists
            var tempDirectory = Constants.GetAppRootDirectory(Constants.GetCallingAssemblyLocalPath()) + "io\\temp\\";
            if (!Directory.Exists(tempDirectory)) Directory.CreateDirectory(tempDirectory);

            //Clean temporary directory
            var di = new DirectoryInfo(tempDirectory);
            foreach (var file in di.GetFiles())
            { file.Delete(); }

            foreach (var dir in di.GetDirectories())
            { dir.Delete(true); }
        }

        public static string GetSignature(long userId, DateTime entryDate)
        {
            var user = BlUser.LoadSingle(userId);
            var label = BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblSignature", user.LanguageId);
            return string.Format(label, user.UserName, DateUtilities.FormatDateForDisplay(entryDate, true));
        }

        public static long GetNextId(long userId, string Id, long branchId)
        {
            //Construct the id
            string tmpId;
            if (branchId != 0)
            { tmpId = Id + "_" + branchId; }
            else
            { tmpId = Id; }

            return DL.DbManager.GetNextId(tmpId);
        }

        #region QuickSearch

        public static QuickSearchResult LoadSingle(long userId, string entity, string id, string parameters = "")
        {
            var item = new DdlVm.DdlOption();

            switch (entity)
            {
                //case "LOCATION": item = BlLocation.LoadQs(userId, Convert.ToInt64(id)); break;
                //case "LOCATIONEMPLOYEE": item = BlLocation.LoadLocationEmployeeQs(userId, id); break;
                case "EMPLOYEE": item = BlEmployee.LoadQs(userId, Convert.ToInt64(id), parameters); break;
                //case "PRODUCT": item = BlProduct.LoadQs(userId, Convert.ToInt64(id)); break;
                //case "SUPPLIER": item = BlSupplier.LoadQs(userId, Convert.ToInt64(id)); break;
                case "USER": item = BlUser.LoadQs(userId, Convert.ToInt64(id)); break;
                case "MODULE": item = BlModule.LoadQs(userId, id); break;
                case "CODE": item = BlCode.LoadQs(userId, id, parameters); break;
                //case "WAREHOUSE": item = BlWarehouse.LoadQs(userId, Convert.ToInt64(id), parameters); break;
                //case "LOCATIONMORAL": item = BlLocationMoral.LoadQs(userId, Convert.ToInt64(id), parameters); break;
                //case "STOCK": item = BlStock.LoadQs(userId, Convert.ToInt64(id)); break;
            }

            var toRet = new QuickSearchResult
            {
                id = item.value,
                text = item.label
            };

            return toRet;
        }

        public static List<QuickSearchResult> LoadMultiple(long userId, string entity, List<string> ids, string parameters = "")
        {
            List<DdlVm.DdlOption> items = null;

            switch (entity)
            {
                //case "LOCATION": items = BlLocation.LoadMultipleQs(userId, ids.ConvertAll(Convert.ToInt64)); break;
                //case "TEAM": items = Structure.BlTeam.LoadMultiple(userId, ids.ConvertAll(Convert.ToInt64)); break;
                //case "EMPLOYEE": items = BlEmployee.LoadMultiple(userId, ids.ConvertAll(Convert.ToInt64), parameters); break;
                //case "EMAIL": items = Entities.BlEntity.LoadMultiple2(userId, ids.ConvertAll(Convert.ToInt64)); break;
            }

            var toRet = (from item in items
                         select new QuickSearchResult
                         {
                             id = item.value,
                             text = item.label
                         }).ToList();
            return toRet;
        }

        public static QuickSearchPagedResult LoadSearch(long userId, string entity, string parameters, string searchTerm, int pageSize, int pageNum)
        {
            List<DdlVm.DdlOption> items = null;
            long count = 0;
            switch (entity)
            {
                //case "LOCATION": items = BlLocation.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                //case "LOCATIONEMPLOYEE": items = BlLocation.LoadLocationEmployeeQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                case "EMPLOYEE": items = BlEmployee.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                //case "PRODUCT": items = BlProduct.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                //case "SUPPLIER": items = BlSupplier.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                case "USER": items = BlUser.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                case "MODULE": items = BlModule.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                case "CODE": items = BlCode.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                //case "WAREHOUSE": items = BlWarehouse.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                //case "LOCATIONMORAL": items = BlLocationMoral.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
                //case "STOCK": items = BlStock.LoadQs(userId, parameters, searchTerm, pageSize, pageNum, out count); break;
            }

            var toRet = new QuickSearchPagedResult { total = count };

            if (items != null)
                foreach (var item in items)
                {
                    toRet.results.Add(new QuickSearchResult
                    {
                        id = item.value,
                        text = item.label
                    });
                }
            return toRet;
        }

        #endregion QuickSearch

        public static string GetReceiptHeader(long branchId, int headerNum)
        {
            var branch = BlBranch.LoadSingle(Constants.SystemUser, branchId);

            var toRet = string.Empty;
            switch (branch.Id)
            {
                case 1:
                    {
                        if (headerNum == 2) { toRet = BlCode.LoadSingle(Constants.SystemUser, "Receipt_1", "Header2").Value1; } // "مصلحة الديوان"; }
                        if (headerNum == 3) { toRet = branch.Entity.ArFirstName; }
                        break;
                    }
                case 2:
                    {
                        if (headerNum == 2) { toRet = BlCode.LoadSingle(Constants.SystemUser, "Receipt_2", "Header2").Value1; } // "الفرع الفني"; }
                        if (headerNum == 3) { toRet = branch.Entity.ArFirstName; }
                        break;
                    }
                case 3:
                    {
                        if (headerNum == 2) { toRet = BlCode.LoadSingle(Constants.SystemUser, "Receipt_3", "Header2").Value1; } // "مصلحة الديوان"; }
                        if (headerNum == 3) { toRet = branch.Entity.ArFirstName; }
                        break;
                    }
            }
            return toRet;
        }

        public static long GetIdFromReference(string reference)
        { return Convert.ToInt64(reference.Split('_')[1]); }

        public static List<FilterToken> GetFilterTokens(string filter)
        {
            var listToken = filter.Split(' ').ToList();
            var toRet = listToken.Select(t => new FilterToken
            {
                Token = t,
                TokenType = t.GetTokenType()
            }).ToList();
            return toRet;
        }

        public static List<GeneralSearchVm> GeneralSearch(long userId, string type, string filter)
        {
            throw new NotImplementedException();
            //switch (type)
            //{
            //    case "ORDER":
            //        { return BlOrder.GeneralSearch(userId, filter); }
            //    case "DISTRIBUTION":
            //        { return BlDistribution.GeneralSearch(userId, filter); }
            //    case "OPERATION":
            //        { return BlOperation.GeneralSearch(userId, filter); }
            //    case "INVENTORY":
            //        { return BlInventory.GeneralSearch(userId, filter); }
            //    case "STOCK":
            //    case "BARCODE":
            //    case "SERIAL":
            //        { return BlStock.GeneralSearch(userId, type, filter); }
            //    default:
            //        {
            //            var toRet = new List<GeneralSearchVm>();
            //            List<GeneralSearchVm> res;

            //            res = BlStock.GeneralSearch(userId, type, filter);
            //            if (res != null) toRet.AddRange(res);

            //            res = BlOrder.GeneralSearch(userId, filter);
            //            if (res != null) toRet.AddRange(res);

            //            res = BlDistribution.GeneralSearch(userId, filter);
            //            if (res != null) toRet.AddRange(res);

            //            res = BlOperation.GeneralSearch(userId, filter);
            //            if (res != null) toRet.AddRange(res);

            //            res = BlInventory.GeneralSearch(userId, filter);
            //            if (res != null) toRet.AddRange(res);

            //            return toRet;
            //        }
            //}
        }
    }
}