using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using BL.System;
using BL.System.Logging;
using BL.System.Security;
using DL.Repositories.Setup;
using DM.Models.Setup;
using Tools.Utilities;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlAddress
    {
        #region Members

        private const string Module = "Address";
        readonly AddressRepository _repository = new AddressRepository();

        #endregion Members

        #region Actions

        public Address Create(long userId, Address toAdd)
        {
            using (var tran = new TransactionScope())
            {
                toAdd.EntryDate = BlCommon.GetServerDateTime();
                toAdd.UserId = userId;
                toAdd.Sequence = GetPinNextSequence(userId, toAdd.Entity.Pin);
                var toRet = _repository.Create(toAdd);

                BlLog.Log(userId, Module, "Create address", "AddressCreated", new object[] { toAdd.Sequence, toAdd.Entity.Pin, BlEntity.FormatFullName(toAdd.Entity) });
                tran.Complete();
                return toRet;
            }
        }

        public Address Edit(long userId, Address toEdit)
        {
            using (var tran = new TransactionScope())
            {
                if (toEdit.EntryDate == DateTime.MinValue)
                { toEdit.EntryDate = BlCommon.GetServerDateTime(); }

                var toRet = _repository.Edit(toEdit);

                BlLog.Log(userId, Module, "Edit address", "AddressModified", new object[] { toEdit.Sequence, toEdit.Entity.Pin, BlEntity.FormatFullName(toEdit.Entity) });
                tran.Complete();
                return toRet;
            }
        }

        public bool Delete(long userId, Address toDelete)
        {
            using (var tran = new TransactionScope())
            {
                var toRet = _repository.Delete(toDelete);

                BlLog.Log(userId, Module, "Delete address", "AddressDeleted", new object[] { toDelete.Sequence, toDelete.Entity.Pin, BlEntity.FormatFullName(toDelete.Entity) });
                tran.Complete();
                return toRet;
            }
        }

        #endregion Actions

        #region Queries

        public Address LoadSingle(long userId, long pin)
        {
            var toRet = _repository.LoadSingle(pin);
            return toRet;
        }

        public IEnumerable<Address> LoadSearch(long userId, Expression<Func<Address, bool>> predicate, int count = 0)
        {
            var toRet = _repository.LoadSearch(predicate, count);
            return toRet;
        }

        public IEnumerable<Address> LoadPaging(long userId, Expression<Func<Address, bool>> predicate, int pageSize, int pageNum, out long totCount)
        {
            var toRet = _repository.LoadPaging(predicate, pageSize, pageNum, out totCount);
            return toRet;
        }

        public static Address GetDefaultAddress(long userId, long pin, bool withFullAddress = false)
        {
            var predicate = PredicateBuilder.True<Address>();
            predicate = predicate.And(m => m.Entity.Pin == pin);
            predicate = predicate.And(m => m.Sequence == 1);

            var blAddress = new BlAddress();
            var toRet = blAddress.LoadSearch(userId, predicate, 1).SingleOrDefault() ?? new Address()
            {
                Country = "0",
                Province = "0",
                Caza = "0",
                Region = "",
                Street = "",
                Building = "",
                Floor = "0",
                Phone1 = "",
                Phone2 = "",
                Phone3 = "",
                Fax = ""
            };

            if (withFullAddress)
            {
                //Set the full address
                toRet.FullAddress = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}-{10}",
                    BlCode.LoadSingle(userId, "Country", toRet.Country ?? "0").Value1,
                    BlCode.LoadSingle(userId, "Province", toRet.Province ?? "0").Value1,
                    BlCode.LoadSingle(userId, "Caza", toRet.Caza ?? "0").Value1,
                    toRet.Region,
                    toRet.Street,
                    toRet.Building,
                    BlCode.LoadSingle(userId, "Floor", toRet.Floor ?? "0").Value1,
                    toRet.Phone1,
                    toRet.Phone2,
                    toRet.Phone3,
                    toRet.Fax);
            }

            return toRet;
        }

        #endregion Queries

        #region Private methods

        private int GetPinNextSequence(long userId, long pin)
        {
            var predicate = PredicateBuilder.True<Address>();
            predicate = predicate.And(m => m.Entity.Pin == pin);

            var addresses = LoadSearch(userId, predicate).OrderByDescending(a => a.Sequence);
            if (!addresses.Any()) return 1;
            return addresses.First().Sequence + 1;
        }

        #endregion Private methods
    }
}