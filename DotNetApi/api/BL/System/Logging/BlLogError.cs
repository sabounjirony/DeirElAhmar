using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System.MultiLanguage;
using BL.System.Security;
using DL.Repositories.System.Logging;
using DM.Models.System.Logging;
using Tools;
using Tools.Utilities;
using VM.Grid;
using VM.System.Logging;
using BL.Setup;

namespace BL.System.Logging
{
    [BlAspect(AspectPriority = 2)]
    public class BlLogError
    {
        #region Members

        readonly LogErrorRepository _repository = new LogErrorRepository();

        #endregion Members

        #region Actions

        public LogError Create(long userId, LogError toAdd)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                toAdd.UserId = userId;
                toAdd.BranchId = BlUser.LoadSingle(toAdd.UserId, true).BranchId;
                toAdd.EntryDate = BlCommon.GetServerDateTime();
                var toRet = _repository.Create(toAdd);

                tran.Complete();
                return toRet;
            }
        }

        public LogError Edit(long userId, LogError toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, LogError toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, long id)
        {
            using (var tran = new TransactionScope())
            {
                var toDelete = LoadSingle(userId, id);
                var toRet = Delete(userId, toDelete);

                tran.Complete();
                return toRet;
            }
        }

        public bool DeleteAll(long userId)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.DeleteAll();
                tran.Complete();
                return toRet;
            }
        }

        public static void HandleError(long userId, ref BusinessException ex, MethodBase method, bool rethrow = true)
        {
            var message = BlDescription.GetDescription(Enumerations.DescriptionResources.Exceptions, ex.ExceptionCode);
            if (ex.ExtraVariables != null)
            {
                foreach (var variable in ex.ExtraVariables.Split('_'))
                {
                    message = message.Replace("{" + ex.ExtraVariables.Split('_').ToList().IndexOf(variable) + "}",
                                            BlDescription.GetDescription(Enumerations.DescriptionResources.Language,
                                                                         variable));
                }
            }
            var exception = (Exception)ex;
            LogError(GetExceptionSource(ref method, ref exception), message, ex.Type, ex.Severity, userId);
            if (rethrow) throw ex;
        }

        public static void HandleError(long userId, Exception ex, MethodBase method, bool rethrow = true)
        {
            var tmpException = ex;
            while (tmpException != null)
            {
                LogError(method.Name, tmpException.Message, Enumerations.ErrorType.Error, Enumerations.ErrorSeverity.High, userId);
                tmpException = tmpException.InnerException;
            }

            if (rethrow) throw ex;
        }

        #endregion Actions

        #region Queries

        public LogError LoadSingle(long userId, long pin)
        {
            var toRet = _repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<LogError> LoadSearch(long userId, Expression<Func<LogError, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<LogError> LoadPaging(long userId, Expression<Func<LogError, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }


        public GridResults LoadPaging(long userId, string search, int pageIndex, out long totalRecords, string sortColumnName = "", string sortOrderBy = "")
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            //Query paged data
            var results = LoadPaging(userId, CreateFilter(search), user.PageSize, pageIndex - 1, out totalRecords);

            //Convert results into display model
            var res = (from r in results
                       select new
                       {
                           r.Id,
                           Type = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "ErrorType", r.Type.ToString())),
                           Severity = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "ErrorSeverity", r.Severity.ToString())),
                           Source = r.Source ?? "",
                           Text = r.Text ?? "",
                           User = r.UserId == 0 ? "" : BlUser.LoadSingle(r.UserId).UserName,
                           branch = BlBranch.GetBranchName(user.Id, r.BranchId),
                           LogDate = r.EntryDate.ToString(true)
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public ErrorVm Init(long userId, long id)
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            var error = LoadSingle(userId, id);
            var toRet = new ErrorVm
            {
                Type = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "ErrorType", error.Type.ToUiString())),
                Severity = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "ErrorSeverity", error.Severity.ToUiString())),
                Source = error.Source,
                Text = error.Text,
                UserName = BlUser.LoadSingle(error.UserId).UserName,
                Branch = BlBranch.GetBranchName(user.Id, error.BranchId),
                Date = error.EntryDate.ToString(true)
            };
            return toRet;
        }

        #endregion Queries

        #region Private methods

        public static void LogError(string source, string text, Enumerations.ErrorType errorType = Enumerations.ErrorType.Error, Enumerations.ErrorSeverity errorSeverity = Enumerations.ErrorSeverity.High, long userId = Constants.SystemUser)
        {
            var logError = new LogError
            {
                Type = (int)errorType,
                Severity = (int)errorSeverity,
                Source = source,
                Text = text
            };
            var blLogError = new BlLogError();
            blLogError.Create(userId, logError);
        }

        private static string GetExceptionSource(ref MethodBase method, ref Exception ex)
        {
            var toRet = string.Empty;
            if (method.DeclaringType != null)
            {
                toRet = "Method: " + method.DeclaringType.FullName;
            }
            toRet += ", StackTrace: " + ex.StackTrace;

            return toRet;
        }

        private static Expression<Func<LogError, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<LogError>();

            if (CheckEmpty.String(ref dict, "Type") != "")
                predicate = predicate.And(p => p.Type == Convert.ToInt64(dict["Type"]));

            if (CheckEmpty.String(ref dict, "Severity") != "")
                predicate = predicate.And(p => p.Severity == Convert.ToInt64(dict["Severity"]));

            if (CheckEmpty.String(ref dict, "Text") != "")
                predicate = predicate.And(p => p.Text.Contains(dict["Text"].ToString()));

            if (CheckEmpty.String(ref dict, "UserId") != "")
                predicate = predicate.And(p => p.UserId == Convert.ToInt64(dict["UserId"]));

            if (CheckEmpty.String(ref dict, "BranchId") != "")
                predicate = predicate.And(p => p.BranchId == Convert.ToInt64(dict["BranchId"]));

            if (CheckEmpty.String(ref dict, "DateFilter") != "")
            {
                var dtLimits = DateUtilities.GetDateFilter(dict["DateFilter"].ToString(), dict["FromDate"].ToString(), dict["ToDate"].ToString());
                predicate = predicate.And(p => p.EntryDate >= dtLimits.FromDate);
                predicate = predicate.And(p => p.EntryDate <= dtLimits.ToDate);
            }

            return predicate;
        }

        #endregion Private methods
    }
}
