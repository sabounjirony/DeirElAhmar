using System.IO;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using DL.Repositories.System.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using DM.Models.System.MultiLanguage;
using Newtonsoft.Json;
using Tools;
using Tools.Cache;
using Tools.Utilities;
using VM.Grid;
using VM.System.MultiLanguage;
using BL.System.Logging;
using BL.System.Security;
using DM.Models.System.Security;

namespace BL.System.MultiLanguage
{
    [BlAspect(AspectPriority = 2)]
    public class BlDescription
    {
        #region Members

        private const string Module = "Description";
        readonly DescriptionRepository _repository = new DescriptionRepository();

        private string _language;
        private Dictionary<string, DateTime> _resources;

        #endregion

        #region Properties

        public string Language
        {
            get
            {
                _language = BlCode.LoadSingle(Constants.SystemUser, "_System", "DefaultLanguage").Value2;
                return _language;
            }
        }

        public Dictionary<string, DateTime> Resources
        {
            get
            {
                if (_resources == null)
                {
                    _resources = new Dictionary<string, DateTime>();
                    var resourcesRoot = string.Format("{0}\\io\\multilanguage\\{1}\\", Constants.GetWebAppPhysicalPath(),
                                                      Language);
                    foreach (var file in Directory.GetFiles(resourcesRoot, "*.js", SearchOption.AllDirectories).ToList())
                    {
                        _resources.Add(file, File.GetLastWriteTime(file));
                    }
                }
                return _resources;
            }
        }

        #endregion

        #region Actions

        public Description Create(long userId, Description toAdd)
        {
            Description toRet;
            using (var tran = new TransactionScope())
            {
                //Check if previously exists
                var predicate = PredicateBuilder.True<Description>();
                predicate = predicate.And(p => p.LanguageId == toAdd.LanguageId);
                predicate = predicate.And(p => p.Parent == toAdd.Parent);
                predicate = predicate.And(p => p.Code == toAdd.Code);
                var toCheck = LoadSearch(userId, predicate).SingleOrDefault();
                if (toCheck != null) throw new BusinessException("AlreadyExists1", "lblDescription");

                toRet = _repository.Create(toAdd);

                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Create description", "DescriptionCreated", new object[] { toAdd.Parent, toAdd.Code, toAdd.LanguageId });
                tran.Complete();
            }
            return toRet;
        }

