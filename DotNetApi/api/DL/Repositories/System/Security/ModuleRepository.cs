using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System.Security;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System.Security
{
    public class ModuleRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Module Create(Module toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Module Edit(Module toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.SaveOrUpdate(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Module toDelete)
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

        public Module LoadSingle(string id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Module>()
                                    .Where(x => x.Id == id)
                                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Module> LoadSearch(Expression<Func<Module, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Module>()
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

        public List<Module> LoadPaging(Expression<Func<Module, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Module>().Where(predicate).Count();

                var toRet = _session.Query<Module>()
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