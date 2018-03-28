using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System.Logging;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System.Logging
{
    public class LogRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Log Create(Log toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Log Edit(Log toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Log toDelete)
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

        public Log LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Log>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Log> LoadSearch(Expression<Func<Log, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Log>()
                    .OrderByDescending(x => x.EntryDate)
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<Log> LoadPaging(Expression<Func<Log, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Log>().Where(predicate).Count();

                var toRet = _session.Query<Log>()
                    .Where(predicate)
                    .OrderByDescending(x => x.EntryDate)
                    .Take(pageSize)
                    .Skip(pageNum * pageSize);

                _session.Flush();
                return toRet.ToList();
            }

        }

        #endregion Queries
    }
}