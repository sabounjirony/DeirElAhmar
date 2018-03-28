using System.Linq;
using System.Transactions;
using BL.System.Logging;
using DL.Repositories.Setup;
using DM.Models.Setup;
using Tools.Utilities;

namespace BL.Setup
{
    [BlAspect(AspectPriority = 2)]
    public class BlArDict
    {
        public static string GetArabicDict(long userId, string token)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                string toRet;
                var repository = new ArDictRepository();
                var dictEntry = repository.LoadSingle(token.Trim());

                if (dictEntry != null)
                {
                    //Increment occurance
                    dictEntry.Occurance += 1;
                    repository.Edit(dictEntry);

                    BlLog.Log(userId, "Entity", "Edit arabic dictionnary", "ArabicDictModified", new object[] { dictEntry.Token, dictEntry.Replacement, dictEntry.Occurance });

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

        public static string GetLatinDict(long userId, string token)
        {
            using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                string toRet;
                var repository = new ArDictRepository();

                var predicate = PredicateBuilder.True<ArDict>();
                predicate = predicate.And(p => p.Replacement == token.Trim());

                var dictEntry = repository.LoadSearch(predicate).FirstOrDefault();

                if (dictEntry != null)
                {
                    //Increment occurance
                    dictEntry.Occurance += 1;
                    repository.Edit(dictEntry);

                    BlLog.Log(userId, "Entity", "Edit arabic dictionnary", "ArabicDictModified", new object[] { dictEntry.Token, dictEntry.Replacement, dictEntry.Occurance });

                    toRet = dictEntry.Token;
                }
                else
                { toRet = ""; }

                tran.Complete();
                return toRet;
            }
        }

        public static void SetArabicDict(long userId, string token, string replacement)
        {
            if (CheckEmpty.String(token) == "" || CheckEmpty.String(replacement) == "") return;
            using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var repository = new ArDictRepository();
                var dictEntry = repository.LoadSingle(token);

                if (dictEntry != null)
                {
                    if (dictEntry.Replacement == replacement)
                    {
                        //Increment hits
                        dictEntry.Hit += 1;
                        repository.Edit(dictEntry);
                    }
                    else
                    {
                        //Add trash data
                        dictEntry.Trash += (dictEntry.Trash == "" ? "" : "~") + replacement ;
                        repository.Edit(dictEntry);
                    }
                }
                else
                {
                    //Create empty dictionary entry for later management
                    CreateEmptyEntry(userId, token, replacement);
                }
                tran.Complete();
            }
        }

        #region Private methods

        private static void CreateEmptyEntry(long userId, string token, string replacement = "")
        {
            var toAdd = new ArDict
            {
                Token = token,
                Replacement = replacement,
                Occurance = 1,
                Hit = 1,
                Trash = ""
            };
            var repository = new ArDictRepository();
            repository.Create(toAdd);

            BlLog.Log(userId, "Entity", "Create arabic dictionnary", "ArabicDictCreated", new object[] { toAdd.Token, toAdd.Replacement });
        }

        #endregion Private methods
    }
}