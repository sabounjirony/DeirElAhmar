using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.Setup;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.Setup
{
    public class EntityRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Entity Create(Entity toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Entity Edit(Entity toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Entity toDelete)
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

        public Entity LoadSingle(long pin)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Entity>()
                                    .Where(x => x.Pin == pin)
                                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Entity> LoadSearch(Expression<Func<Entity, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Entity>()
                    .OrderBy(x => x.Pin)
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<Entity> LoadPaging(Expression<Func<Entity, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Entity>().Where(predicate).Count();

                var toRet = _session.Query<Entity>()
                    .Where(predicate)
                    .OrderBy(x => x.Pin)
                    .Take(pageSize)
                    .Skip(pageNum * pageSize);

                _session.Flush();
                return toRet.ToList();
            }

        }

        #endregion Queries
    }
}