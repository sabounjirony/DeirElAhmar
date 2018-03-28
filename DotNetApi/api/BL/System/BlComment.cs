using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
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
    public class BlComment
    {
        #region Members

        private const string Module = "Comment";
        readonly CommentRepository _repository = new CommentRepository();

        #endregion Members

        #region Actions

        public Comment Create(long userId, Comment toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create comment", "CommentCreated", new object[] { toAdd.Text.ManageTextLength(100), toAdd.Reference });
                tran.Complete();
                return toRet;
            }
        }

        public Comment Edit(long userId, Comment toEdit)
        {
            using (var tran = new TransactionScope())
            {
                //Only note owner and full administrator can delete 
                if (userId != toEdit.UserId && !BlUser.LoadSingle(userId).IsFullPermission)
                { throw new BusinessException("NoteCannotBeModifiedUnlessByOwner"); }

                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit comment", "CommentModified", new object[] { toEdit.Text.ManageTextLength(100), toEdit.Reference });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Comment toDelete)
        {
            using (var tran = new TransactionScope())
            {
                //Only note owner and full administrator can delete 
                if (userId != toDelete.UserId && !BlUser.LoadSingle(userId).IsFullPermission)
                { throw new BusinessException("NoteCannotBeDeletedUnlessByOwner"); }

                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete comment", "CommentDleted", new object[] { toDelete.Text.ManageTextLength(100), toDelete.Reference });
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
        public CommentVm Save(long userId, CommentVm toSave)
        {
            var obj = toSave.Comment;
            PreSave(userId, ref obj, toSave.ActionMode);
            toSave.Comment = obj;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Comment = Create(userId, toSave.Comment);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Comment = Edit(userId, toSave.Comment);
                    break;
            }

            return Init(userId, toSave.Comment.Id);
        }

        #endregion Actions

        #region Queries

        public static Comment LoadSingle(long userId, long pin)
        {
            var repository = new CommentRepository();
            var toRet = repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<Comment> LoadSearch(long userId, Expression<Func<Comment, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Comment> LoadPaging(long userId, Expression<Func<Comment, bool>> predicate, int pageSize, int pageNum, out long totCount)
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
                           r.Text,
                           EntryDate = r.EntryDate.ToString(true),
                           User = r.UserId == 0 ? "" : BlUser.LoadSingle(r.UserId).UserName,
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public CommentVm Init(long userId, long? id)
        {
            var toRet = new CommentVm
            {
                ActionMode = Enumerations.ActionMode.Add,
                Comment = new Comment { }
            };

            if (id != null)
            {
                var obj = LoadSingle(userId, Convert.ToInt64(id));
                toRet.Comment = obj;

                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        #endregion Queries

        #region Private methods

        private static Expression<Func<Comment, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Comment>();

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            if (CheckEmpty.String(ref dict, "Reference") != "")
                predicate = predicate.And(p => p.Reference == Convert.ToString(dict["Reference"]));

            if (CheckEmpty.String(ref dict, "Reference") == "" && CheckEmpty.String(ref dict, "Id") == "")
                predicate = predicate.And(p => 1 == 2);

            return predicate;
        }

        private static void PreSave(long userId, ref Comment toSave, Enumerations.ActionMode action)
        {
            if (action == Enumerations.ActionMode.Add)
            {
                toSave.EntryDate = BlCommon.GetServerDateTime();
            }
            else
            {
                var oldComment = LoadSingle(userId, toSave.Id);
                oldComment.Text = toSave.Text;
                toSave = oldComment;
            }
            toSave.UserId = userId;
        }

        #endregion Private methods
    }
}