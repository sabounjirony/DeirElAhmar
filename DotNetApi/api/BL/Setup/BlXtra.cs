using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System;
using BL.System.Logging;
using BL.System.MultiLanguage;
using BL.System.Security;
using DL.Repositories.Setup;
using DM.Models.Setup;
using Tools;
using Tools.Utilities;
using VM.Grid;
using VM.Setup;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlXtra
    {
        #region Members

        readonly XtraRepository _repository = new XtraRepository();

        #endregion Members

        #region Actions

        public Xtra Create(long userId, Xtra toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                //BlLog.Log(userId, Module, "Delete company", "CompanyCreated", new object[] { toAdd.Id });
                tran.Complete();
                return toRet;
            }
        }

        public Xtra Edit(long userId, Xtra toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                //BlLog.Log(userId, Module, "Delete company", "CompanyModified", new object[] { toEdit.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Xtra toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                //BlLog.Log(userId, Module, "Delete company", "CompanyDeleted", new object[] { toDelete.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, string Object, long toDeleteId, string property)
        {
            using (var tran = new TransactionScope())
            {
                var toDelete = LoadSingle(userId, Object, toDeleteId, property);
                Delete(userId, toDelete);
                tran.Complete();
                return true;
            }
        }

        public XtraVm Save(long userId, XtraVm toSave)
        {
            var obj = toSave.Xtra;
            PreSave(userId, ref obj, toSave.ActionMode);
            toSave.Xtra = obj;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Xtra = Create(userId, toSave.Xtra);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Xtra = Edit(userId, toSave.Xtra);
                    break;
            }

            return Init(userId, toSave.Xtra.Object, toSave.Xtra.Id, toSave.Xtra.Property);
        }

        #endregion Actions

        #region Queries

        public static Xtra LoadSingle(long userId, string Object, long id, string property)
        {
            var predicate = PredicateBuilder.True<Xtra>();
            predicate = predicate.And(p => p.Object == Object);
            predicate = predicate.And(p => p.Id == id);
            predicate = predicate.And(p => p.Property == property);

            var blXtra = new BlXtra();
            var toRet = blXtra.LoadSearch(userId, predicate, 1).FirstOrDefault();

            return toRet;
        }

        public IEnumerable<Xtra> LoadSearch(long userId, Expression<Func<Xtra, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Xtra> LoadPaging(long userId, Expression<Func<Xtra, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public XtraVm Init(long userId, string Object, long id, string property)
        {
            long branchId = 0;
            var toRet = new XtraVm
            {
                ActionMode = Enumerations.ActionMode.Add,
                Properties = BlCode.LoadTable(userId, "ProductOpenProperty_" + branchId, "", branchId.ToString()),
                Xtra = new Xtra { Object = Object, Id = id }
            };

            if (property != "null")
            {
                var obj = LoadSingle(userId, Object, id, property);
                toRet.Xtra = obj;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public GridResults LoadPaging(long userId, string search, int pageIndex, out long totalRecords, string sortColumnName = "", string sortOrderBy = "")
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            //Query paged data
            var results = LoadSearch(userId, CreateFilter(search));
            totalRecords = results.Count();

            //Convert results into display model
            var res = (from r in results
                       select new
                       {
                           r.Id,
                           Name = r.Property,
                           r.Value
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Xtra, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Xtra>();

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            if (CheckEmpty.String(ref dict, "Object") != "")
                predicate = predicate.And(p => p.Object == dict["Object"].ToString());

            return predicate;
        }

        private static void PreSave(long userId, ref Xtra toSave, Enumerations.ActionMode action)
        {
            var user = BlUser.LoadSingle(userId);
            if (action == Enumerations.ActionMode.Add)
            {
                //Check if previously exists
                if (LoadSingle(userId, toSave.Object, toSave.Id, toSave.Property) != null)
                    throw new BusinessException("AlreadyExists1", BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblProperty", user.LanguageId));
            }
        }
        #endregion Private methods
    }
}