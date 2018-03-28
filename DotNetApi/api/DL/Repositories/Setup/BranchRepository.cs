using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.Setup;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.Setup
{
    public class BranchRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Branch Create(Branch toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Branch Edit(Branch toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Branch toDelete)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Delete(toDelete);
                _session.Flush();
                return true;
            }
        }

        #endregion Actions

        #region Queries

        public Branch LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Branch>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Branch> LoadSearch(Expression<Func<Branch, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Branch>()
                    .OrderBy(x => x.Id)
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<Branch> LoadPaging(Expression<Func<Branch, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Branch>().Where(predicate).Count();

                var toRet = _session.Query<Branch>()
                    .Where(predicate)
                    .OrderBy(x => x.Id)
                    .Take(pageSize)
                    .Skip(pageNum * pageSize);

                _session.Flush();
                return toRet.ToList();
            }

        }

        #endregion Queries
    }
}