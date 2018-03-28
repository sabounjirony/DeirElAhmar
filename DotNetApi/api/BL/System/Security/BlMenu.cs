using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using BL.Setup;
using BL.System.Logging;
using BL.System.MultiLanguage;
using DL.Repositories.System.Security;
using DM.Models.System.Security;
using Tools;
using Tools.Utilities;
using VM.System.Security;
using Tools.Cache;

namespace BL.System.Security
{
    [BlAspect(AspectPriority = 2)]
    public class BlMenu
    {
        #region Members

        private const string Module = "Menu";
        readonly MenuRepository _repository = new MenuRepository();

        #endregion Members

        #region Actions

        public Menu Create(long userId, Menu toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create menu", "MenuCreated", new object[] { toAdd.DescriptionCode, toAdd.Module.Id });
                tran.Complete();
                return toRet;
            }
        }

        public Menu Edit(long userId, Menu toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit menu", "MenuModified", new object[] { toEdit.DescriptionCode, toEdit.Module.Id });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Menu toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete menu", "MenuDeleted", new object[] { toDelete.DescriptionCode, toDelete.Module.Id });
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
        public MenuVm Save(long userId, MenuVm toSave)
        {
            var menu = toSave.Menu;
            PreSave(userId, ref menu, toSave.ActionMode);
            toSave.Menu = menu;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Menu = Create(userId, toSave.Menu);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Menu = Edit(userId, toSave.Menu);
                    break;
            }

