using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.Setup;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.Setup
{
    public class AntiDictRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public AntiDict Create(AntiDict toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public AntiDict Edit(AntiDict toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(AntiDict toDelete)
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

        public AntiDict LoadSingle(string token)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<AntiDict>()
                    .Where(x => x.Token == token)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<AntiDict> LoadSearch(Expression<Func<AntiDict, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<AntiDict>()
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<AntiDict> LoadPaging(Expression<Func<AntiDict, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<AntiDict>().Where(predicate).Count();

                var toRet = _session.Query<AntiDict>()
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
