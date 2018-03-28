using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.Setup;
using BL.System.Logging;
using BL.System.Security;
using DL.Repositories.System;
using DM.Models.System;
using DM.Models.System.Security;
using Tools;
using Tools.Cache;
using Tools.Utilities;
using VM.Ddl;
using VM.Grid;
using VM.System;

namespace BL.System
{
    [BlAspect(AspectPriority = 2)]
    public class BlCode
    {
        #region Members

        private const string Module = "Code";
        readonly CodeRepository _repository = new CodeRepository();

        #endregion Members

        #region Actions

        [BlAspect(Module = Module, Permission = "Create")]
        public Code Create(long userId, Code toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Create code", "CodeCreated", new object[] { toAdd.CodeName, toAdd.TableName });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Edit")]
        public Code Edit(long userId, Code toEdit)
        {
            using (var tran = new TransactionScope())
            {
                Code toRet;
                if (toEdit.OldCodeName != toEdit.CodeName)
                {
                    Delete(userId, toEdit.TableName, toEdit.OldCodeName);
                    toRet = Create(userId, toEdit);
                }
                else
                {
                    toRet = _repository.Edit(toEdit);
                }
                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Edit code", "CodeModified", new object[] { toEdit.CodeName, toEdit.TableName });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, Code toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                //Force cash refresh for all module entries
                CacheHelper.Clear(Module);

                BlLog.Log(userId, Module, "Delete code", "CodeDeleted", new object[] { toDelete.CodeName, toDelete.TableName });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, string tableName, string codeName)
        {
            using (var tran = new TransactionScope())
            {
                var toDelete = LoadSingle(userId, tableName, codeName);
                var toRet = Delete(userId, toDelete);

                tran.Complete();
                return toRet;
            }
        }

        public void Save(long userId, CodeVm toSave)
        {
            if (toSave.ActionMode == Enumerations.ActionMode.Add)
            {
                toSave.Code = Create(userId, toSave.Code);
            }
            else if (toSave.ActionMode == Enumerations.ActionMode.Edit)
            {
                toSave.Code = Edit(userId, toSave.Code);
            }
        }

        #endregion Actions

        #region Queries

        public static Code LoadSingle(long userId, string tableName, string codeName, bool notFromCache = false)
        {
            var toRet = new Code { TableName = tableName, CodeName = codeName };

            if (!CacheHelper.Get(Module + "_" + toRet.TableName +"_"+ toRet.CodeName, out toRet) || notFromCache)
            {
                var codeRepository = new CodeRepository();
                toRet = codeRepository.LoadSingle(tableName, codeName);
                if (toRet != null)
                {
                    if (tableName == "_System" && codeName == "DefaultTimeOut")
                    { CacheHelper.Add(Module + "_" + toRet.TableName + "_" + toRet.CodeName, toRet, Convert.ToInt32(toRet.Value1)); }
                    else
                    { CacheHelper.Add(Module + "_" + toRet.TableName + "_" + toRet.CodeName, toRet, BlCommon.DefaultTimeOut()); }
                }
            }
            return toRet;
        }

        public IEnumerable<Code> LoadSearch(long userId, Expression<Func<Code, bool>> predicate, int count = 0)
        {
            IEnumerable<Code> toRet;
            if (!CacheHelper.Get(Module + "_" + Evaluator.PartialEval(predicate), out toRet))
            {
                toRet = _repository.LoadSearch(predicate, count);
                CacheHelper.Add(Module + "_" + Evaluator.PartialEval(predicate), toRet, BlCommon.DefaultTimeOut());
            }
            return toRet;
        }

        public IEnumerable<Code> LoadPaging(long userId, Expression<Func<Code, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            IEnumerable<Code> toRet;
            if (!CacheHelper.Get(Module + "_" + pageSize + "_" + pageNum + "_" + Evaluator.PartialEval(predicate), out toRet))
            {
                toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
                CacheHelper.Add(Module + "_" + pageSize + "_" + pageNum + "_" + Evaluator.PartialEval(predicate), toRet, BlCommon.DefaultTimeOut());
                CacheHelper.Add(Module + "_totCount_" + Evaluator.PartialEval(predicate), totCount, BlCommon.DefaultTimeOut());
            }
            else
            {
                CacheHelper.Get(Module + "_totCount_" + Evaluator.PartialEval(predicate), out totCount);
            }
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
                           r.TableName,
                           r.CodeName,
                           r.Value1,
                           Value2 = r.Value2 ?? "",
                           Value3 = r.Value3 ?? "",
                           Order = r.DisplayOrder ?? "",
                           Status = r.Status ? "check colorGreen" : "close colorRed",
                           Protected = r.IsProtected ? "lock colorRed" : "unlock-alt colorGreen"
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public static Dictionary<string, string> LoadTable(long userId, string tableName, string toRetColumn = "Value1", string relCode = "")
        {
            var user = BlUser.LoadSingle(userId);
            toRetColumn = (toRetColumn == "Value1" || CheckEmpty.String(toRetColumn) == "") ? user.LanguageId == 1 ? "Value1" : "Value2" : toRetColumn;

            List<Code> results;
            if (!CacheHelper.Get(Module + "_Table_" + tableName + "_" + relCode, out results))
            {
                var codeRepository = new CodeRepository();
                var predicate = PredicateBuilder.True<Code>();
                predicate = predicate.And(p => p.TableName == tableName);
                predicate = predicate.And(p => p.Status);

                if (relCode != "")
                { predicate = predicate.And(p => p.RelCode == relCode); }

                results = codeRepository.LoadSearch(predicate);
                if (results != null)
                {
                    CacheHelper.Add(Module + "_Table_" + tableName + "_" + relCode, results, BlCommon.DefaultTimeOut());
                }
            }

            Dictionary<string, string> toRet = null;
            if (results != null)
                toRet = results.ToDictionary(k => k.CodeName, v => v.GetType().GetProperty(toRetColumn).GetValue(v, null).ToString());

            return toRet;
        }

        public CodeVm Init(long userId, string tableName, string codeName)
        {
            var toRet = new CodeVm
            {
                Tables = LoadTables(userId).ToDictionary(p => p, p => p),
                Statuses = LoadTable(userId, "CodeStatus"),
                Orders = LoadTable(userId, "DisplayOrder"),
                ActionMode = Enumerations.ActionMode.Add,
                Code = new Code { Status = true, DisplayOrder = "A" }
            };

            if (tableName != "null" && codeName != "null")
            {
                var code = LoadSingle(userId, tableName, codeName, true);
                code.OldCodeName = code.CodeName;
                toRet.Code = code;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public IEnumerable<string> LoadTables(long userId)
        {
            IEnumerable<string> toRet;
            if (!CacheHelper.Get(Module + "_TableNames", out toRet))
            {
                var codeRepository = new CodeRepository();
                toRet = codeRepository.LoadTables();
                if (toRet != null)
                {
                    CacheHelper.Add(Module + "_TableNames", toRet, BlCommon.DefaultTimeOut());
                }
            }
            return toRet;
        }

        #region Get LOV

        public static List<DdlVm.DdlOption> GetLov(long userId, string operation, bool required, string relCode = "")
        {
            if (operation.ToUpper() == "CODETABLES")
            { return GetLovTables(userId, required).ToList(); }

            if (operation.ToUpper() == "BRANCHES")
            { return BlBranch.GetLov(userId, required).ToList(); }

            var user = BlUser.LoadSingle(userId);
            var fieldName = user.LanguageId == 1 ? "Value1" : "Value2";

            var blCode = new BlCode();
            var predicate = PredicateBuilder.True<Code>();
            predicate = predicate.And(p => p.TableName == operation);

            if (relCode != "")
            { predicate = predicate.And(p => p.RelCode == relCode); }

            var result = blCode.LoadSearch(userId, predicate).ToList();
            if (!result.Any()) return null;

            var results = (from a in result.Where(m => m.Status)
                           orderby a.DisplayOrder ascending
                           select new DdlVm.DdlOption
                           {
                               value = a.CodeName,
                               label = a.GetType().GetProperty(fieldName).GetValue(a, null).ToString()
                           }).ToList();
            if (!required)
            {
                results.Insert(0, new DdlVm.DdlOption("...", ""));
            }

            return results.ToList();
        }

        public static List<DdlVm.DdlOption> GetLovNumber(long userId, long start, long end, bool required)
        {
            var results = new List<DdlVm.DdlOption>();

            for (var i = start; i <= end; i++)
            {
                results.Insert(0, new DdlVm.DdlOption(i.ToUiString(), i.ToUiString()));
            }

            if (!required)
            {
                results.Insert(0, new DdlVm.DdlOption("...", ""));
            }
            return results;
        }

        public static List<DdlVm.DdlOption> GetLovTables(long userId, bool required)
        {
            var blCode = new BlCode();
            var result = blCode.LoadTables(userId).ToList();
            if (!result.Any()) return null;

            var results = (from a in result
                           select new DdlVm.DdlOption
                           {
                               value = a,
                               label = a
                           }).ToList();
            if (!required)
            {
                results.Insert(0, new DdlVm.DdlOption("...", ""));
            }

            return results.ToList();
        }

        #endregion Get LOV

        public static string GetCodeByLanguage(User user, Code code)
        {
            if (user.LanguageId == 2) //Arabic
            { return CheckEmpty.String(code.Value2) != "" ? code.Value2 : code.Value1; }

            return code.Value1;
        }

        #endregion Queries

        #region QuickSearch

        public static List<DdlVm.DdlOption> LoadQs(long userId, string parameters, string searchTerm, int pageSize, int pageNum, out long count)
        {
            var blObject = new BlCode();

            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var tableName = CheckEmpty.String(ref dict, "tableName");
            var relationKey = CheckEmpty.String(ref dict, "relationKey");

            var predicate = PredicateBuilder.True<Code>();
            predicate = predicate.And(c => c.TableName == tableName);
            predicate = predicate.And(c => c.Status);

            if (CheckEmpty.String(relationKey) != "")
            { predicate = predicate.And(c => c.RelCode == relationKey); }

            if (CheckEmpty.String(searchTerm) != "")
            {
                var tokens = searchTerm.Tokens();
                foreach (var token in tokens)
                {
                    var predicate2 = PredicateBuilder.False<Code>();
                    predicate2 = predicate2.Or(c => c.Value1.Contains(token));
                    predicate2 = predicate2.Or(c => c.Value2.Contains(token));
                    predicate2 = predicate2.Or(c => c.Value3.Contains(token));
                    predicate2 = predicate2.Or(c => c.Value4.Contains(token));
                    predicate2 = predicate2.Or(c => c.Value5.Contains(token));
                    predicate2 = predicate2.Or(c => c.Value6.Contains(token));
                    predicate = predicate.And(predicate2);
                }
            }

            var items = blObject.LoadPaging(userId, predicate, pageSize, (pageNum - 1), out count);

            return items.Select(i => FormatForQs(userId, i)).ToList();
        }

        public static DdlVm.DdlOption LoadQs(long userId, string id, string parameters)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var tableName = CheckEmpty.String(ref dict, "tableName");

            return FormatForQs(userId, LoadSingle(userId, tableName, id));
        }

        private static DdlVm.DdlOption FormatForQs(long userId, Code item)
        {
            var user = BlUser.LoadSingle(userId);

            var toRet = new DdlVm.DdlOption
            {
                value = item.CodeName,
                label = user.LanguageId == 1 ? item.Value1 : item.Value2
            };
            return toRet;
        }

        #endregion QuickSearch

        #region Private methods

        private static Expression<Func<Code, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Code>();

            if (CheckEmpty.String(ref dict, "Table") != "")
                predicate = predicate.And(p => p.TableName == dict["Table"].ToString());

            if (CheckEmpty.String(ref dict, "Code") != "")
                predicate = predicate.And(p => p.CodeName.Contains(dict["Code"].ToString()));

            if (CheckEmpty.String(ref dict, "Protected") != "")
                predicate = predicate.And(p => p.IsProtected == CheckEmpty.Boolean(dict["Protected"]));

            if (CheckEmpty.String(ref dict, "Status") != "")
                predicate = predicate.And(p => p.Status == CheckEmpty.Boolean(dict["Status"]));

            return predicate;
        }

        #endregion
    }
}