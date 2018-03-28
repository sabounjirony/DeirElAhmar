using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Script.Serialization;
using BL.System.Logging;
using BL.System.Security;
using DL.Repositories.System;
using DM.Models.System;
using Tools;
using Tools.Utilities;
using VM.Grid;
using VM.System;

namespace BL.System
{
    [BlAspect(AspectPriority = 2)]
    public class BlDocument
    {
        #region Members

        private const string Module = "Document";
        readonly DocumentRepository _repository = new DocumentRepository();

        #endregion Members

        #region Actions

        public Document Create(long userId, Document toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create document", "DocumentCreated", new object[] { toAdd.Name, toAdd.Reference });
                tran.Complete();
                return toRet;
            }
        }

        public Document Edit(long userId, Document toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit document", "DocumentModified", new object[] { toEdit.Name, toEdit.Reference });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Document toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                //Delete physical file
                var rootDirectory = BlCode.LoadSingle(userId, "_System", "DocumentsRootDirectory").Value1 + "documents\\";
                if (File.Exists(rootDirectory + toDelete.Path)) File.Delete(rootDirectory + toDelete.Path);

                BlLog.Log(userId, Module, "Delete document", "DocumentDeleted", new object[] { toDelete.Name, toDelete.Reference });
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

        [BlAspect(Module = Module, Permission = "Delete")]
        public void Delete(long userId, string reference)
        {
            using (var tran = new TransactionScope())
            {
                var documents = LoadByReference(userId, reference);
                foreach (var document in documents)
                {
                    Delete(userId, document);
                }
                tran.Complete();
            }
        }

        [BlAspect(Module = Module, Permission = "Save")]
        public DocumentVm Save(long userId, DocumentVm toSave)
        {
            var obj = toSave.Document;
            PreSave(userId, ref obj, toSave.ActionMode);
            toSave.Document = obj;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Document = Create(userId, toSave.Document);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Document = Edit(userId, toSave.Document);
                    break;
            }

            return Init(userId, toSave.Document.Id);
        }

        #endregion Actions

        #region Queries

        public static Document LoadSingle(long userId, long pin)
        {
            var repository = new DocumentRepository();
            var toRet = repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<Document> LoadSearch(long userId, Expression<Func<Document, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Document> LoadPaging(long userId, Expression<Func<Document, bool>> predicate, int pageSize, int pageNum, out long totCount)
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
                           Url = "<a class='text-primary' href=" + BlCode.LoadSingle(userId, "_System", "DocumentsRootUrl").Value1 + "documents/" + r.Path + " target='_blank' >" + r.Name + "</a>",
                           Type = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "DocumentType", r.Type)),
                           EntryDate = r.EntryDate.ToString(true),
                           User = r.UserId == 0 ? "" : BlUser.LoadSingle(r.UserId).UserName,
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public DocumentVm Init(long userId, long? id)
        {
            var toRet = new DocumentVm
            {
                ActionMode = Enumerations.ActionMode.Add,
                Document = new Document()
            };

            if (id != null)
            {
                var obj = LoadSingle(userId, Convert.ToInt64(id));
                toRet.Document = obj;

                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public static bool DocumentExist(long userId, string reference, string type)
        {
            var repository = new DocumentRepository();
            var predicate = PredicateBuilder.True<Document>();
            predicate = predicate.And(p => p.Reference == reference);
            predicate = predicate.And(p => p.Type == type);
            var toRet = repository.LoadSearch(predicate).Any();
            return toRet;
        }

        public static string GetDocumentPath(long userId, string reference, string type)
        {
            var repository = new DocumentRepository();
            var predicate = PredicateBuilder.True<Document>();
            predicate = predicate.And(p => p.Reference == reference);
            predicate = predicate.And(p => p.Type == type);
            var toRet = repository.LoadSearch(predicate).First();
            return BlCode.LoadSingle(userId, "_System", "DocumentsRootUrl").Value1 + "documents/" + toRet.Path;
        }

        public IEnumerable<Document> LoadByReference(long userId, string reference)
        {
            var predicate = PredicateBuilder.True<Document>();
            predicate = predicate.And(p => p.Reference == reference);
            var toRet = _repository.LoadSearch(predicate);
            return toRet;
        }


        #endregion Queries

        #region Private methods

        private static Expression<Func<Document, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Document>();

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            if (CheckEmpty.String(ref dict, "Reference") != "")
                predicate = predicate.And(p => p.Reference == Convert.ToString(dict["Reference"]));

            if (CheckEmpty.String(ref dict, "Reference") == "" && CheckEmpty.String(ref dict, "Id") == "")
                predicate = predicate.And(p => 1 == 2);

            return predicate;
        }

        private static void PreSave(long userId, ref Document toSave, Enumerations.ActionMode action)
        {
            if (action == Enumerations.ActionMode.Add)
            {
                //Save physical file under specific location
                var file = toSave.File;
                toSave.Path = SavePhysicalFile(userId, ref file, toSave.Name, toSave.Reference);

                toSave.EntryDate = BlCommon.GetServerDateTime();

            }
            toSave.UserId = userId;
        }

        private static string SavePhysicalFile(long userId, ref HttpPostedFile file, string name, string reference)
        {
            var rootDirectory = BlCode.LoadSingle(userId, "_System", "DocumentsRootDirectory").Value1 + "Documents\\";

            //Create root directory if needed
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);

            //Construct custom directory
            string customDirectory;
            if (reference.IndexOf('_') != -1)
            { customDirectory = reference.Split('_')[0] + "\\" + reference.Replace(reference.Split('_')[0] + "_", "") + "\\"; }
            else
            { customDirectory = reference + "\\"; }

            //Create custom directory if needed
            if (!Directory.Exists(rootDirectory + customDirectory))
            { Directory.CreateDirectory(rootDirectory + customDirectory); }

            var fileName = name + Path.GetExtension(file.FileName);
            var destination = rootDirectory + customDirectory + fileName;

            var i = 1;
            while (File.Exists(destination))
            {
                fileName = name +"_"+ i + Path.GetExtension(file.FileName);
                destination = rootDirectory + customDirectory + fileName;
                i += 1;
            }

            file.SaveAs(destination);

            return customDirectory + fileName;
        }

        #endregion Private methods
    }
}