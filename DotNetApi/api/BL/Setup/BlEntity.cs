using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System.Logging;
using BL.System.Security;
using DL.Repositories.Setup;
using DM.Models.Setup;
using Tools;
using Tools.Utilities;
using VM.Ddl;
using VM.Setup;
using VM.Grid;
using BL.System;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlEntity
    {
        #region Members

        private const string Module = "Entity";
        readonly EntityRepository _repository = new EntityRepository();

        #endregion Members

        #region Actions

        public Entity Create(long userId, Entity toAdd, Address address = null)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                if (address != null)
                {
                    address.Entity = toRet;
                    //Create related address
                    var blAddress = new BlAddress();
                    blAddress.Create(userId, address);
                }

                BlLog.Log(userId, Module, "Create entity", "EntityCreated", new object[] { toAdd.Pin });
                tran.Complete();
                return toRet;
            }
        }

        public Entity Edit(long userId, Entity toEdit, Address address = null)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                if (address != null)
                {
                    if (address.Id != 0)
                    {
                        address.Entity = toRet;
                        //Edit related address
                        var blAddress = new BlAddress();
                        blAddress.Edit(userId, address);
                    }
                }

                BlLog.Log(userId, Module, "Edit entity", "EntityModified", new object[] { toEdit.Pin });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Entity toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete entity", "EntityDeleted", new object[] { toDelete.Pin });
                tran.Complete();
                return toRet;
            }
        }


        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, long toDeletePin)
        {
            using (var tran = new TransactionScope())
            {
                var toDelete = LoadSingle(userId, toDeletePin);
                var toRet = Delete(userId, toDelete);

                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Save")]
        public EntityVm Save(long userId, EntityVm toSave)
        {
            var entity = toSave.Entity;
            PreSave(userId, ref entity, toSave.ActionMode);
            toSave.Entity = entity;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Entity = Create(userId, toSave.Entity);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Entity = Edit(userId, toSave.Entity);
                    break;
            }

            return Init(userId, toSave.Entity.Pin);
        }

        public void ReIndex(long userId)
        {
            var predicate = PredicateBuilder.True<Entity>();
            var entities = LoadSearch(userId, predicate);
            foreach (var entity in entities)
            {
                entity.NameIndex = NameIndex.GetNameIndex(userId, entity.FirstName, entity.FamilyName, entity.FatherName, DateUtilities.IsDate(entity.Dob.ToString()) ? CheckEmpty.Date(entity.Dob).Year : 0);
                entity.NameIndex += NameIndex.GetNameIndex(userId, entity.ArFirstName, entity.ArFamilyName, entity.ArFatherName, DateUtilities.IsDate(entity.Dob.ToString()) ? CheckEmpty.Date(entity.Dob).Year : 0);
                Edit(userId, entity);
            }
        }

        #endregion Actions

        #region Queries

        public static Entity LoadSingle(long userId, long pin)
        {
            var repository = new EntityRepository();
            var toRet = repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<Entity> LoadSearch(long userId, Expression<Func<Entity, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Entity> LoadPaging(long userId, Expression<Func<Entity, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public IEnumerable<Entity> LoadMultiple(long userId, List<long> pins)
        {
            var predicate = PredicateBuilder.True<Entity>();
            predicate = predicate.And(p => pins.Contains(p.Pin));

            var toRet = _repository.LoadSearch(predicate);
            return toRet;
        }

        public GridResults LoadPaging(long userId, string search, int pageIndex, out long totalRecords, string sortColumnName = "", string sortOrderBy = "")
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            //Query paged data
            var results = LoadPaging(userId, CreateFilter(search), user.PageSize, pageIndex - 1, out totalRecords);

            //Convert results into display model
            var res = (from r in results
                       select new
                       {
                           r.Pin,
                           r.FullEnLongName,
                           name= FormatFullName(r),
                           EntryDate = r.EntryDate.ToString(true),
                           Status = r.Status == "A" ? "check colorGreen" : "close colorRed"
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public EntityVm Init(long userId, long? pin)
        {
            var toRet = new EntityVm
            {
                ActionMode = Enumerations.ActionMode.Add,
                Entity = new Entity { Status = "A" }
            };

            if (pin != null)
            {
                var entity = LoadSingle(userId, Convert.ToInt64(pin));
                toRet.Entity = entity;

                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public static List<DdlVm.DdlOption> LoadDdl(long userId, string parameters, string searchTerm, int pageSize, int pageNum, out long count)
        {
            var user = BlUser.LoadSingle(userId);
            var blObject = new BlEntity();

            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var isActive = CheckEmpty.Boolean(ref dict, "isActive");
            var gender = CheckEmpty.String(ref dict, "Gender");

            var predicate = PredicateBuilder.True<Entity>();

            if (isActive)
            { predicate = predicate.And(c => c.Status == "A"); }

            if (gender != "")
            {
                predicate = predicate.And(c => gender.Split(',').Contains(c.Gender));
            }

            if (CheckEmpty.String(searchTerm) != "")
            {
                var predicate2 = PredicateBuilder.False<Entity>();
                predicate2 = predicate2.Or(m => m.FirstName.Contains(searchTerm));
                predicate2 = predicate2.Or(m => m.FatherName.Contains(searchTerm));
                predicate2 = predicate2.Or(m => m.FamilyName.Contains(searchTerm));
                predicate2 = predicate2.Or(m => m.ArFirstName.Contains(searchTerm));
                predicate2 = predicate2.Or(m => m.ArFatherName.Contains(searchTerm));
                predicate2 = predicate2.Or(m => m.ArFamilyName.Contains(searchTerm));
                predicate = predicate.And(predicate2);
            }

            var items = blObject.LoadPaging(userId, predicate, pageSize, (pageNum - 1), out count);

            return items.Select(i => FormatForDdl(user.LanguageId, i)).ToList();
        }

        public static DdlVm.DdlOption LoadDdl(long userId, long pin)
        {
            var user = BlUser.LoadSingle(userId);
            return FormatForDdl(user.LanguageId, LoadSingle(userId, pin));
        }

        public static string FormatFullName(Entity entity, bool withMilitaryId = false)
        {
            var toRet = string.Empty;

            //Title
            if (CheckEmpty.String(entity.Title) != "")
            {
                toRet = BlCode.LoadSingle(Constants.SystemUser, "Title", entity.Title).Value1 + " ";
            }

            //Full arabic name
            toRet += entity.FullArLongName;

            //Military number if exists
            if (withMilitaryId)
            {
                if (CheckEmpty.String(entity.IdType) != "" && CheckEmpty.String(entity.IdNum) != "")
                {
                    if (CheckEmpty.String(entity.IdType) == "M")
                    { toRet += " " + entity.IdNum; }
                }
            }

            return toRet;
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Entity, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Entity>();

            if (CheckEmpty.String(ref dict, "Pin") != "")
                predicate = predicate.And(p => p.Pin == Convert.ToInt64(dict["Pin"]));

            if (CheckEmpty.String(ref dict, "Gender") != "")
                predicate = predicate.And(p => Convert.ToString(dict["Gender"]).Split(',').Contains(p.Gender));

            return predicate;
        }

        private static void PreSave(long userId, ref Entity toSave, Enumerations.ActionMode action)
        {
            if (action == Enumerations.ActionMode.Add)
            {
                toSave.EntryDate = BlCommon.GetServerDateTime();
            }
            toSave.UserId = userId;
        }

        private static DdlVm.DdlOption FormatForDdl(int languageId, Entity item)
        {
            var toRet = new DdlVm.DdlOption
            {
                value = item.Pin.ToUiString(),
                label = languageId == 1 ? item.FullEnShortName : item.FullArShortName
            };
            return toRet;
        }

        #endregion Private methods
    }
}
