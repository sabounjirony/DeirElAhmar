using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using CheckEmpty = Tools.Utilities.CheckEmpty;
using DateUtilities = Tools.Utilities.DateUtilities;
using PredicateBuilder = Tools.Utilities.PredicateBuilder;
using BL.Setup;

namespace BL.System.Logging
{
    [BlAspect(AspectPriority = 2)]
    public class BlLog
    {
        #region Members

        private const string Module = "EventLog";
        readonly LogRepository _repository = new LogRepository();

        #endregion Members

        #region Actions

        [BlAspect(Module = Module)]
        public Log Create(long userId, Log toAdd)
        {
            using (var tran = new TransactionScope())
            {
                toAdd.UserId = userId;
                toAdd.BranchId = BlUser.LoadSingle(toAdd.UserId).BranchId;
                toAdd.EntryDate = BlCommon.GetServerDateTime();
                var toRet = _repository.Create(toAdd);

                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module)]
        public Log Edit(long userId, Log toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module)]
        public bool Delete(long userId, Log toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                tran.Complete();
                return toRet;
            }
        }

        public static void Log(long userId, string moduleId, string action, string text, object[] args = null)
        {
            //Leave if module logging not enabled
            var module = BlModule.LoadSingle(userId, moduleId);
            if (module != null)
            {
                if (!module.EnableLogging) return;
            }

            var log = new Log
            {
                Module = module,
                Action = action,
                Text = BlDescription.GetDescription(Enumerations.DescriptionResources.Exceptions, text, 1, true) ?? text
            };

            if (args != null)
            {
                try
                {
                    log.Text = string.Format(log.Text, args);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            var blLog = new BlLog();
            blLog.Create(userId, log);
        }

        #endregion Actions

        #region Queries

        [BlAspect(Module = Module)]
        public Log LoadSingle(long userId, long pin)
        {
            var toRet = _repository.LoadSingle(pin);
            return toRet;
        }

        [BlAspect(Module = Module)]
        public IEnumerable<Log> LoadSearch(long userId, Expression<Func<Log, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        [BlAspect(Module = Module)]
        public IEnumerable<Log> LoadPaging(long userId, Expression<Func<Log, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        [BlAspect(Module = Module)]
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
                           ModuleId = r.Module.Id,
                           r.Action,
                           Text = r.Text ?? "",
                           User = r.UserId == 0 ? "" : BlUser.LoadSingle(r.UserId).UserName,
                           branch = BlBranch.GetBranchName(user.Id, r.BranchId),
                           LogDate = r.EntryDate.ToString(true)
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        [BlAspect(Module = Module)]
        public EventVm Init(long userId, long id)
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            var eventLog = LoadSingle(userId, id);
            var toRet = new EventVm
            {
                Module = eventLog.Module.Id,
                Action = eventLog.Action,
                Text = eventLog.Text,
                UserName = BlUser.LoadSingle(eventLog.UserId).UserName,
                Branch = BlBranch.GetBranchName(user.Id, eventLog.BranchId),
                Date = eventLog.EntryDate.ToString(true)
            };
            return toRet;
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Log, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Log>();

            if (CheckEmpty.String(ref dict, "Module") != "")
                predicate = predicate.And(p => p.Module.Id == dict["Module"].ToString());

            if (CheckEmpty.String(ref dict, "Action") != "")
                predicate = predicate.And(p => p.Action.Contains(dict["Action"].ToString()));

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
