using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System.Logging;
using DL.Repositories.System.Security;
using DM.Models.System.Security;
using Tools;
using Tools.Utilities;
using VM.Grid;
using VM.System.Security;
using VM.Tree;

namespace BL.System.Security
{
    [BlAspect(AspectPriority = 2)]
    public class BlPermission
    {
        #region Members

        private const string Module = "Permission";
        readonly PermissionRepository _repository = new PermissionRepository();

        #endregion Members

        #region Actions

        public Permission Create(long userId, Permission toAdd)
        {
            using (var tran = new TransactionScope())
            {
                PreSave(userId, ref toAdd, Enumerations.ActionMode.Add);
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create permission", "PermissionCreated", new object[] { toAdd.Code, toAdd.Module.Id });
                tran.Complete();
                return toRet;
            }
        }

        public Permission Edit(long userId, Permission toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit permission", "PermissionModified", new object[] { toEdit.Id, toEdit.Module.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Permission toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete permission", "PermissionDeleted", new object[] { toDelete.Id, toDelete.Module.Id });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, long toDeleteId)
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
        public PermissionVm Save(long userId, PermissionVm toSave)
        {
            var permission = toSave.Permission;
            PreSave(userId, ref permission, toSave.ActionMode, toSave.RolesTree);
            toSave.Permission = permission;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Permission = Create(userId, toSave.Permission);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Permission = Edit(userId, toSave.Permission);
                    break;
            }

