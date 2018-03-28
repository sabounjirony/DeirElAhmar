using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System.Logging;
using BL.System.MultiLanguage;
using BL.System.Security;
using DL.Repositories.System;
using DM.Models.System;
using Tools;
using Tools.Utilities;
using VM.System;
using VM.Tree;

namespace BL.System
{
    [BlAspect(AspectPriority = 2)]
    public class BlHelp
    {
        #region Members

        private const string Module = "Help";
        readonly HelpRepository _repository = new HelpRepository();

        #endregion Members

        #region Actions

        public Help Create(long userId, Help toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create help", "HelpCreated", new object[] { toAdd.Page, toAdd.Ctrl });
                tran.Complete();
                return toRet;
            }
        }

        public Help Edit(long userId, Help toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit help", "HelpModified", new object[] { toEdit.Page, toEdit.Ctrl });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Help toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete help", "HelpDeleted", new object[] { toDelete.Page, toDelete.Ctrl });
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
        public HelpVm Save(long userId, HelpVm toSave)
        {
            var help = toSave.Help;
            PreSave(userId, ref help, toSave.ActionMode);
            toSave.Help = help;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Help = Create(userId, toSave.Help);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Help = Edit(userId, toSave.Help);
                    break;
            }

            return Init(userId, toSave.Help.Page, toSave.Help.Ctrl);
        }

        #endregion Actions

        #region Queries

        public IEnumerable<Help> LoadAll(long userId)
        {
            var predicate = PredicateBuilder.True<Help>();
            var toRet = LoadSearch(userId, predicate);
            return toRet;
        }

        public static Help LoadSingle(long userId, long? id)
        {
            var helpRepository = new HelpRepository();
            var toRet = helpRepository.LoadSingle(Convert.ToInt64(id));
            return toRet;
        }

        public Help LoadSingle(long userId, string page, string ctrl)
        {
            var userIsEditor = BlPermission.CanDo(userId, Module, "HelpCreator");

            var predicate = PredicateBuilder.True<Help>();
            predicate = predicate.And(p => p.Page == page);
            predicate = predicate.And(p => p.Ctrl == ctrl);
            var toRet = LoadSearch(userId, predicate).SingleOrDefault();

            //If entry does not exist then create it
            if (toRet == null && userIsEditor)
            {
                using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    var toCreate = new Help
                                       {
                                           Page = page,
                                           Ctrl = ctrl,
                                           EntryDate = BlCommon.GetServerDateTime(),
                                           UserId = userId,
                                           Title = "",
                                           Text = "",
                                           DisplayOrder = "ZZ"
                                       };
                    var blHelp = new BlHelp();
                    toRet = blHelp.Create(userId, toCreate);

                    tran.Complete();
                }
            }

            //If entry is empty and user not editor then get parent
            if (toRet.Text == "" && ctrl != "" && !userIsEditor)
            {
                predicate = PredicateBuilder.True<Help>();
                predicate = predicate.And(p => p.Page == page);
                predicate = predicate.And(p => p.Ctrl == "");
                toRet = LoadSearch(userId, predicate).SingleOrDefault();
            }

            return toRet;
        }


        public IEnumerable<Help> LoadSearch(long userId, Expression<Func<Help, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Help> LoadPaging(long userId, Expression<Func<Help, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public HelpVm Init(long userId, string page, string ctrl)
        {
            var parents = LoadAll(userId).Where(h => h.Ctrl == "").ToList();

            foreach (var parent in parents)
            { parent.Title = parent.Title == "" ? Path.GetFileNameWithoutExtension(parent.Page) : parent.Title; }

            var toRet = new HelpVm
            {
                Parents = parents.OrderBy(p => p.Title).ToList(),
                Orders = BlCode.LoadTable(userId, "DisplayOrder"),
                ActionMode = Enumerations.ActionMode.Add,
                Help = new Help { }
            };

            if (page != "")
            {
                var help = LoadSingle(userId, page, ctrl);
                toRet.Help = help;
                toRet.DisplayPage = Path.GetFileNameWithoutExtension(toRet.Help.Page);
                toRet.DisplayControl = toRet.Help.Ctrl == "" ? "N/A" : toRet.Help.Ctrl;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }
            //toRet.Help.User.Roles = null;
            return toRet;
        }

        public HelpVm Init(long userId, long id)
        {
            var parents = LoadAll(userId).Where(h => h.Ctrl == "").ToList();

            foreach (var parent in parents)
            { parent.Title = parent.Title == "" ? Path.GetFileNameWithoutExtension(parent.Page) : parent.Title; }

            var toRet = new HelpVm
            {
                Parents = parents.OrderBy(p => p.Title).ToList(),
                Orders = BlCode.LoadTable(userId, "DisplayOrder"),
                ActionMode = Enumerations.ActionMode.Add,
                Help = new Help { }
            };

            if (id != 0)
            {
                var help = LoadSingle(userId, id);
                toRet.Help = help;
                toRet.DisplayPage = Path.GetFileNameWithoutExtension(toRet.Help.Page);
                toRet.DisplayControl = toRet.Help.Ctrl == "" ? "N/A" : toRet.Help.Ctrl;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }
            //toRet.Help.User.Roles = null;
            return toRet;
        }

        public string LoadAllForTree(long userId)
        {
            var userIsEditor = BlPermission.CanDo(userId, Module, "HelpCreator");
            var predicate = PredicateBuilder.True<Help>();

            //Exclude empty if not editor
            if (!userIsEditor)
            { predicate = predicate.And(e => e.Text != ""); }
            var roleTree = LoadSearch(userId, predicate);

            //Get security permissions
            var allowView = BlPermission.CanDo(userId, Module, "View");
            var allowAdd = BlPermission.CanDo(userId, Module, "Add");
            var allowEdit = BlPermission.CanDo(userId, Module, "Edit");
            var allowDelete = BlPermission.CanDo(userId, Module, "Delete");

            //Create return object
            var toRet = new List<TreeItemVm>();

            //Create root node
            var rootNode = new Help { Id = 0 };

            //Add root node
            toRet.AddRange(ConstructTreeNode(userId, rootNode.SingleItemAsEnumerable(), false, allowAdd, false, false));

            //Add menus           
            toRet.AddRange(ConstructTreeNode(userId, roleTree.ToList(), allowView, allowAdd, allowEdit, allowDelete));

            return new JavaScriptSerializer().Serialize(toRet);
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Help, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Help>();

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            return predicate;
        }

        private void PreSave(long userId, ref Help toSave, Enumerations.ActionMode action)
        {

        }

        private static IEnumerable<TreeItemVm> ConstructTreeNode(long userId, IEnumerable<Help> helps, bool allowView, bool allowAdd, bool allowEdit, bool allowDelete)
        {
            var user = BlUser.LoadSingle(userId);
            var toRet = new List<TreeItemVm>();
            foreach (var help in helps)
            {
                TreeItemVm treeNode;
                var actions = "</span>";
                actions += "<span id='actions_" + help.Id + "' style='display:none'>&nbsp;";
                actions += "<a class='treeAction' onclick='javascript:ViewAction(" + help.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblView", user.LanguageId) + "</span></a>&nbsp;";

                if (allowAdd)
                {
                    actions += "<a class='treeAction' onclick='javascript:AddAction(" + help.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblAdd", user.LanguageId) + "</span></a>&nbsp;";
                }
                if (allowEdit)
                {
                    actions += "<a class='treeAction' onclick='javascript:EditAction(" + help.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblEdit", user.LanguageId) + "</span></a>&nbsp;";
                }
                if (allowDelete)
                {
                    actions += "<a class='treeAction' onclick='javascript:DeleteAction(" + help.Id + ");'><span>" + BlDescription.GetDescription(Enumerations.DescriptionResources.Language, "lblDelete", user.LanguageId) + "</span></a>&nbsp;";
                }
                actions += "<span>";

                if (help.Id == 0)
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
                        id = help.Id.ToUiString(),
                        parent = help.ParentId == null ? "0" : help.ParentId.ToString(),
                        text = "<span onclick='ShowDetails(" + help.Id + ");'>" + (help.Title == "" ? Path.GetFileNameWithoutExtension(help.Page) : help.Title) + actions + "</span>",
                        icon = "fa fa-info fa-lg colorMain",
                        li_attr = "{\"class\" : \"form-control-label\"}"
                    };
                }
                toRet.Add(treeNode);
            }
            return toRet;
        }

        #endregion Private methods
    }
}