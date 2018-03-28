using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using DM.Models.System;

namespace DL.Repositories.System
{
    public class HelpRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Help Create(Help toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Help Edit(Help toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.SaveOrUpdate(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Help toDelete)
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

        public Help LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Help>()
                                    .Where(x => x.Id == id)
                                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Help> LoadSearch(Expression<Func<Help, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Help>()
                    .OrderBy(x => x.DisplayOrder)
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<Help> LoadPaging(Expression<Func<Help, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Help>().Where(predicate).Count();

                var toRet = _session.Query<Help>()
                    .Where(predicate)
                    .OrderBy(x => x.DisplayOrder)
                    .Take(pageSize)
                    .Skip(pageNum * pageSize);

                _session.Flush();
                return toRet.ToList();
            }

        }

        #endregion Queries
    }
}