            return Init(userId, toSave.Permission.Id);
        }

        #endregion Actions

        #region Queries

        public IEnumerable<Permission> LoadAll(long userId)
        {
            var predicate = PredicateBuilder.True<Permission>();
            var toRet = LoadSearch(userId, predicate);
            return toRet;
        }

        public Permission LoadSingle(long userId, long id)
        {
            var toRet = _repository.LoadSingle(id);
            return toRet;
        }

        public IEnumerable<Permission> LoadSearch(long userId, Expression<Func<Permission, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Permission> LoadPaging(long userId, Expression<Func<Permission, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public GridResults LoadPaging(long userId, string search, int pageIndex, out long totalRecords, string sortColumnName = "", string sortOrderBy = "")
        {
            //Get current user
            var user = BlUser.LoadSingle(userId);

            //Query paged data
            var results = LoadPaging(userId, CreateFilter(search), Int16.MaxValue, pageIndex - 1, out totalRecords);

            //Convert results into display model
            var res = (from r in results
                       select new
                       {
                           r.Id,
                           r.Code,
                           EntryDate = r.EntryDate.ToString(true),
                           Status = r.Status == "A" ? "check colorGreen" : "close colorRed"
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, Int16.MaxValue, pageIndex, totalRecords);
        }

        public PermissionVm Init(long userId, long? id)
        {
            var toRet = new PermissionVm
            {
                Statuses = BlCode.LoadTable(userId, "PermissionStatus"),
                ActionMode = Enumerations.ActionMode.Add,
                Permission = new Permission { Status = "A", Module = new Module() }
            };

            if (id != null)
            {
                var permission = LoadSingle(userId, Convert.ToInt64(id));
                toRet.Permission = permission;
                //Create roles tree
                var blRole = new BlRole();
                var roles = blRole.LoadForPermission(userId, permission.Module.Id, permission.Code);

                toRet.RolesTree = new JavaScriptSerializer().Serialize((from r in roles
                                                                        select new TreeItemVm
                                                                        {
                                                                            id = r.Id.ToUiString(),
                                                                            parent = r.ParentRole == null ? "#" : r.ParentRole.Id.ToString(),
                                                                            text = r.Code ?? "",
                                                                            icon = "fa fa-users colorMain",
                                                                            state = "{\"checked\": \"" + (r.IsActive ? "true" : "false") + "\"}",
                                                                            li_attr = "{\"class\" : \"form-control-label\"}"
                                                                        }));
                toRet.Signature = BlCommon.GetSignature(toRet.Permission.UserId, toRet.Permission.EntryDate);
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public IEnumerable<Permission> LoadByModule(long userId, string moduleId, bool onlyActive = false)
        {
            var predicate = PredicateBuilder.True<Permission>();
            predicate = predicate.And(e => e.Module.Id == moduleId);
            if (onlyActive)
            { predicate = predicate.And(e => e.Module.Status == "A"); }
            var toRet = _repository.LoadSearch(predicate);
            return toRet;
        }

        public IEnumerable<Permission> LoadByRole(long userId, long roleId)
        {
            var predicate = PredicateBuilder.True<Permission>();
            predicate = predicate.And(p => p.Roles.Contains(BlRole.LoadSingle(userId, roleId, false)));
            var toRet = LoadSearch(userId, predicate);
            return toRet;
        }

        public IEnumerable<Permission> LoadByModuleAndRole(long userId, string moduleId, List<Role> roles, bool onlyActive = false)
        {
            var predicate = PredicateBuilder.True<Permission>();
            predicate = predicate.And(e => e.Module.Id == moduleId);
            if (onlyActive)
            { predicate = predicate.And(e => e.Module.Status == "A"); }
            var permissions = _repository.LoadSearch(predicate).ToList();

            //Create modules and access permission if it does not exist
            if (!permissions.Any())
            {
                var user = BlUser.LoadSingle(userId);
                //Create module if does not exist
                if (BlModule.LoadSingle(userId, moduleId) == null)
                {
                    var newModule = new Module
                    {
                        Id = moduleId,
                        Description = "N/A",
                        Author = user.Id,
                        UserId = user.Id,
                        Status = "A"
                    };
                    var blModule = new BlModule();
                    blModule.Create(userId, newModule);
                }
                //Create access permission
                var newPermission = new Permission
                {
                    Module = BlModule.LoadSingle(userId, moduleId),
                    Code = "Access",
                    Roles = BlRole.LoadSingle(userId, Constants.FullAdminRole).SingleItemAsEnumerable().ToList(),
                    Status = "A"
                };
                Create(userId, newPermission);
            }

            var tmpPermissions = new Permission[permissions.Count];
            permissions.CopyTo(tmpPermissions);

            //Remove permission outside the roles
            foreach (var permission in tmpPermissions)
            {
                var keepFlag = false;
                foreach (var role in roles)
                {
                    if (permission.Roles.Select(r => r.Id).Contains(role.Id))
                    {
                        keepFlag = true;
                        break;
                    }

                }
                if (!keepFlag) permissions.Remove(permission);
            }

            return permissions;
        }

        public Permission LoadByModuleAndCode(long userId, string moduleId, string permissionCode)
        {
            var predicate = PredicateBuilder.True<Permission>();
            predicate = predicate.And(e => e.Module.Id == moduleId);
            predicate = predicate.And(e => e.Code == permissionCode);
            var toRet = _repository.LoadSearch(predicate).SingleOrDefault();
            return toRet;
        }

        public static bool CanAccess(long userId, string moduleId)
        {
            var toRet = CanDo(userId, moduleId, "Access");
            if (!toRet) throw new BusinessException("UnAuthorizedAccess");
            return true;
        }

        public static bool CanDo(long userId, string moduleId, string code)
        {
            //Load user for roles
            var user = BlUser.LoadSingle(userId);
            if (user == null) throw new BusinessException("UserNotFound");

            var roles = BlRole.LoadParents(userId, user.Roles.ToList());

            //Exit if full permission case of roles system administrator
            if (roles.Select(r => r.Id).Contains(Constants.FullAdminRole)) return true;

            //Load permissions for user roles
            var blPermission = new BlPermission();
            var permissions = blPermission.LoadByModuleAndRole(user.Id, moduleId, roles.ToList(), true).ToList();

            //Return permission existance
            return permissions.Where(p => p.Code == code).Any();
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Permission, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Permission>();

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            if (CheckEmpty.String(ref dict, "ModuleId") != "")
                predicate = predicate.And(p => p.Module.Id == dict["ModuleId"].ToString());

            return predicate;
        }

        private void PreSave(long userId, ref Permission toSave, Enumerations.ActionMode action, string roles = "")
        {
            if (action == Enumerations.ActionMode.Add)
            {
                toSave.Module = BlModule.LoadSingle(userId, toSave.Module.Id);
                toSave.EntryDate = BlCommon.GetServerDateTime();
            }
            toSave.UserId = userId;

            if (CheckEmpty.String(roles) != "")
            {
                toSave.Roles.Clear();
                foreach (var roleId in roles.Split(','))
                {
                    toSave.Roles.Add(BlRole.LoadSingle(userId, Convert.ToInt64(roleId)));
                }
            }

        }

        #endregion Private methods
    }
}