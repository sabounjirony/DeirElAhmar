using System.Transactions;
using BL.System.Logging;
using DL.Repositories.Setup;
using DM.Models.Setup;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlAntiDict
    {
        public static string GetAntiDict(long userId, string token)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                string toRet;
                var repository = new AntiDictRepository();
                var dictEntry = repository.LoadSingle(token);

                if (dictEntry != null)
                {
                    //Increment occurance
                    dictEntry.Occurance += 1;
                    repository.Edit(dictEntry);

                    BlLog.Log(userId, "Entity", "Edit anti dictionnary", "AntiDictModified", new object[] { dictEntry.Token, dictEntry.Replacement, dictEntry.Occurance });

                    toRet = dictEntry.Replacement;
                }
                else
                {
                    //Create empty dictionary entry for later management
                    CreateEmptyEntry(userId, token);
                    toRet = token;
                }
                tran.Complete();
                return toRet;
            }
        }

        #region Private methods

        private static void CreateEmptyEntry(long userId, string token, string replacement = "")
        {
            var toAdd = new AntiDict
            {
                Token = token,
                Replacement = replacement,
                Occurance = 0
            };
            var repository = new AntiDictRepository();
            repository.Create(toAdd);

            BlLog.Log(userId, "Entity", "Create anti dictionnary", "AntiDictCreated", new object[] { toAdd.Token, toAdd.Replacement });
        }

        #endregion Private methods
    }
}