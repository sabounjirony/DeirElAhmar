using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System.Logging;
using BL.System.MultiLanguage;
using DL.Repositories.System.Security;
using DM.Models.System.Security;
using Tools;
using Tools.Cache;
using Tools.Utilities;
using VM.System.Security;
using VM.Tree;

namespace BL.System.Security
{
    [BlAspect(AspectPriority = 2)]
    public class BlRole
    {
        #region Members

        private const string Module = "Role";
        readonly RoleRepository _repository = new RoleRepository();

        #endregion Members

        #region Actions

        public Role Create(long userId, Role toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create role", "RoleCreated", new object[] { toAdd.Code });
                tran.Complete();
                return toRet;
            }
        }

        public Role Edit(long userId, Role toEdit, string roleUsers = "", string rolePermissions = "")
        {
            using (var tran = new TransactionScope())
            {
                var oldImage = LoadSingle(userId, toEdit.Id, true);
                oldImage.ParentRole = toEdit.ParentRole;
                oldImage.Code = toEdit.Code;
                var toRet = _repository.Edit(oldImage);

                //Adjust role users
                var arRoleUsers = roleUsers.Split(',');
                var blUser = new BlUser();
                var allUsers = blUser.LoadAll(userId);
                foreach (var user in allUsers)
                {
                    if (user.Roles.Where(r => r.Id == toRet.Id).Any() && !arRoleUsers.Contains(user.Id.ToUiString()))
                    {
                        user.Roles.Remove(user.Roles.Where(ur => ur.Id == toRet.Id).First());
                        blUser.Edit(userId, user);
                    }

                    if (!user.Roles.Where(r => r.Id == toRet.Id).Any() && arRoleUsers.Contains(user.Id.ToUiString()))
                    {
                        user.Roles.Add(LoadSingle(userId, toRet.Id));
                        blUser.Edit(userId, user);
                    }
                }

                //Adjust role permissions
                var arRolePermissions = rolePermissions.Split(',');
                var blPermission = new BlPermission();
                var allPermissions = blPermission.LoadAll(userId);
                foreach (var permission in allPermissions)
                {
                    if (permission.Roles.Where(r => r.Id == toRet.Id).Any() && !arRolePermissions.Contains(permission.Id.ToUiString()))
                    {
                        permission.Roles.Remove(permission.Roles.Where(rp => rp.Id == toRet.Id).First());
                        blPermission.Edit(userId, permission);
                    }

                    if (!permission.Roles.Where(r => r.Id == toRet.Id).Any() && arRolePermissions.Contains(permission.Id.ToUiString()))
                    {
                        permission.Roles.Add(LoadSingle(userId, toRet.Id));
                        blPermission.Edit(userId, permission);
                    }
                }

                //if (toRet.Users == null)
                //{toRet.Users = new List<User>();}
                //toRet.Users = toRet.Users.ToList();

                //foreach (var permissionUser in toRet.Users)
                //{
                //    var user = BlUser.LoadSingle(Convert.ToInt64(permissionUser.Id));
                //    if (!toRet.Users.Where(u => roleUsers.Split(',').Contains(u.Id.ToUiString())).Any())
                //    {
                //        user.Roles.Remove(user.Roles.Where(ur => ur.Id == toRet.Id).First());
                //        blUser.Edit(userId, user);
                //    }
                //}
                //foreach (var roleUserId in arRoleUser)
                //{
                //    var user = BlUser.LoadSingle(Convert.ToInt64(roleUserId));
                //    if (!toRet.Users.Where(u => u.Id == userId).Any())
                //    {
                //        user.Roles.Remove(user.Roles.Where(r => r.Id == toRet.Id).First());
                //        blUser.Edit(userId, user);
                //    }
                //    else
                //    {
                //        user.Roles.Add(LoadSingle(userId, toRet.Id));
                //        blUser.Edit(userId, user);
                //    }
                //}
                //}

                BlLog.Log(userId, Module, "Edit role", "RoleModified", new object[] { toEdit.Code });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Role toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete role", "RoleDeleted", new object[] { toDelete.Code });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, long toDeleteId)
        {
            using (var tran = new TransactionScope())
            {
                //Check if parent
                if (HaveChildren(userId, toDeleteId))
                    throw new BusinessException("CannotDeleteRelatedChildren");

                //Check if related users
                var blUser = new BlUser();
                if (blUser.LoadByRole(userId, toDeleteId).Any())
                    throw new BusinessException("CannotDeleteRelatedUsers");

                var toDelete = LoadSingle(userId, toDeleteId);
                var toRet = Delete(userId, toDelete);

                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Save")]
        public RoleVm Save(long userId, RoleVm toSave)
        {
            var role = toSave.Role;
            PreSave(userId, ref role, toSave.ActionMode);
            toSave.Role = role;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Role = Create(userId, toSave.Role);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Role = Edit(userId, toSave.Role, toSave.RoleUsers, toSave.RolePermissions);
                    break;
            }

            return Init(userId, toSave.Role.Id);
        }

        #endregion Actions

        #region Queries

        public IEnumerable<Role> LoadAll(long userId)
        {
            var predicate = PredicateBuilder.True<Role>();
            //Exclude full admin
            predicate = predicate.And(e => e.Id != 1);

            var toRet = LoadSearch(userId, predicate);
            return toRet;
        }

        public string LoadAllForTree(long userId)
        {
            var predicate = PredicateBuilder.True<Role>();

            //Exclude full admin
            predicate = predicate.And(e => e.Id != 1);

            var roleTree = LoadSearch(userId, predicate);

            //Get security permissions
            var allowView = BlPermission.CanDo(userId, Module, "View");
            var allowAdd = BlPermission.CanDo(userId, Module, "Add");
            var allowEdit = BlPermission.CanDo(userId, Module, "Edit");
            var allowDelete = BlPermission.CanDo(userId, Module, "Delete");

            //Create return object
            var toRet = new List<TreeItemVm>();

            //Create root node
            var rootNode = new Role { Id = 0 };

            //Add root node
            toRet.AddRange(ConstructTreeNode(userId, rootNode.SingleItemAsEnumerable(), false, allowAdd, false, false));

            //Add menus           
            toRet.AddRange(ConstructTreeNode(userId, roleTree.ToList(), allowView, allowAdd, allowEdit, allowDelete));

            return new JavaScriptSerializer().Serialize(toRet);
        }

        public string LoadRoleUsersTree(long userId, long roleId)
        {
            //Get all users
            var blUser = new BlUser();
            var users = blUser.LoadAll(userId);
            users.ToList().ForEach(u => u.IsRestricted = !u.Roles.Select(r => r.Id).Contains(roleId));

            //Create return object
            var toRet = new List<TreeItemVm>();

            //Create root node
            var rootNode = new User { Id = 0, UserName = LoadSingle(userId, roleId).Code };

            //Add root node
            toRet.AddRange(ConstructUserTreeNode(userId, rootNode.SingleItemAsEnumerable()));

            //Add menus           
            toRet.AddRange(ConstructUserTreeNode(userId, users.ToList()));

            return new JavaScriptSerializer().Serialize(toRet);
        }

        public static Role LoadSingle(long userId, long? id, bool notFromCache = false)
        {
            if (id == null) return null;

            Role toRet;
            if (!CacheHelper.Get(Module + "_" + Convert.ToInt64(id).ToUiString(), out toRet) || notFromCache)
            {
                var roleRepository = new RoleRepository();
                toRet = roleRepository.LoadSingle(Convert.ToInt64(id));
                if (toRet != null)
                {
                    CacheHelper.Add(Module + "_" + toRet.Id.ToUiString(), toRet, BlCommon.DefaultTimeOut());
                }
            }
            return toRet;
        }

        public IEnumerable<Role> LoadSearch(long userId, Expression<Func<Role, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Role> LoadPaging(long userId, Expression<Func<Role, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public IEnumerable<Role> LoadForPermission(long userId, string moduleId, string permissionCode, long? roleId = 0)
        {
            //Load all roles
            var toRet = LoadAll(userId).ToList();

            //Load all selected permission roles
            var blPermission = new BlPermission();
            var permission = blPermission.LoadByModuleAndCode(userId, moduleId, permissionCode);

            foreach (var role in permission.Roles)
            {
                if (toRet.Where(r => r.Id == role.Id).Any())
                {
                    toRet.Where(r => r.Id == role.Id).SingleOrDefault().IsActive = true;
                }
            }
            return toRet.ToList();
        }

        public RoleVm Init(long userId, long? id)
        {
            var toRet = new RoleVm
            {
                ParentRoles = LoadAll(userId).ToDictionary(k => k.Id.ToUiString(), v => v.FullParent + (v.FullParent == "" ? "" : "-") + v.Code),
                ActionMode = Enumerations.ActionMode.Add,
                Role = new Role { ParentRole = new Role() }
            };

            if (id != null)
            {
                var role = LoadSingle(userId, Convert.ToInt64(id));
                toRet.Role = role;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
                toRet.RoleUsers = GetRoleUsers(userId, toRet.Role.Id);
                toRet.RolePermissions = GetRolePermissions(userId, toRet.Role.Id);
            }

            return toRet;
        }

        public static IEnumerable<Role> LoadParents(long userId, List<Role> roles)
        {
            var toRet = roles;
            var toAdd = new List<Role>();
            foreach (var role in roles)
            {
                var tmpRole = role;
                while (tmpRole.ParentRole != null)
                {
                    toAdd.Add(tmpRole.ParentRole);
                    tmpRole = tmpRole.ParentRole;
                }
            }
            toRet.AddRange(toAdd);
            return toRet;
        }

        public bool HaveChildren(long userId, long id)
        {
            var predicate = PredicateBuilder.True<Role>();

            if (id == 0)
            { predicate = predicate.And(p => p.ParentRole == null); }
            else
            { predicate = predicate.And(p => p.ParentRole.Id == id); }

            var children = LoadSearch(userId, predicate).ToList();

            return children.Any();
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Role, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Role>();
            predicate = predicate.And(p => p.Id != 1);

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            return predicate;
        }

        private void PreSave(long userId, ref Role toSave, Enumerations.ActionMode action)
        {
            if (toSave.ParentRole != null)
            {
                toSave.ParentRole = LoadSingle(userId, toSave.ParentRole.Id);
            }
        }

        private static IEnumerable<TreeItemVm> ConstructTreeNode(long userId, IEnumerable<Role> roles, bool allowView, bool allowAdd, bool allowEdit, bool allowDelete)
        {
            var user = BlUser.LoadSingle(userId);
            var toRet = new List<TreeItemVm>();
            foreach (var role in roles)
            {
                TreeItemVm treeNode;
                var actions = "</span>";
                actions += "<span id='actions_" + role.Id + "' style='display:none'>&nbsp;";
                if (allowView)
                {
                    actions += "<a class='treeAction' onclick='javascript:ViewAction(" + role.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblView", user.LanguageId) + "</span></a>&nbsp;";
                }
                if (allowAdd)
                {
                    actions += "<a class='treeAction' onclick='javascript:AddAction(" + role.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblAdd", user.LanguageId) + "</span></a>&nbsp;";
                }
                if (allowEdit)
                {
                    actions += "<a class='treeAction' onclick='javascript:EditAction(" + role.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblEdit", user.LanguageId) + "</span></a>&nbsp;";
                }
                if (allowDelete)
                {
                    actions += "<a class='treeAction' onclick='javascript:DeleteAction(" + role.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblDelete", user.LanguageId) + "</span></a>&nbsp;";
                }
                actions += "<span>";

                if (role.Id == 0)
                {
                    treeNode = new TreeItemVm
                    {
                        id = "0",
                        parent = "#",
                        text = "<span onclick='ShowDetails(0);'>Root" + actions + "</span>",
                        icon = "fa fa-briefcase fa-lg colorMain",
                        state = "{\"opened\": \"true\"}",
                        li_attr = "{\"class\" : \"form-control-label\"}"
                    };
                }
                else
                {
                    treeNode = new TreeItemVm
                    {
                        id = role.Id.ToUiString(),
                        parent = role.ParentRole == null ? "0" : role.ParentRole.Id.ToUiString(),
                        text = "<span onclick='ShowDetails(" + role.Id + ");'>" + role.Code + actions + "</span>",
                        icon = "fa fa-users fa-lg colorMain",
                        li_attr = "{\"class\" : \"form-control-label\"}"
                    };
                }
                toRet.Add(treeNode);
            }
            return toRet;
        }

        private static IEnumerable<TreeItemVm> ConstructUserTreeNode(long userId, IEnumerable<User> users)
        {
            var toRet = new List<TreeItemVm>();
            foreach (var user in users)
            {
                TreeItemVm treeNode;
                if (user.Id == 0)
                {
                    treeNode = new TreeItemVm
                    {
                        id = "0",
                        parent = "#",
                        text = user.UserName,
                        icon = "fa fa-users fa-lg colorMain",
                        state = "{\"opened\": \"true\"}",
                        li_attr = "{\"class\" : \"form-control-label\"}"
                    };
                }
                else
                {
                    treeNode = new TreeItemVm
                    {
                        id = user.Id.ToUiString(),
                        parent = "0",
                        text = user.UserName,
                        icon = "fa fa-user fa-lg " + (user.IsRestricted ? "colorRed" : "colorMain"),
                        li_attr = "{\"class\" : \"form-control-label\"}"
                    };
                }
                toRet.Add(treeNode);
            }
            return toRet;
        }

        private string GetRoleUsers(long userId, long roleId)
        {
            //Get all role users and set is active for assigned ones
            var blUser = new BlUser();
            var users = blUser.LoadAll(userId).ToList();

            users.ForEach(u => u.IsRestricted = !u.Roles.Where(r => r.Id == roleId).Any());


            var toRet = new JavaScriptSerializer().Serialize((from u in users
                                                              select new TreeItemVm
                                                              {
                                                                  id = u.Id.ToUiString(),
                                                                  parent = "#",
                                                                  text = u.UserName,
                                                                  icon = "fa fa-user " + (u.IsRestricted ? "colorRed" : "colorGreen"),
                                                                  state = "{\"checked\": \"" + (u.IsRestricted ? "false" : "true") + "\"}",
                                                                  li_attr = "{\"class\" : \"form-control-label\"}"
                                                              }));
            return toRet;
        }

        private string GetRolePermissions(long userId, long roleId)
        {
            var blModule = new BlModule();
            var modules = blModule.LoadAll(userId).ToList();

            var toRetModules = (from m in modules
                                select new TreeItemVm
                                {
                                    id = "M_" + m.Id,
                                    parent = "#",
                                    text = m.Id,
                                    icon = "fa fa-folder colorMain ",
                                    state = "{\"opened\": \"true\"}",
                                    li_attr = "{\"class\" : \"form-control-label\"}",
                                    a_attr = "{\"class\": \"no_checkbox\"}"
                                }).ToList();


            //Get all modules permissions and set is active for assigned ones
            var blPermission = new BlPermission();
            var permissions = blPermission.LoadAll(userId).ToList();

            permissions.ForEach(p => p.IsActive = p.Roles.Where(r => r.Id == roleId).Any());

            //Get is active from parent and set them as disabled
            var parentRolesIds = LoadParents(userId, LoadSingle(userId, roleId).SingleItemAsList()).Where(role => role.Id != roleId).Select(parentRole => parentRole.Id);
            permissions.ForEach(p => p.IsActiveInherited = p.Roles.Where(r => parentRolesIds.Contains(r.Id)).Any());

            var toRetPermissions = (from p in permissions
                                    select new TreeItemVm
                                    {
                                        id = p.Id.ToString(),
                                        parent = "M_" + p.Module.Id,
                                        text = p.Code,
                                        icon = "fa fa-gear " + (p.IsActive ? "colorGreen" : "colorRed"),
                                        state = "{\"selected\": \"" + (p.IsActive || p.IsActiveInherited ? "true" : "false") + "\", \"disabled\": \"" + (p.IsActiveInherited ? "true" : "false") + "\"}",
                                        li_attr = "{\"class\" : \"form-control-label " + (p.IsActiveInherited ? "jstree-disabled" : "") + "\"}"
                                    }).ToList();

            toRetModules.AddRange(toRetPermissions);
            var toRet = new JavaScriptSerializer().Serialize(toRetModules);
            return toRet;
        }

        #endregion Private methods
    }
}