using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using BL.System.Logging;
using BL.System.Security;
using DL.Repositories.Setup;
using DM.Models.Setup;
using Tools.Utilities;
using VM.Ddl;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlBranch
    {
        #region Members

        private const string Module = "Branch";
        readonly BranchRepository _repository = new BranchRepository();

        #endregion Members

        #region Actions

        public Branch Create(long userId, Branch toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Delete branch", "BranchCreated", new object[] { toAdd.Id });
                tran.Complete();
                return toRet;
            }
        }

        public Branch Edit(long userId, Branch toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Delete branch", "BranchModified", new object[] { toEdit.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Branch toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete branch", "BranchDeleted", new object[] { toDelete.Id });
                tran.Complete();
                return toRet;
            }
        }

        #endregion Actions

        #region Queries

        public static Branch LoadSingle(long userId, long pin)
        {
            var repository = new BranchRepository();
            var toRet = repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<Branch> LoadSearch(long userId, Expression<Func<Branch, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Branch> LoadPaging(long userId, Expression<Func<Branch, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public static List<DdlVm.DdlOption> GetLov(long userId, bool isMandatory = false)
        {
            var user = BlUser.LoadSingle(userId);
            var canCrossBranch = BlPermission.CanDo(userId, Module, "CrossBranches");

            var blBranch = new BlBranch();
            var predicate = PredicateBuilder.True<Branch>();
            predicate = predicate.And(p => p.Status == "A");

            //Retrieve only user branch if cannot cross branch
            if (!canCrossBranch)
            { predicate = predicate.And(p => p.Id == user.BranchId); }

            var result = blBranch.LoadSearch(userId, predicate).ToList();
            if (!result.Any()) return null;

            var results = (from a in result
                           orderby a.Name ascending
                           select new DdlVm.DdlOption
                           {
                               value = a.Id.ToUiString(),
                               label = user.LanguageId == 1 ? a.Entity.FullEnShortName : a.Entity.FullArShortName
                           }).ToList();

            //Add empty value if can cross branch
            if (canCrossBranch && !isMandatory)
            { results.Insert(0, new DdlVm.DdlOption("...", "")); }

            return results.ToList();
        }

        public static string GetBranchName(long userId)
        {
            var user = BlUser.LoadSingle(userId);
            var branch = LoadSingle(user.Id, user.BranchId);
            if (user.LanguageId == 1)
            { return branch.Entity.FullEnShortName; }
            else
            { return branch.Entity.FullArShortName; }
        }

        public static string GetBranchName(long userId, long branchId)
        {
            var user = BlUser.LoadSingle(userId);
            var branch = BlBranch.LoadSingle(user.Id, branchId);
            if (user.LanguageId == 1)
            { return branch.Entity.FullEnShortName; }
            else
            { return branch.Entity.FullArShortName; }
        }
        #endregion Queries
    }
}