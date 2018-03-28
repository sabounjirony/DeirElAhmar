using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System.Logging;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System.Logging
{
    public class LogErrorRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public LogError Create(LogError toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public LogError Edit(LogError toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(LogError toDelete)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Delete(toDelete);
                _session.Flush();
                return true;
            }
        }

        public bool DeleteAll()
        {
            using (_session = DbManager.GetSession())
            {
                _session.CreateSQLQuery("truncate table _LogError").ExecuteUpdate();
                _session.Flush();
                return true;
            }
        }
        #endregion Actions

        #region Queries

        public LogError LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<LogError>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<LogError> LoadSearch(Expression<Func<LogError, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<LogError>()
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

        public List<LogError> LoadPaging(Expression<Func<LogError, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<LogError>().Where(predicate).Count();

                var toRet = _session.Query<LogError>()
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