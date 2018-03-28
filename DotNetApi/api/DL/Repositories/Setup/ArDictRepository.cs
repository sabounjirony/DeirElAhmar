using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.Setup;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.Setup
{
    public class ArDictRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public ArDict Create(ArDict toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public ArDict Edit(ArDict toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(ArDict toDelete)
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

        public ArDict LoadSingle(string token)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<ArDict>()
                    .Where(x => x.Token == token)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<ArDict> LoadSearch(Expression<Func<ArDict, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<ArDict>()
                    .OrderByDescending(i => i.Occurance)
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<ArDict> LoadPaging(Expression<Func<ArDict, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<ArDict>().Where(predicate).Count();

                var toRet = _session.Query<ArDict>()
                    .Where(predicate)

                    .Take(pageSize)
                    .Skip(pageNum * pageSize);

                _session.Flush();
                return toRet.ToList();
            }

        }

        #endregion Queries
    }
}
