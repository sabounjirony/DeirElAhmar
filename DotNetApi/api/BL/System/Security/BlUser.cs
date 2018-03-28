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
using Tools.Utilities;
using Newtonsoft.Json;
using Tools.Cache;
using VM.Ddl;
using VM.Grid;
using VM.System.Security;
using VM.Tree;

namespace BL.System.Security
{
    [BlAspect(AspectPriority = 2)]
    public class BlUser
    {
        #region Members

        private const string Module = "User";
        readonly UserRepository _repository = new UserRepository();

        #endregion Members

        #region Actions

        [BlAspect(Module = Module, Permission = "Create")]
        public User Create(long userId, User toAdd)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create user", "UserCreated", new object[] { toAdd.UserName });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Edit")]
        public User Edit(long userId, User toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                //Force cash refresh for module entry
                CacheHelper.Clear(Module + "_" + toEdit.Id.ToUiString());

                BlLog.Log(userId, Module, "Edit user", "UserModified", new object[] { toEdit.UserName });
                tran.Complete();
                return toRet;
            }
        }

        [BlAspect(Module = Module, Permission = "Delete")]
        public bool Delete(long userId, User toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete user", "UserDeleted", new object[] { toDelete.UserName });
                tran.Complete();
                return toRet;
            }
        }


        [BlAspect(Module = Module, Permission = "Delete")]
        public void Delete(long userId, long toDeleteId)
        {
            using (var tran = new TransactionScope())
            {
                var toDelete = LoadSingle(toDeleteId);

                Delete(userId, toDelete);
                tran.Complete();
            }
        }

        [BlAspect(Module = Module, Permission = "Save")]
        public UserVm Save(long userId, UserVm toSave)
        {
            var user = toSave.User;
            PreSave(userId, ref user, toSave.ActionMode, toSave.UserRoles);
            toSave.User = user;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.User = Create(userId, toSave.User);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.User = Edit(userId, toSave.User);
                    break;
            }

            return Init(userId, toSave.User.Id);
        }

        public UserVm ProfileSave(long userId, UserVm toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var user = LoadSingle(toEdit.User.Id, true);
                if (toEdit.ChangePassword)
                {
                    if (!Cryptography.ComputeToHash(toEdit.OldPassword).SequenceEqual(user.Password))
                        throw new BusinessException("WrongOldPassword");
                    if (!Cryptography.ComputeToHash(toEdit.NewPassword).SequenceEqual(Cryptography.ComputeToHash(toEdit.ConfirmPassword)))
                        throw new BusinessException("NewPasswordDoesNotmatchConfirmation");
                    user.LastPasswordUpdate = BlCommon.GetServerDateTime();
                    if (user.PasswordHistory != "")
                    {
                        var oldPasswordList = CheckEmpty.String(user.PasswordHistory).Split(',');
                        foreach (var pass in oldPasswordList)
                        {
                            if (Cryptography.ComputeToHash(toEdit.NewPassword).SequenceEqual(Convert.FromBase64String(pass)))
                            { throw new BusinessException("NewPasswordFrequentlyUsed"); }
                        }
                    }
                    user.PasswordHistory += user.PasswordHistory == "" ? "" : ",";
                    user.PasswordHistory += Convert.ToBase64String(user.Password);
                    var historyLength = BlCode.LoadSingle(userId, "_System", "PasswordHistroyLength").Value1;
                    if (user.PasswordHistory.Split(',').Length > Convert.ToInt32(historyLength))
                    {
                        var newPasswordHistroy = user.PasswordHistory.Split(',').ToList();
                        newPasswordHistroy.RemoveAt(0);
                        user.PasswordHistory = string.Join(",", newPasswordHistroy);
                    }
                    user.Password = Cryptography.ComputeToHash(toEdit.NewPassword);
                    toEdit.ChangePassword = false;
                }
                user.UserName = toEdit.User.UserName;
                user.LanguageId = Convert.ToInt16(toEdit.User.LanguageId);
                user.PageSize = Convert.ToInt16(toEdit.User.PageSize);

                Edit(userId, user);

                //Force cash refresh for module entry
                CacheHelper.Clear(Module + "_" + user.Id.ToUiString());

                BlLog.Log(userId, Module, "Profile change", "UserProfileModified", new object[] { user.UserName });
                tran.Complete();
                return toEdit;
            }
        }

        public static bool LogOut(long userId)
        {
            var user = LoadSingle(userId, true);
            //Add extra log out code
            BlLog.Log(userId, Module, "LogOut", "UserSuccessfulLogout", new object[] { user.UserName });
            return true;
        }

        public LoginVm Authenticate(LoginVm model)
        {
            var hashedPass = Cryptography.ComputeToHash(model.Password);
            if (hashedPass == null) throw new BusinessException("InvalidLogin");

            var userRepository = new UserRepository();
            var predicate = PredicateBuilder.True<User>();
            predicate = predicate.And(u => u.UserName == model.Username);
            var users = userRepository.LoadSearch(predicate);

            //Check if any user have same password
            IStructuralEquatable eqa1 = hashedPass;
            var user = users.FirstOrDefault(u => eqa1.Equals(u.Password, StructuralComparisons.StructuralEqualityComparer));

            if (user == null) throw new BusinessException("InvalidLogin");

            //Check if user is active
            if (user.IsBlocked) throw new BusinessException("UserInactive");

            UpdateTicketValidity(user.Id);

            model.SecurityToken = Cryptography.Encrypt(JsonConvert.SerializeObject(user.Id), true);
            model.Password = null;
            model.Language = BlCode.LoadSingle(user.Id, "Language", user.LanguageId.ToUiString()).Value1;
            model.BranchId = user.BranchId;
            model.BranchName = BlBranch.GetBranchName(user.Id);
            model.CrossBranches = BlPermission.CanDo(user.Id, "BRANCH", "CrossBranches");
            model.PageSize = user.PageSize;
            BlLog.Log(user.Id, Module, "LogIn", "UserSuccessfulLogin", new object[] { user.UserName });
            return model;
        }

        public long? Authorize(string token, string requestUri = "")
        {
            try
            {
                var userId = Cryptography.Decrypt(token, true);

                if (Convert.ToInt64(userId) == 0)
                { throw new BusinessException("InvalidTicket", "lblUser"); }

                //Validate ticket user
                var user = LoadSingle(Convert.ToInt64(userId));

                if (user == null)
                { throw new BusinessException("InvalidTicket", "lblUser"); }

                //Validate ticket user status
                if (user.IsBlocked)
                { throw new BusinessException("UserInactive"); }

                //Validate ticket datestamp
                DateTime dateStamp;
                if (!CacheHelper.Get(Module + "_DateStamp_" + userId, out dateStamp))
                {
                    //BlLogError.LogError("BlUser.Authorize", "Datestamp empty");
                    throw new BusinessException("InvalidTicket", "lblDate");
                }
                if (dateStamp > BlCommon.GetServerDateTime())
                {
                    //BlLogError.LogError("BlUser.Authorize", "datestamp less than current date");
                    throw new BusinessException("InvalidTicket", "lblDate");
                }

                //Validate ticket validtill
                DateTime validTill;
                if (!CacheHelper.Get(Module + "_ValidTill_" + userId, out validTill))
                {
                    //BlLogError.LogError("BlUser.Authorize", "Valid till is empty");
                    throw new BusinessException("InvalidTicket", "lblDate");
                }
                if (validTill < BlCommon.GetServerDateTime())
                {
                    //BlLogError.LogError("BlUser.Authorize", "Valid till less than current date");
                    throw new BusinessException("InvalidTicket", "lblDate");
                }

                UpdateTicketValidity(user.Id);
                return user.Id;
            }
            catch (Exception)
            {
                if (requestUri.ToUpper().Contains("LOGOUT"))
                    return 1;
            }
            return null;
        }

        public void ResetPassword(long userId, long toResetUserId)
        {
            using (var tran = new TransactionScope())
            {
                var defaultPassword = BlCode.LoadSingle(userId, "_System", "DefaultPassword").Value1;
                var toResetUser = LoadSingle(toResetUserId);
                toResetUser.Password = Cryptography.ComputeToHash(defaultPassword);
                toResetUser.MustChangePassword = true;

                BlLog.Log(userId, Module, "Password reset", "UserPasswordReset", new object[] { toResetUser.UserName });
                tran.Complete();
            }
        }

        #endregion Actions

        #region Queries

        public IEnumerable<User> LoadAll(long userId)
        {
            var predicate = PredicateBuilder.True<User>();
            predicate = predicate.And(e => e.Id != 1);

            var toRet = _repository.LoadSearch(predicate);
            return toRet.ToList();
        }

        [BlAspect(Module = Module)]
        public static User LoadSingle(long userId, bool notFromCache = false)
        {
            if (userId == 0) return null;

            User toRet;
            if (!CacheHelper.Get(Module + "_" + userId.ToUiString(), out toRet) || notFromCache)
            {
                var predicate = PredicateBuilder.True<User>();
                predicate = predicate.And(e => e.Id == userId);
                var userRepository = new UserRepository();
                toRet = userRepository.LoadSearch(predicate).First();

                if (toRet != null)
                {
                    if (toRet.Roles.Where(r => r.Id == Constants.FullAdminRole).Any())
                        toRet.IsFullPermission = true;
                    CacheHelper.Add(Module + "_" + toRet.Id.ToUiString(), toRet, BlCommon.DefaultTimeOut());
                }
                else
                {
                    //Force cash refresh for module entry
                    CacheHelper.Clear(Module + "_" + userId.ToUiString());
                }
            }

            return toRet;
        }

        public static User LoadByPin(long userId, long pin)
        {
            var blUser = new BlUser();
            var predicate = PredicateBuilder.False<User>();
            predicate = predicate.Or(m => m.Entity.Pin == pin);
            var toRet = blUser.LoadSearch(userId, predicate).FirstOrDefault();
            return toRet;
        }

        public IEnumerable<User> LoadMany(long userId, string ids)
        {
            var listId = ids.Split(',');
            return listId.Select(id => LoadSingle(Convert.ToInt64(id))).ToList();
        }

        public IEnumerable<User> LoadSearch(long userId, Expression<Func<User, bool>> predicate, int count = 0)
        {
            IEnumerable<User> toRet;
            if (!CacheHelper.Get(Module + "_" + Evaluator.PartialEval(predicate), out toRet))
            {
                toRet = _repository.LoadSearch(predicate, count);
                CacheHelper.Add(Module + "_" + Evaluator.PartialEval(predicate), toRet, BlCommon.DefaultTimeOut());
            }
            return toRet;
        }

        public IEnumerable<User> LoadPaging(long userId, Expression<Func<User, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public GridResults LoadPaging(long userId, string search, int pageIndex, out long totalRecords, string sortColumnName = "", string sortOrderBy = "")
        {
            //Get current user
            var user = LoadSingle(userId);

            //Query paged data
            var results = LoadPaging(userId, CreateFilter(search), user.PageSize, pageIndex - 1, out totalRecords);

            //Convert results into display model
            var res = (from r in results
                       select new
                       {
                           r.Id,
                           r.UserName,
                           Name = BlEntity.FormatFullName(r.Entity),
                           Language = BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "Language", r.LanguageId.ToUiString())),
                           r.PageSize,
                           roles = string.Join(",", r.Roles.Select(ro => ro.Code)),
                           Branch = BlBranch.GetBranchName (user.Id, r.BranchId),
                           IsBlocked = r.IsBlocked ? "close colorRed" : "check colorGreen"
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public UserVm Init(long userId, long? id)
        {
            var callingUser = LoadSingle(userId);

            var toRet = new UserVm
            {
                Branches = BlBranch.GetLov(userId, true).ToDictionary(i => i.value, i => i.label),
                Languages = BlCode.LoadTable(userId, "Language", callingUser.LanguageId == (int)Enumerations.Languages.English ? "Value1" : "Value2"),
                PageSizes = BlCode.LoadTable(userId, "PageSizes", callingUser.LanguageId == (int)Enumerations.Languages.English ? "Value1" : "Value2"),
                ActionMode = Enumerations.ActionMode.Add,
                User = new User { LanguageId = 1, PageSize = 10 }
            };

            if (id != null)
            {
                var user = LoadSingle((long)id);
                user.Pin = user.Entity == null ? user.Pin : user.Entity.Pin;
                user.Entity = null;
                //user.Branch = null;
                toRet.User = user;
                toRet.Signature = BlCommon.GetSignature(toRet.User.EnteringUserId, toRet.User.EntryDate);
                //Get all roles and check user assigned ones
                var blRole = new BlRole();
                var roles = blRole.LoadAll(userId).ToList();
                if (user.Roles != null)
                {
                    foreach (var role in user.Roles)
                    {
                        if (roles.Where(r => r.Id == role.Id).Any())
                        {
                            roles.Where(r => r.Id == role.Id).SingleOrDefault().IsActive = true;
                        }
                    }
                }
                toRet.UserRoles = new JavaScriptSerializer().Serialize((from r in roles
                                                                        select new TreeItemVm
                                                                        {
                                                                            id = r.Id.ToUiString(),
                                                                            parent = r.ParentRole == null ? "#" : r.ParentRole.Id.ToUiString(),
                                                                            text = r.Code ?? "",
                                                                            icon = "fa fa-users colorMain",
                                                                            state = "{\"checked\": \"" + (r.IsActive ? "true" : "false") + "\"}",
                                                                            li_attr = "{\"class\" : \"form-control-label\"}"
                                                                        }));
                //user.Roles = null;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
            }

            return toRet;
        }

        public UserVm ProfileInit(long userId)
        {
            var user = LoadSingle(userId, true);
            var toRet = new UserVm
            {
                User = user,
                Languages = BlCode.LoadTable(userId, "Language", user.LanguageId == (int)Enumerations.Languages.English ? "Value1" : "Value2"),
                PageSizes = BlCode.LoadTable(userId, "PageSizes", user.LanguageId == (int)Enumerations.Languages.English ? "Value1" : "Value2"),
                Signature = BlCommon.GetSignature(user.EnteringUserId, user.EntryDate),
                ActionMode = Enumerations.ActionMode.Edit
            };
            return toRet;
        }

        public static bool IsFullAdmin(long userId)
        {
            var user = LoadSingle(userId);
            return user.Roles.Select(r => r.Id).Contains(Constants.FullAdminRole);
        }

        public IEnumerable<User> LoadByRole(long userId, long roleId, bool onlyActive = false)
        {
            var predicate = PredicateBuilder.True<User>();
            predicate = predicate.And(u => u.Roles.Contains(BlRole.LoadSingle(userId, roleId, false)));
            if (onlyActive)
            { predicate = predicate.And(u => u.IsBlocked == false); }

            var toRet = LoadSearch(userId, predicate);
            return toRet;
        }

        #endregion Queries

        #region QuickSearch

        public static List<DdlVm.DdlOption> LoadQs(long userId, string parameters, string searchTerm, int pageSize, int pageNum, out long count)
        {
            var user = LoadSingle(userId);
            var blObject = new BlUser();

            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var isActive = CheckEmpty.Boolean(ref dict, "isActive");
            var branchId = CheckEmpty.Numeric(ref dict, "branchId");

            var predicate = PredicateBuilder.True<User>();

            if (isActive)
            { predicate = predicate.And(c => c.Entity.Status == "A"); }

            if (branchId != 0)
            { predicate = predicate.And(c => c.BranchId == branchId); }

            if (CheckEmpty.String(searchTerm) != "")
            {
                var tokens = searchTerm.Tokens();
                foreach (var token in tokens)
                {
                    var predicate2 = PredicateBuilder.False<User>();
                    predicate2 = predicate2.Or(m => m.Entity.FirstName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.FatherName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.FamilyName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFirstName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFatherName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFamilyName.Contains(token));
                    predicate2 = predicate2.Or(m => m.UserName.Contains(token));
                    predicate = predicate.And(predicate2);
                }
            }

            var items = blObject.LoadPaging(userId, predicate, pageSize, (pageNum - 1), out count);

            return items.Select(i => FormatForQs(user.LanguageId, i)).ToList();
        }

        public static DdlVm.DdlOption LoadQs(long userId, long id)
        {
            var user = LoadSingle(userId);
            return FormatForQs(user.LanguageId, LoadByPin(userId, id));
        }

        private static DdlVm.DdlOption FormatForQs(int languageId, User item)
        {
            var toRet = new DdlVm.DdlOption
            {
                value = item.Id.ToUiString(),
                label = item.UserName
            };
            return toRet;
        }

        #endregion QuickSearch

        #region Private methods

        private static Expression<Func<User, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<User>();
            predicate = predicate.And(p => p.Id != 1);

            if (CheckEmpty.String(ref dict, "UserName") != "")
                predicate = predicate.And(p => p.UserName.Contains(dict["UserName"].ToString()));

            if (CheckEmpty.String(ref dict, "Status") != "")
                predicate = predicate.And(p => p.IsBlocked == (dict["Status"].ToString() == "S"));

            if (CheckEmpty.String(ref dict, "BranchId") != "")
                predicate = predicate.And(p => p.BranchId == Convert.ToInt64(dict["BranchId"]));

            return predicate;
        }

        private void PreSave(long userId, ref User toSave, Enumerations.ActionMode action, string userRoles = "")
        {
            if (action == Enumerations.ActionMode.Add)
            {
                var defaultPassword = BlCode.LoadSingle(userId, "_System", "DefaultPassword").Value1;
                toSave.Password = Cryptography.ComputeToHash(defaultPassword);
                toSave.MustChangePassword = true;
                toSave.EntryDate = BlCommon.GetServerDateTime();
                toSave.LastPasswordUpdate = toSave.EntryDate;
                //toSave.Branch = BlBranch.LoadSingle(userId, toSave.Branch.Id);
            }
            else if (action == Enumerations.ActionMode.Edit)
            {

            }
            toSave.EnteringUserId = userId;
            toSave.Entity = BlEntity.LoadSingle(userId, toSave.Pin);
            if (CheckEmpty.String(userRoles) != "")
            {
                if (toSave.Roles == null) toSave.Roles = new List<Role>();

                toSave.Roles.Clear();
                foreach (var roleId in userRoles.Split(','))
                {
                    toSave.Roles.Add(BlRole.LoadSingle(userId, Convert.ToInt64(roleId)));
                }
            }
        }

        private static void UpdateTicketValidity(long userId)
        {
            var defaultTimeOut = Convert.ToInt32(BlCode.LoadSingle(userId, "_System", "DefaultTimeOut").Value1);

            //BlLogError.LogError("BlUser.UpdateTicketValidity", "Key: " + Module + "_DateStamp_" + userId.ToUiString() +", Value: " + BlCommon.GetServerDateTime());
            //BlLogError.LogError("BlUser.UpdateTicketValidity", "Key: " + Module + "_ValidTill_" + userId.ToUiString() + ", Value: " + BlCommon.GetServerDateTime().AddMinutes(defaultTimeOut));

            //Increment ticket cached lifetime
            CacheHelper.Add(Module + "_DateStamp_" + userId.ToUiString(), BlCommon.GetServerDateTime(), defaultTimeOut);
            CacheHelper.Add(Module + "_ValidTill_" + userId.ToUiString(), BlCommon.GetServerDateTime().AddMinutes(defaultTimeOut), defaultTimeOut);
        }

        #endregion Private methods
    }
}