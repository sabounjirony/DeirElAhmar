using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using BL.System.Logging;
using DL.Repositories.Setup;
using DM.Models.Setup;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlCompany
    {
        #region Members

        private const string Module = "Company";
        readonly CompanyRepository _repository = new CompanyRepository();

        #endregion Members

        #region Actions

        public Company Create(long userId, Company toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Delete company", "CompanyCreated", new object[] { toAdd.Id });
                tran.Complete();
                return toRet;
            }
        }

        public Company Edit(long userId, Company toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Delete company", "CompanyModified", new object[] { toEdit.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Company toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete company", "CompanyDeleted", new object[] { toDelete.Id });
                tran.Complete();
                return toRet;
            }
        }

        #endregion Actions

        #region Queries

        public static Company LoadSingle(long userId, long id)
        {
            var repository = new CompanyRepository();
            var toRet = repository.LoadSingle(id);
            return toRet;
        }

        public IEnumerable<Company> LoadSearch(long userId, Expression<Func<Company, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Company> LoadPaging(long userId, Expression<Func<Company, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        #endregion Queries
    }
}