using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System.Security;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System.Security
{
    public class PermissionRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Permission Create(Permission toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Permission Edit(Permission toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.SaveOrUpdate(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Permission toDelete)
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

        public Permission LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Permission>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Permission> LoadSearch(Expression<Func<Permission, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Permission>()
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

        public List<Permission> LoadPaging(Expression<Func<Permission, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Permission>().Where(predicate).Count();

                var toRet = _session.Query<Permission>()
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