            return Init(userId, toSave.Menu.Id);
        }

        #endregion Actions

        #region Queries

        public string LoadAll(long userId)
        {
            //var toRet = new JavaScriptSerializer().Serialize((from r in roles
            //                                                        select new TreeItemVm
            //                                                        {
            //                                                            id = r.Id.ToUiString(),
            //                                                            parent = r.ParentRoleId?.ToString() ?? "#",
            //                                                            text = r.Code ?? "",
            //                                                            icon = "fa fa-users colorMain",
            //                                                            state = "{\"checked\": \"" + (r.IsActive ? "true" : "false") + "\"}",
            //                                                            li_attr = "{\"class\" : \"form-control-label\"}"
            //                                                        }));
            return null;
        }

        public Menu LoadSingle(long userId, long pin)
        {
            Menu toRet;
            if (!CacheHelper.Get(Module + "_" + userId.ToUiString(), out toRet))
            {
                toRet = _repository.LoadSingle(pin);
                CacheHelper.Add(Module + "_" + toRet.Id.ToUiString(), toRet, BlCommon.DefaultTimeOut());
            }
            return toRet;
        }

        public IEnumerable<Menu> LoadSearch(long userId, Expression<Func<Menu, bool>> predicate, int count = 0)
        {
            IEnumerable<Menu> toRet;
            if (!CacheHelper.Get(Module + "_" + Evaluator.PartialEval(predicate), out toRet))
            {
                toRet = _repository.LoadSearch(predicate, count);
                CacheHelper.Add(Module + "_" + Evaluator.PartialEval(predicate), toRet, BlCommon.DefaultTimeOut());
            }
            return toRet;
        }

        public IEnumerable<Menu> LoadPaging(long userId, Expression<Func<Menu, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public string LoadUserMenu(long userId, bool forEdit)
        {
            var user = BlUser.LoadSingle(userId);

            //Load all active menu items per company
            var predicate = PredicateBuilder.True<Menu>();
            predicate = predicate.And(p => p.Status);

            //Query all entries without a branch or specific for a branch
            var predicate2 = PredicateBuilder.False<Menu>();
            predicate2 = predicate2.Or(p => p.BranchId == user.BranchId);
            predicate2 = predicate2.Or(p => p.BranchId == null);
            predicate = predicate.And(predicate2);

            var lMenu = LoadSearch(userId, predicate).ToList();

            //Vaidate if all modules exist in permissions
            var lDeniedMenu = new List<Menu>();
            foreach (var menuItem in lMenu.ToList())
            {
                if (menuItem.Module != null)
                {
                    if (!BlPermission.CanDo(userId, menuItem.Module.Id, "Access"))
                    {
                        //Build the denied menu list
                        lDeniedMenu.Add(menuItem);
                    }
                }
            }

            //Remove unauthorized and broken menu items
            lMenu = (from m in lMenu
                     where !(from dm in lDeniedMenu select dm.Id).Contains(m.Id)
                     select m).ToList();

            //Iterate all sub menu items and clear broken entries
            foreach (var menuItem in lMenu.ToList())
            {
                //Remove each node that does not have a module nor children
                if (menuItem.Module == null)
                {
                    if (menuItem.Parent != null && menuItem.DescriptionCode != null)
                    {
                        if (!lMenu.Where(m => m.Id == menuItem.Parent.Id && m.Module != null && m.DescriptionCode != null).Any())
                        {
                            //Check if any children have this item as parent
                            if (!lMenu.Where(m => m.Parent != null).Where(m => m.Parent.Id == menuItem.Id && m.Module != null && m.DescriptionCode != null).Any())
                            {
                                lDeniedMenu.Add(menuItem);
                            }
                        }
                    }
                }
            }

            //Remove unauthorized and broken menu entries
            lMenu = (from m in lMenu
                     where !(from dm in lDeniedMenu select dm.Id).Contains(m.Id)
                     select m).ToList();


            //Iterate all top menu items and clear broken entries
            foreach (var menuItem in lMenu.Where(m => m.Parent == null).ToList())
            {
                if (menuItem.Module == null)
                {
                    //Remove each node that does not have a module nor children
                    if (!lMenu.Where(m => m.Parent != null).Where(m => m.Parent.Id == menuItem.Id && m.DescriptionCode != null).Any())
                    {
                        lDeniedMenu.Add(menuItem);
                    }
                }
            }

            //Remove unauthorized and broken menu items
            lMenu = (from m in lMenu
                     where !(from dm in lDeniedMenu select dm.Id).Contains(m.Id)
                     select m).ToList();

            var menu = BuildMenuChildren(user, lMenu, null, forEdit);

            return menu;
        }

        public MenuVm Init(long userId, long? id)
        {
            var toRet = new MenuVm
            {
                Branches = BlBranch.GetLov(userId).ToDictionary(i => i.value, i => i.label),
                Statuses = BlCode.LoadTable(userId, "Status"),
                Menu = new Menu { Status = true },
                ActionMode = Enumerations.ActionMode.Add
            };

            if (id != 0)
            {
                var menu = LoadSingle(userId, Convert.ToInt64(id));
                toRet.Menu = menu;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }


        #endregion Queries

        #region Private methods

        private static void PreSave(long userId, ref Menu toSave, Enumerations.ActionMode action)
        {
            if (action == Enumerations.ActionMode.Add)
            {
                //toSave.Branch = BlBranch.LoadSingle(userId, toSave.Branch.Id);
            }
        }

        private static string BuildMenuChildren(User user, List<Menu> lMenu, long? itemId, bool forEdit)
        {
            var menuChildren = string.Empty;
            IEnumerable<Menu> children;

            if (itemId == null)
            {
                children = lMenu.Where(m => Equals(m.Parent, itemId)).OrderBy(m => m.DisplayOrder).ToList();
            }
            else
            {
                lMenu = lMenu.Where(m => m.Parent != null).ToList();
                children = lMenu.Where(m => m.Parent.Id == itemId).OrderBy(m => m.DisplayOrder).ToList();
            }

            if (children.Any())
            {
                if (itemId != null) menuChildren += "<ul class='dropdown-menu'>";
                foreach (var menuItem in children)
                {
                    if (CheckEmpty.String(menuItem.Details).ToUpper().Contains("SEPARATOR"))
                    {
                        menuChildren += string.Format("<li {0}></li>", menuItem.Details);
                    }
                    else
                    {
                        lMenu = lMenu.Where(m => m.Parent != null).ToList();
                        var subChildren = lMenu.Where(m => m.Parent.Id == menuItem.Id && menuItem.Parent != null).OrderBy(m => m.DisplayOrder).ToList();
                        if (subChildren.Any())
                        { menuChildren += "<li class='dropdown-submenu'>"; }
                        else
                        { menuChildren += "<li>"; }
                        menuChildren += BuildMenuItem(user, menuItem, forEdit);
                        menuChildren += BuildMenuChildren(user, lMenu, menuItem.Id, forEdit);
                        menuChildren += "</li>";
                    }
                }
                if (itemId != null) menuChildren += "</ul>";
            }

            return menuChildren;
        }

        private static string BuildMenuItem(User user, Menu menuItem, bool forEdit)
        {
            if (forEdit)
                return string.Format("<input type='radio' name='chkMenu' id='chkMenu' value='{0}'/>{1}", menuItem.Id, BlDescription.GetDescription(Enumerations.DescriptionResources.Language, menuItem.DescriptionCode, user.LanguageId, true));

            if (menuItem.Module == null && menuItem.Parent == null)
                return string.Format("<a href='#' {1}>{0} <span class='caret'></a>", BlDescription.GetDescription(Enumerations.DescriptionResources.Language, menuItem.DescriptionCode, user.LanguageId, true), menuItem.Details);

            if (menuItem.Module == null)
                return string.Format("<a href='#' {1}>{0} </a>", BlDescription.GetDescription(Enumerations.DescriptionResources.Language, menuItem.DescriptionCode, user.LanguageId, true), menuItem.Details);

            return string.Format("<a href='../{1}' target='content'>{2}</a>", Constants.GetWebAppRootUrl(), CheckEmpty.String(menuItem.Module.Path) + "?ts=" + DateUtilities.GetDateStamp(BlCommon.GetServerDateTime(), true), BlDescription.GetDescription(Enumerations.DescriptionResources.Language, menuItem.DescriptionCode, user.LanguageId, true));
        }

        #endregion Private methods
    }
}