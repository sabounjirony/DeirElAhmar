using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.Setup;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.Setup
{
    public class CompanyRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Company Create(Company toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Company Edit(Company toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Company toDelete)
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

        public Company LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Company>()
                                    .Where(x => x.Id == id)
                                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Company> LoadSearch(Expression<Func<Company, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Company>()
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

        public List<Company> LoadPaging(Expression<Func<Company, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Company>().Where(predicate).Count();

                var toRet = _session.Query<Company>()
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