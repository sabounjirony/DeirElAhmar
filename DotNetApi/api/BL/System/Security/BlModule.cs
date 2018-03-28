using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.Setup;
using BL.System.Logging;
using DL.Repositories.System.Security;
using DM.Models.System.Security;
using Tools;
using Tools.Cache;
using Tools.Utilities;
using VM.Ddl;
using VM.Grid;
using VM.System.Security;

namespace BL.System.Security
{
    [BlAspect(AspectPriority = 2)]
    public class BlModule
    {
        #region Members

        private const string Module = "Module";
        readonly ModuleRepository _repository = new ModuleRepository();

        #endregion Members

        #region Actions

        public Module Create(long userId, Module toAdd)
        {
            using (var tran = new TransactionScope())
            {
                PreSave(userId, ref toAdd, Enumerations.ActionMode.Add);
                var toRet = _repository.Create(toAdd);

                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Create module", "ModuleCreated", new object[] { toAdd.Id });
                tran.Complete();
                return toRet;
            }
        }

        public Module Edit(long userId, Module toEdit)
        {
            using (var tran = new TransactionScope())
            {
                PreSave(userId, ref toEdit, Enumerations.ActionMode.Edit);
                var toRet = _repository.Edit(toEdit);

                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Edit module", "ModuleModified", new object[] { toEdit.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Module toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Delete module", "ModuleDeleted", new object[] { toDelete.Id });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, string toDeleteId)
        {
            using (var tran = new TransactionScope())
            {
                var toDelete = LoadSingle(userId, toDeleteId);
                var toRet = Delete(userId, toDelete);

                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Save")]
        public ModuleVm Save(long userId, ModuleVm toSave)
        {
            var module = toSave.Module;
            PreSave(userId, ref module, toSave.ActionMode);
            toSave.Module = module;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Module = Create(userId, toSave.Module);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Module = Edit(userId, toSave.Module);
                    break;
            }

            return Init(userId, toSave.Module.Id);
        }

        #endregion Actions

        #region Queries

        public IEnumerable<Module> LoadAll(long userId)
        {
            var predicate = PredicateBuilder.True<Module>();
            var toRet = LoadSearch(userId, predicate);
            return toRet;
        }

        public static Module LoadSingle(long userId, string id)
        {
            Module toRet;
            if (!CacheHelper.Get(Module + "_" + id.ToUpper(), out toRet))
            {
                var moduleRepository = new ModuleRepository();
                toRet = moduleRepository.LoadSingle(id);
                if (toRet != null)
                {
                    CacheHelper.Add(Module + "_" + toRet.Id, toRet, BlCommon.DefaultTimeOut());
                }
            }
            return toRet;
        }

        public IEnumerable<Module> LoadSearch(long userId, Expression<Func<Module, bool>> predicate, int count = 0)
        {
            IEnumerable<Module> toRet;
            if (!CacheHelper.Get(Module + "_" + Evaluator.PartialEval(predicate), out toRet))
            {
                toRet = _repository.LoadSearch(predicate, count);
                CacheHelper.Add(Module + "_" + Evaluator.PartialEval(predicate), toRet, BlCommon.DefaultTimeOut());
            }
            return toRet;
        }

        public IEnumerable<Module> LoadPaging(long userId, Expression<Func<Module, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public GridResults LoadPaging(long userId, string search, int pageIndex, out long totalRecords, string sortColumnName = "", string sortOrderBy = "")
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            //Query paged data
            var results = LoadPaging(userId, CreateFilter(search), user.PageSize, pageIndex - 1, out totalRecords);

            //Convert results into display model
            var i = 0;
            var res = (from r in results
                       select new
                       {
                           Id = i++,
                           Module = r.Id,
                           Path = r.Path ?? "",
                           r.Description,
                           EntryDate = r.EntryDate.ToString(true),
                           Logging = r.EnableLogging ? "eye" : "eye-slash colorRed",
                           Status = r.Status == "A" ? "check colorGreen" : "close colorRed"
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public ModuleVm Init(long userId, string id)
        {
            var toRet = new ModuleVm
            {
                Branches = BlBranch.GetLov(userId).ToDictionary(i => i.value, i => i.label),
                Statuses = BlCode.LoadTable(userId, "Status"),
                Module = new Module { Status = "A" },
                ActionMode = Enumerations.ActionMode.Add
            };

            if (id != "null")
            {
                var module = LoadSingle(userId, id);
                toRet.Module = module;
                //module.User.Roles = null;
                //module.Author.Roles = null;
                toRet.Signature = BlCommon.GetSignature(toRet.Module.UserId, toRet.Module.EntryDate);
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public string GetSecurityString(long userId, string path)
        {
            //CLean path url
            path = path.ToUpper().Replace("../STOCK/APP/", "");
            path = path.ToUpper().Replace("/STOCK/APP/", "");

            var toRetList = new ArrayList();

            //Load the module by path
            var predicate = PredicateBuilder.True<Module>();
            predicate = predicate.And(p => p.Path == path);
            var module = LoadSearch(userId, predicate).FirstOrDefault();

            //Create module if does not exist
            if (module == null)
            {
                using (var tran = new TransactionScope())
                {
                    var systemUser = BlUser.LoadSingle(Constants.SystemUser);
                    var newModule = new Module
                    {
                        Id = path.Split('/')[path.Split('/').Length - 1].ToUpper().Replace(".HTML", ""),
                        Path = path,
                        Description = "N/A",
                        Author = Constants.SystemUser,
                        Status = "A"
                    };
                    newModule = Create(userId, newModule);

                    //Create first access permission and assign to system admin
                    var newPermission = new Permission
                    {
                        Module = newModule,
                        Code = "Access",
                        Status = "A",
                        Roles =
                                                    BlRole.LoadSingle(userId, Constants.FullAdminRole).
                                                    SingleItemAsEnumerable().ToList()
                    };
                    var blPermission = new BlPermission();
                    blPermission.Create(userId, newPermission);
                    tran.Complete();
                }
            }

            //Get module permissions if exists
            if (module != null)
            {
                var blPermission = new BlPermission();
                var permissions = blPermission.LoadByModule(userId, module.Id, true);

                //Check each permission status
                foreach (var permission in permissions)
                {
                    if (BlPermission.CanDo(userId, module.Id, permission.Code))
                        toRetList.Add(permission.Code);
                }
            }

            return string.Join(":", toRetList.ToArray());
        }

        #endregion Queries

        #region QuickSearch

        public static List<DdlVm.DdlOption> LoadQs(long userId, string parameters, string searchTerm, int pageSize, int pageNum, out long count)
        {
            var blObject = new BlModule();

            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var isActive = CheckEmpty.Boolean(ref dict, "isActive");

            var predicate = PredicateBuilder.True<Module>();

            if (isActive)
            {
                predicate = predicate.And(c => c.Status == "A");
            }

            if (CheckEmpty.String(searchTerm) != "")
            {
                var tokens = searchTerm.Tokens();
                foreach (var token in tokens)
                {
                    var predicate2 = PredicateBuilder.False<Module>();
                    predicate2 = predicate2.Or(m => m.Id.Contains(token));
                    predicate = predicate.And(predicate2);
                }
            }

            var items = blObject.LoadPaging(userId, predicate, pageSize, (pageNum - 1), out count);

            return items.Select(FormatForQs).ToList();
        }

        public static DdlVm.DdlOption LoadQs(long userId, string id)
        {
            return FormatForQs(LoadSingle(userId, id));
        }

        private static DdlVm.DdlOption FormatForQs(Module item)
        {
            var toRet = new DdlVm.DdlOption
            {
                value = item.Id,
                label = item.Id
            };
            return toRet;
        }

        #endregion QuickSearch

        #region Private methods

        private static Expression<Func<Module, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Module>();

            if (CheckEmpty.String(ref dict, "Module") != "")
                predicate = predicate.And(p => p.Id == dict["Module"].ToString());

            return predicate;
        }

        private static void PreSave(long userId, ref Module toSave, Enumerations.ActionMode action)
        {
            if (action == Enumerations.ActionMode.Add)
            {
                toSave.Author = userId;
                toSave.EntryDate = BlCommon.GetServerDateTime();
            }
            toSave.UserId = userId;

        }

        #endregion Private methods
    }
}
