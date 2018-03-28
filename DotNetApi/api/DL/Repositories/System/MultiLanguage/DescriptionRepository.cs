using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System.MultiLanguage;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System.MultiLanguage
{
    public class DescriptionRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Description Create(Description toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Description Edit(Description toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Description toDelete)
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

        public Description LoadSingle(string code, string parent, int languageId)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Description>()
                    .Where(x => x.Code == code && x.Parent == parent && x.LanguageId == languageId)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public Description LoadSingle(long id)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Description>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Description> LoadSearch(Expression<Func<Description, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Description>()
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

        public List<Description> LoadPaging(Expression<Func<Description, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Description>().Where(predicate).Count();

                var toRet = _session.Query<Description>()
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