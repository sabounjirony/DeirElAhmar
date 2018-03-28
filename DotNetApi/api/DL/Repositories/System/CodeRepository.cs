using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DM.Models.System;
using NHibernate;
using NHibernate.Linq;

namespace DL.Repositories.System
{
    public class CodeRepository
    {
        #region Members

        private ISession _session;

        #endregion

        #region Actions

        public Code Create(Code toAdd)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Save(toAdd);
                _session.Flush();
                return toAdd;
            }
        }

        public Code Edit(Code toEdit)
        {
            using (_session = DbManager.GetSession())
            {
                _session.Update(toEdit);
                _session.Flush();
                return toEdit;
            }
        }

        public bool Delete(Code toDelete)
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

        public Code LoadSingle(string tableName, string codeName)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Code>()
                    .Where(x => x.TableName == tableName && x.CodeName == codeName)
                    .SingleOrDefault();
                _session.Flush();
                return toRet;
            }
        }

        public List<Code> LoadSearch(Expression<Func<Code, bool>> predicate, int count = 0)
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Code>()
                    .OrderBy(x => x.TableName)
                    .ThenBy(x => x.DisplayOrder)
                    .Where(predicate);
                if (count != 0)
                {
                    toRet = toRet.Take(count);
                }
                _session.Flush();
                return toRet.ToList();
            }
        }

        public List<Code> LoadPaging(Expression<Func<Code, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {

            using (_session = DbManager.GetSession())
            {
                totCount = _session.Query<Code>().Where(predicate).Count();

                var toRet = _session.Query<Code>()
                    .Where(predicate)
                    .OrderBy(x => x.TableName)
                    .ThenBy(x => x.DisplayOrder)
                    .Take(pageSize)
                    .Skip(pageNum * pageSize);

                _session.Flush();
                return toRet.ToList();
            }

        }

        public List<string> LoadTables()
        {
            using (_session = DbManager.GetSession())
            {
                var toRet = _session.Query<Code>()
                    .OrderBy(x => x.TableName)
                    .Select(m => m.TableName)
                    .Distinct();
                _session.Flush();
                return toRet.ToList();
            }
        }

        #endregion Queries
    }
}