        public Description Edit(long userId, Description toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit description", "DescriptionModified", new object[] { toEdit.Parent, toEdit.Code, toEdit.LanguageId });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Description toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete description", "DescriptionDeleted", new object[] { toDelete.Parent, toDelete.Code, toDelete.LanguageId });
                tran.Complete();
                return toRet;
            }
        }

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

        public void Save(long userId, DescriptionVm toSave)
        {
            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Description = Create(userId, toSave.Description);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Description = Edit(userId, toSave.Description);
                    break;
            }
        }

        #endregion Actions

        #region Queries

        public Description LoadSingle(long userId, long pin)
        {
            var toRet = _repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<Description> LoadSearch(long userId, Expression<Func<Description, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Description> LoadPaging(long userId, Expression<Func<Description, bool>> predicate, int pageSize, int pageNum, out long totCount)
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
            var res = (from r in results
                       select new
                       {
                           r.Id,
                           Language = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "Language", r.LanguageId.ToUiString())),
                           Parent = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "DescriptionParent", r.Parent)),
                           r.Code,
                           r.Text
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public DescriptionVm Init(long userId, long? id)
        {
            var toRet = new DescriptionVm
            {
                Languages = BlCode.LoadTable(userId, "Language"),
                DescriptionParents = BlCode.LoadTable(userId, "DescriptionParent"),
                ActionMode = Enumerations.ActionMode.Add,
                Description = new Description { LanguageId = 0, Parent = "" }
            };

            if (id != null)
            {
                var description = LoadSingle(userId, (long)id);
                toRet.Description = description;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public static string GetDescription(ref User user, string code)
        {
            var toRet = string.Empty;

            if (user.LanguageId == 2)
            {
                toRet = GetDescription(Enumerations.DescriptionResources.Language, code, 2, true);
                if (toRet == "")
                    toRet = GetDescription(Enumerations.DescriptionResources.Language, code, 1, true);
            }
            else if (user.LanguageId == 1)
            {
                toRet = GetDescription(Enumerations.DescriptionResources.Language, code, 1, true) ?? code;
            }
            return toRet;
        }

        [BlAspect(Module = Module)]
        public static string GetDescription(Enumerations.DescriptionResources parent, string code, int languageId = 1, bool allowNotFound = false)
        {
            var toRet = CacheHelper.Get(string.Format("{0}_{1}_{2}", Enum.GetName(typeof(Enumerations.Languages), languageId), parent, code));
            if (toRet == null)
            {
                if (allowNotFound) return null;
                throw new BusinessException("DescriptionNotFound", code + "|=|" + Enum.GetName(typeof(Enumerations.Languages), languageId));
            }
            return toRet.ToString();
        }

        public void Initiate(long userId)
        {
            //Make sure multilanguage directory exists
            var rootDirectory = Constants.GetAppRootDirectory(Constants.GetCallingAssemblyLocalPath()) + "io\\multiLanguage\\";
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);

            //Get system languages
            var languages = BlCode.LoadSingle(userId, "_System", "SystemLanguages").Value1.Split(',');

            //Loop over each language and create its resource files
            foreach (var language in languages)
            {
                var files = new Dictionary<string, Dictionary<string, string>>();

                //Get language description
                var languageDesc = BlCode.LoadSingle(userId, "Language", language).Value1;

                //Get and make sure language directory exists
                var languageDirectory = rootDirectory + languageDesc + "\\";
                if (!Directory.Exists(languageDirectory)) Directory.CreateDirectory(languageDirectory);

                //Load language descriptions
                var predicate = PredicateBuilder.True<Description>();
                var localLanguage = language;
                predicate = predicate.And(e => e.LanguageId == Convert.ToInt16(localLanguage));
                var blDescription = new BlDescription();
                var descriptions = blDescription.LoadSearch(userId, predicate).ToList();

                //Clear resource cache
                foreach (var resource in descriptions.Select(d => d.Parent).Distinct())
                { CacheHelper.Clear(string.Format("{0}_{1}", languageDesc, resource)); }

                //Recreate the cache for server side uasge
                foreach (var description in descriptions)
                {
                    if (description.Parent == "") description.Parent = "language";

                    if (!files.ContainsKey(description.Parent))
                    { files.Add(description.Parent, new Dictionary<string, string>()); }

                    files[description.Parent].Add(description.Code, description.Text);
                    CacheHelper.Add(string.Format("{0}_{1}_{2}", languageDesc, description.Parent, description.Code), description.Text);
                }

                //Recreate the json files for client side usage
                foreach (var file in files)
                {
                    var ioFile = languageDirectory + file.Key + ".js";
                    if (!File.Exists(ioFile)) File.Create(ioFile).Close();
                    var json = JsonConvert.SerializeObject(file.Value);
                    File.WriteAllText(ioFile, json);
                }
            }
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Description, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Description>();

            if (CheckEmpty.String(ref dict, "Language") != "")
                predicate = predicate.And(p => p.LanguageId == Convert.ToInt64(dict["Language"]));

            if (CheckEmpty.String(ref dict, "DescriptionParent") != "")
                predicate = predicate.And(p => p.Parent == dict["DescriptionParent"].ToString());

            if (CheckEmpty.String(ref dict, "Code") != "")
                predicate = predicate.And(p => p.Code.Contains(dict["Code"].ToString()));

            if (CheckEmpty.String(ref dict, "Text") != "")
                predicate = predicate.And(p => p.Text.Contains(dict["Text"].ToString()));

            return predicate;
        }

        #endregion
    }
}
