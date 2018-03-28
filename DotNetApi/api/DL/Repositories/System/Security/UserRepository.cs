using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System.Security;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System.Security
{
    public class UserRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public User Create(User toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public User Edit(User toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(User toDelete)
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

        public User LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<User>()
                    .Where(x => x.Id == id)
                    .Fetch(u => u.Roles)
                    .AsEnumerable()
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<User> LoadSearch(Expression<Func<User, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<User>()
                    .OrderBy(x => x.Id)
                    .Where(predicate)
                    .Fetch(u => u.Roles).AsEnumerable();
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<User> LoadPaging(Expression<Func<User, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<User>().Where(predicate).Count();

                var toRet = _session.Query<User>()
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