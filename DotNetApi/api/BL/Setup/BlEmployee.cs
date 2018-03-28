using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web.Script.Serialization;
using BL.System;
using BL.System.Logging;
using BL.System.Security;
using DL.Repositories.Setup;
using DM.Models.Setup;
using Tools;
using Tools.Utilities;
using VM.Ddl;
using VM.Grid;
using VM.Setup;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlEmployee
    {
        #region Members

        private const string Module = "Employee";
        readonly EmployeeRepository _repository = new EmployeeRepository();

        #endregion Members

        #region Actions

        public Employee Create(long userId, Employee toAdd)
        {
            using (var tran = new TransactionScope())
            {
                toAdd.UserId = userId;
                toAdd.EntryDate = BlCommon.GetServerDateTime();

                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create employee", "EmployeeCreated", new object[] { toAdd.Entity.Pin, BlEntity.FormatFullName(toAdd.Entity) });
                tran.Complete();
                return toRet;
            }
        }

        public Employee Edit(long userId, Employee toEdit)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit employee", "EmployeeModified", new object[] { toEdit.Entity.Pin, BlEntity.FormatFullName(toEdit.Entity) });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Employee toDelete)
        {
            using (var tran = new TransactionScope())
            {
                //Check if employee has related user account
                if (BlUser.LoadByPin(userId, toDelete.Entity.Pin) != null)
                    throw new BusinessException("CannotDeleteRelatedUser");

                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete employee", "EmployeeDeleted", new object[] { toDelete.Entity.Pin, BlEntity.FormatFullName(toDelete.Entity) });
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
        public EmployeeVm Save(long userId, EmployeeVm toSave)
        {
            var obj = toSave.Employee;
            PreSave(userId, ref obj, toSave.ActionMode);
            toSave.Employee = obj;

            switch (toSave.ActionMode)
            {
                case Enumerations.ActionMode.Add:
                    toSave.Employee = Create(userId, toSave.Employee);
                    break;
                case Enumerations.ActionMode.Edit:
                    toSave.Employee = Edit(userId, toSave.Employee);
                    break;
            }

            return Init(userId, toSave.Employee.Id);
        }

        public bool AddLocation(long userId, long employeeId, long locationId)
        {
            var user = BlUser.LoadSingle(userId);

            using (var tran = new TransactionScope())
            {
                var employee = LoadSingle(userId, employeeId);

                //Adjust the employee
                Edit(userId, employee);

                tran.Complete();
                return true;
            }
        }

        public bool DeleteLocation(long userId, long employeeId, long locationId)
        {
            var user = BlUser.LoadSingle(userId);

            using (var tran = new TransactionScope())
            {
                var employee = LoadSingle(userId, employeeId);

                //Check if employee location had some stock in it
                //var blStock = new BlStock();
                //var employeeLocationStock = blStock.LoadByLocationEmployee(userId, locationId, employee.Id, new[] { "I" });
                //if (employeeLocationStock.Any())
                //{ throw new BusinessException("CannotDeleteRelatedStock"); }

                //Adjust the employee
                Edit(userId, employee);

                tran.Complete();
                return true;
            }
        }

        #endregion Actions

        #region Queries

        public static Employee LoadSingle(long userId, long id)
        {
            var repository = new EmployeeRepository();
            var toRet = repository.LoadSingle(id);
            return toRet;
        }

        public static Employee LoadByPin(long userId, long pin)
        {
            var repository = new EmployeeRepository();

            var predicate = PredicateBuilder.True<Employee>();
            predicate = predicate.And(p => p.Entity.Pin == pin);

            var toRet = repository.LoadSearch(predicate).FirstOrDefault();
            return toRet;
        }

        public IEnumerable<Employee> LoadSearch(long userId, Expression<Func<Employee, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Employee> LoadPaging(long userId, Expression<Func<Employee, bool>> predicate, int pageSize, int pageNum, out long totCount)
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
                           Gender = r.Entity.Gender == null ? "" : BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "Gender", r.Entity.Gender)),
                           Name = BlEntity.FormatFullName(r.Entity),
                           Number = r.Entity.IdType == "M" ? r.Entity.IdNum : "",
                           r.Entity.FullEnLongName,
                           Level = r.Level == null ? "" : BlCode.GetCodeByLanguage(user, BlCode.LoadSingle(userId, "EmployeeLevel", r.Level)),
                           Status = r.Status == "A" ? "check colorGreen" : "close colorRed"
                       }).ToList();


            //Convert display model into json data
            return GridVm.FormatResult(res, user.PageSize, pageIndex, totalRecords);
        }

        public EmployeeVm Init(long userId, long? id)
        {
            var user = BlUser.LoadSingle(userId);

            var toRet = new EmployeeVm
            {
                Branches = BlBranch.GetLov(userId, true).ToDictionary(i => i.value, i => i.label),
                Titles = BlCode.LoadTable(userId, "Title"),
                Genders = BlCode.LoadTable(userId, "Gender"),
                IdTypes = BlCode.LoadTable(userId, "IdType"),
                Maritals = BlCode.LoadTable(userId, "Marital"),
                Statuses = BlCode.LoadTable(userId, "Status"),
                Levels = BlCode.LoadTable(userId, "EmployeeLevel"),
                ActionMode = Enumerations.ActionMode.Add,
                Employee = new Employee { Status = "A", Entity = new Entity { BranchId = user.BranchId, Nationality = 422, Status = "A" }, Level = "0" }
            };

            if (id != null)
            {
                var obj = LoadSingle(userId, Convert.ToInt64(id));
                toRet.Employee = obj;
                toRet.ActionMode = Enumerations.ActionMode.Edit;
                toRet.Signature = BlCommon.GetSignature(toRet.Employee.UserId, toRet.Employee.EntryDate);
            }

            return toRet;
        }

        #endregion Queries

        #region QuickSearch

        public static DdlVm.DdlOption LoadQs(long userId, long id, string parameters)
        {
            var user = BlUser.LoadSingle(userId);
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var returnPin = CheckEmpty.Boolean(ref dict, "returnPin");
            var item = returnPin ? LoadByPin(userId, id) : LoadSingle(userId, id);
            return FormatForQs(user.LanguageId, item, returnPin);
        }

        public static List<DdlVm.DdlOption> LoadQs(long userId, string parameters, string searchTerm, int pageSize, int pageNum, out long count)
        {
            var user = BlUser.LoadSingle(userId);
            var blObject = new BlEmployee();

            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
            var isActive = CheckEmpty.Boolean(ref dict, "isActive");
            var gender = CheckEmpty.String(ref dict, "Gender");
            var returnPin = CheckEmpty.Boolean(ref dict, "returnPin");
            var locationId = CheckEmpty.Numeric(ref dict, "locationId");

            var predicate = PredicateBuilder.True<Employee>();

            predicate = predicate.And(p => p.Id != 1); //Exclude employee presidential palace

            if (isActive)
            { predicate = predicate.And(c => c.Status == "A"); }

            if (gender != "")
            { predicate = predicate.And(c => gender.Split(',').Contains(c.Entity.Gender)); }

            if (CheckEmpty.String(searchTerm) != "")
            {
                var tokens = searchTerm.Tokens();
                foreach (var token in tokens)
                {
                    var predicate2 = PredicateBuilder.False<Employee>();
                    predicate2 = predicate2.Or(m => m.Entity.FirstName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.FatherName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.FamilyName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFirstName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFatherName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFamilyName.Contains(token));
                    predicate = predicate.And(predicate2);
                }
            }

            var items = blObject.LoadPaging(userId, predicate, pageSize, (pageNum - 1), out count);

            return items.Select(i => FormatForQs(user.LanguageId, i, returnPin)).ToList();
        }

        private static DdlVm.DdlOption FormatForQs(int languageId, Employee item, bool returnPin = false)
        {
            var toRet = new DdlVm.DdlOption
            {
                value = returnPin ? item.Entity.Pin.ToUiString() : item.Id.ToUiString(),
                label = BlEntity.FormatFullName(item.Entity)
            };
            return toRet;
        }


        //public static DdlVm.DdlOption LoadEmployeeLocationQs(long userId, string id, string parameters)
        //{
        //    var user = BlUser.LoadSingle(userId);
        //    var serializer = new JavaScriptSerializer();
        //    var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
        //    var returnPin = CheckEmpty.Boolean(ref dict, "returnPin");
        //    var item = returnPin ? LoadByPin(userId, id) : LoadSingle(userId, id);
        //    var employee = LoadSingle(userId, Convert.ToInt64(id.Split('-')[0]));
        //    var location = BlEmployee.LoadSingle(userId, Convert.ToInt64(id.Split('-')[1]));
        //    return FormatEmployeeLocationForQs(user.LanguageId, item, returnPin);
        //}

        //public static List<DdlVm.DdlOption> LoadEmployeeLocationQs(long userId, string parameters, string searchTerm, int pageSize, int pageNum, out long count)
        //{
        //    var user = BlUser.LoadSingle(userId);
        //    var blObject = new BlEmployee();

        //    var serializer = new JavaScriptSerializer();
        //    var dict = serializer.Deserialize<Dictionary<string, object>>(parameters);
        //    var isActive = CheckEmpty.Boolean(ref dict, "isActive");
        //    var gender = CheckEmpty.String(ref dict, "Gender");
        //    var returnPin = CheckEmpty.Boolean(ref dict, "returnPin");

        //    var predicate = PredicateBuilder.True<Employee>();

        //    if (isActive)
        //    { predicate = predicate.And(c => c.Status == "A"); }

        //    if (gender != "")
        //    { predicate = predicate.And(c => gender.Split(',').Contains(c.Entity.Gender)); }

        //    if (CheckEmpty.String(searchTerm) != "")
        //    {
        //        var predicate2 = PredicateBuilder.False<Employee>();
        //        predicate2 = predicate2.Or(m => m.Entity.FirstName.Contains(searchTerm));
        //        predicate2 = predicate2.Or(m => m.Entity.FatherName.Contains(searchTerm));
        //        predicate2 = predicate2.Or(m => m.Entity.FamilyName.Contains(searchTerm));
        //        predicate2 = predicate2.Or(m => m.Entity.ArFirstName.Contains(searchTerm));
        //        predicate2 = predicate2.Or(m => m.Entity.ArFatherName.Contains(searchTerm));
        //        predicate2 = predicate2.Or(m => m.Entity.ArFamilyName.Contains(searchTerm));
        //        predicate = predicate.And(predicate2);
        //    }

        //    var items = blObject.LoadPaging(userId, predicate, pageSize, (pageNum - 1), out count);

        //    var toRet = new List<EmployeeLocationVm>();
        //    foreach (var employee in items)
        //    {
        //        toRet.Add(new EmployeeLocationVm { Employee = employee, Location = null });
        //        if (CheckEmpty.String(employee.Locations) != "")
        //        {
        //            foreach (var locationId in CheckEmpty.String(employee.Locations).Split(','))
        //            {
        //                toRet.Add(new EmployeeLocationVm { Employee = employee, Location = BlLocation.LoadSingle(userId, Convert.ToInt64(locationId)) });
        //            }
        //        }
        //    }

        //    return toRet.Select(el => FormatEmployeeLocationForQs(user.LanguageId, el, returnPin)).ToList();
        //}

        //private static DdlVm.DdlOption FormatEmployeeLocationForQs(int languageId, EmployeeLocationVm item, bool returnPin = false)
        //{
        //    var toRet = new DdlVm.DdlOption
        //    {
        //        value = (returnPin ? item.Employee.Entity.Pin.ToUiString() : item.Employee.Id.ToUiString()) + "-" + (item.Location == null ? 0 : item.Location.Id),
        //        label = string.Format("{0} - {1}", item.Employee.Entity.FullArShortName, (item.Location == null ? "" : item.Location.FullName))
        //    };
        //    return toRet;
        //}

        #endregion QuickSearch

        #region Private methods

        private static Expression<Func<Employee, bool>> CreateFilter(string search)
        {
            var serializer = new JavaScriptSerializer();
            var dict = serializer.Deserialize<Dictionary<string, object>>(search);

            var predicate = PredicateBuilder.True<Employee>();

            predicate = predicate.And(p => p.Id != 1); //Exclude employee presidential palace

            if (CheckEmpty.String(ref dict, "Id") != "")
                predicate = predicate.And(p => p.Id == Convert.ToInt64(dict["Id"]));

            if (CheckEmpty.String(ref dict, "Name") != "")
            {
                foreach (var token in dict["Name"].ToString().Split(' '))
                {
                    var predicate2 = PredicateBuilder.False<Employee>();
                    predicate2 = predicate2.Or(m => m.Entity.FirstName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.FatherName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.FamilyName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFirstName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFatherName.Contains(token));
                    predicate2 = predicate2.Or(m => m.Entity.ArFamilyName.Contains(token));
                    predicate = predicate.And(predicate2);
                }
            }

            if (CheckEmpty.String(ref dict, "Title") != "")
                predicate = predicate.And(p => p.Entity.Title == dict["Title"].ToString());

            if (CheckEmpty.String(ref dict, "Gender") != "")
                predicate = predicate.And(p => p.Entity.Gender == dict["Gender"].ToString());

            if (CheckEmpty.String(ref dict, "Level") != "")
                predicate = predicate.And(p => p.Level == dict["Level"].ToString());

            if (CheckEmpty.String(ref dict, "Number") != "")
            {
                predicate = predicate.And(p => p.Entity.IdType == "M");
                predicate = predicate.And(p => p.Entity.IdNum.Contains(dict["Number"].ToString()));
            }
            return predicate;
        }

        private static void PreSave(long userId, ref Employee toSave, Enumerations.ActionMode action)
        {
            if (action == Enumerations.ActionMode.Add)
            {
                toSave.EntryDate = BlCommon.GetServerDateTime();
                toSave.Entity.EntryDate = toSave.EntryDate;
                toSave.Entity.UserId = toSave.UserId;
                toSave.Entity.Status = toSave.Status;
            }
            else if (action == Enumerations.ActionMode.Edit)
            {
                //toSave.Entity = BlEntity.LoadSingle(userId, toSave.Entity.Pin);
                if (toSave.Entity.Status == "S")
                {
                    //TODO:Check no goods are related to this employee
                }
            }
            toSave.Entity.NameIndex = NameIndex.GetNameIndex(userId, toSave.Entity.FirstName, toSave.Entity.FamilyName, toSave.Entity.FatherName);
            toSave.Entity.NameIndex += NameIndex.GetNameIndex(userId, toSave.Entity.ArFirstName, toSave.Entity.ArFamilyName, toSave.Entity.ArFatherName);

            BlArDict.SetArabicDict(userId, toSave.Entity.FirstName, toSave.Entity.ArFirstName);
            BlArDict.SetArabicDict(userId, toSave.Entity.FatherName, toSave.Entity.ArFatherName);
            BlArDict.SetArabicDict(userId, toSave.Entity.FamilyName, toSave.Entity.ArFamilyName);
        }

        #endregion Private methods
    }
}